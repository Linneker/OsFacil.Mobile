using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.ViewModels.Clients.Messagens;

public sealed class ClientsChangedMessage : ValueChangedMessage<bool>
{
    public ClientsChangedMessage(bool load) : base(load) { }
}
