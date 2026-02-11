using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OsFacil.Mobile.Api.ViewModels.Workorders.Messages;

public sealed class WorkordersChangedMessage : ValueChangedMessage<bool>
{
    public WorkordersChangedMessage(bool load) : base(load) { }
}
