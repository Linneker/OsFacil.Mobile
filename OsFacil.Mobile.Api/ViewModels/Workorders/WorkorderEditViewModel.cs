using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.Models.Workorders;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Messages;
using OsFacil.Mobile.Api.ViewModels.Workorders.Messages;
using OsFacil.Mobile.Service.Https.Workorders;
using OsFacil.Mobile.Service.Https.Workorders.Response;

namespace OsFacil.Mobile.Api.ViewModels.Workorders;

public partial class WorkorderEditViewModel : ObservableObject
{
    private readonly IWorkspaceHttp _workspaceHttp;
    private readonly IAuthSession _session;
    private readonly IToastService _toast;
    private readonly IFlyoutNavigationService _navigation;

    private Guid _workorderId;

    private static readonly List<string> StatusLabels = ["Aberto", "Em andamento", "Concluido", "Cancelado"];

    public WorkorderEditViewModel(
        IWorkspaceHttp workspaceHttp,
        IAuthSession session,
        IToastService toast,
        IFlyoutNavigationService navigation)
    {
        _workspaceHttp = workspaceHttp;
        _session = session;
        _toast = toast;
        _navigation = navigation;

        WeakReferenceMessenger.Default.Register<SessionClearedMessage>(this, (r, _) =>
        {
            var vm = (WorkorderEditViewModel)r;
            vm._workorderId = Guid.Empty;
            vm.Model.Clean();
        });
    }

    public void PrepareLoad(Guid workorderId)
    {
        _workorderId = workorderId;
        Model.Clean();
    }

    [ObservableProperty] private EditWorkorderModel model = new();
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private ImageSource? selectedImage;
    [ObservableProperty] private bool isImageExpanded;

    [RelayCommand]
    private void ExpandImage(ImageSource image)
    {
        SelectedImage = image;
        IsImageExpanded = true;
    }

    [RelayCommand]
    private void CloseImage()
    {
        IsImageExpanded = false;
        SelectedImage = null;
    }

