namespace OsFacil.Mobile.Api.Pages
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage(PageModels.TaskDetailPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
