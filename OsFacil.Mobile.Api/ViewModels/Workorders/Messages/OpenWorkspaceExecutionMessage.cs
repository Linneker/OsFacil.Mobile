using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.ViewModels.Workorders.Messages;

public sealed class OpenWorkspaceExecutionMessage : ValueChangedMessage<Guid>
{
    public OpenWorkspaceExecutionMessage(Guid workspaceId) : base(workspaceId) { }
}