    public bool IsNotBusy => !IsBusy;
    public string SaveButtonText => IsBusy ? "Salvando..." : "Salvar";

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_workorderId == Guid.Empty) return;

        try
        {
            Model.IsLoading = true;

            var token = _session.AccessToken ?? "";
            var response = await _workspaceHttp.GetWorkOrdersByIdAsync(token, _workorderId);

            if (response.Data?.WorkOrder is not null)
            {
                var wo = response.Data.WorkOrder;
                Model.Id = Guid.Parse(wo.Id);
                Model.ClientId = wo.ClientId;
                Model.ClientName = wo.ClientName ?? "";
                Model.Title = wo.Title ?? "";
                Model.Description = wo.Description ?? "";
                Model.Amount = (decimal)wo.Amount;
                Model.SelectedStatusIndex = wo.Status;

                // Carregar fotos existentes
                Model.BeforeImages.Clear();
                Model.AfterImages.Clear();

                if (response.Data.Photos is { Count: > 0 })
                {
                    // Baixa todas as fotos em paralelo
                    var tasks = response.Data.Photos
                        .Where(p => !string.IsNullOrWhiteSpace(p.Url))
                        .Select(async p =>
                        {
                            var bytes = await _workspaceHttp.DownloadPhotoAsync(token, p.Url);
                            if (bytes is null) return (p.Kind, Image: (FileImageSource?)null);
                            var imgSource = await BytesToFileImageAsync(bytes);
                            return (p.Kind, Image: imgSource);
                        });

                    var results = await Task.WhenAll(tasks);

                    foreach (var (kind, image) in results)
                    {
                        if (image is null) continue;
                        if (kind == 0) Model.BeforeImages.Add(image);
                        else if (kind == 1) Model.AfterImages.Add(image);
                    }
                }
            }
            else
            {
                await _toast.ShowAsync("Não foi possível carregar a ordem de serviço.");
            }
        }
        catch
        {
            await _toast.ShowAsync("Erro ao carregar a ordem de serviço.");
        }
        finally
        {
            Model.IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveStatusAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var token = _session.AccessToken ?? "";
            var request = new UpdateWorkOrderHttp
            {
                ClientId = Model.ClientId,
                Title = Model.Title,
                Description = Model.Description,
                Status = Model.SelectedStatusIndex,
                Amount = Model.Amount
            };

            var response = await _workspaceHttp.UpadateWorkOrderAsync(token, Model.Id, request);

            if (response.IsSuccessStatusCode)
                await _toast.ShowAsync("Ordem atualizada com sucesso!");
            else
                await _toast.ShowAsync(response.Error ?? "Erro ao salvar a ordem.");
        }
        catch
        {
            await _toast.ShowAsync("Erro ao salvar o status.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddBeforeImageAsync()
    {
        await CaptureAndUploadAsync(0, Model.BeforeImages);
    }

    [RelayCommand]
    private async Task AddAfterImageAsync()
    {
        await CaptureAndUploadAsync(1, Model.AfterImages);
    }

    private async Task CaptureAndUploadAsync(int kind, System.Collections.ObjectModel.ObservableCollection<ImageSource> targetCollection)
    {
        try
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page is null)
                return;

            var action = await page.DisplayActionSheetAsync(
                "Selecionar foto", "Cancelar", null, "Câmera", "Galeria");

            if (action == "Câmera")
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    await _toast.ShowAsync("Permissão de câmera negada.");
                    return;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo is null) return;

                IsBusy = true;
                await UploadFileAsync(photo, kind, targetCollection);
            }
            else if (action == "Galeria")
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Photos>();

                if (status != PermissionStatus.Granted)
                {
                    await _toast.ShowAsync("Permissão de galeria negada.");
                    return;
                }

                var files = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Selecione as fotos",
                    FileTypes = FilePickerFileType.Images
                });

                if (files is null || !files.Any()) return;

                IsBusy = true;
                foreach (var file in files)
                    await UploadFileAsync(file, kind, targetCollection);
            }
        }
        catch (Exception ex)
        {
            await _toast.ShowAsync($"Erro: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UploadFileAsync(FileResult file, int kind, System.Collections.ObjectModel.ObservableCollection<ImageSource> targetCollection)
    {
        using var fileStream = await file.OpenReadAsync();
        var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var token = _session.AccessToken ?? "";
        var response = await _workspaceHttp.UploadFileAsync(
            token, Model.Id, kind,
            new MemoryStream(imageBytes), file.FileName, file.ContentType ?? "image/jpeg");

        if (response.IsSuccessStatusCode)
        {
            var imgSource = await BytesToFileImageAsync(imageBytes);
            MainThread.BeginInvokeOnMainThread(() => targetCollection.Add(imgSource));
            await _toast.ShowAsync("Foto enviada com sucesso!");
        }
        else
        {
            await _toast.ShowAsync(response.Error ?? "Erro ao enviar a foto.");
        }
    }

    private static async Task<FileImageSource> BytesToFileImageAsync(byte[] bytes)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid():N}.jpg");
        await File.WriteAllBytesAsync(path, bytes);
        return (FileImageSource)ImageSource.FromFile(path);
    }

    [RelayCommand]
    private async Task SharePdfAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var token = _session.AccessToken ?? "";
            using var pdfStream = await _workspaceHttp.GetWorkOrderPdfAsync(token, Model.Id);

            if (pdfStream is null)
            {
                await _toast.ShowAsync("Não foi possível baixar o PDF.");
                return;
            }

            var filePath = Path.Combine(FileSystem.CacheDirectory, $"OS_{Model.Id:N}.pdf");
            using (var fileStream = File.Create(filePath))
                await pdfStream.CopyToAsync(fileStream);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"OS - {Model.Title}",
                File = new ShareFile(filePath)
            });
        }
        catch
        {
            await _toast.ShowAsync("Erro ao compartilhar o PDF.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task BackAsync()
    {
        WeakReferenceMessenger.Default.Send(new WorkordersChangedMessage(true));
        Model.Clean();
        return _navigation.PopAsync();
    }
}
