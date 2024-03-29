﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Access_API.SignalR
{

    public class SignalRHandler : Hub
    {
        static string _suggestorClientId = "";

        public void SuggesterJoin()
        {
            _suggestorClientId = Context.ConnectionId;
            Debug.WriteLine("Here is the suggesterClientId: " + _suggestorClientId);
        }

        public async Task SendGroupMessage(string groupName, string messageTag, string message)
        {
            switch (messageTag)
            {
                case "status":
                    {
                        await Clients.Group(groupName).SendAsync("status", message);
                        break;
                    }
                case "suggestionRequest":
                    {
                        await Clients.Group(_suggestorClientId).SendAsync("suggestionRequest", groupName, message);
                        break;
                    }
                case "suggestionResponse":
                    {
                        await Clients.Group(groupName).SendAsync("suggestionResponse", message);
                        break;
                    }
                case "suggestionRequestTest":
                    {
                        var request = JsonConvert.DeserializeObject<SuggesterRequest>(message)!;
                        var testData = SuggestorSimulator.GenerateTestResponse(request);
                        await Clients.Group(groupName).SendAsync("suggestionResponse", JsonConvert.SerializeObject(testData));
                        break;
                    }
                case "evaluateSentence":
                    {
                        await Clients.Group(_suggestorClientId).SendAsync("evaluateSentence", message);
                        break;
                    }
                default:
                    {
                        await Clients.Group(groupName).SendAsync("error", "unsupported messageTag");
                        break;
                    }
            }
        }

        //Used by suggestor to join a group with client
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Debug.WriteLine($"Suggestor joined group: {groupName}");
        }

        //Used by suggestor to leave a group when client has left
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Debug.WriteLine($"Suggestor left group: {groupName}");
        }

        //Called upon clients connects
        public async void AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Client(_suggestorClientId).SendAsync("JoinGroup", groupName);
            Debug.WriteLine($"Added {Context.ConnectionId} to group: {groupName}");
        }

        //Called upon clients disconnects
        public async void RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Client(_suggestorClientId).SendAsync("LeaveGroup", groupName);
            Debug.WriteLine($"Remove {Context.ConnectionId} from group: {groupName}");
        }

        public override Task OnConnectedAsync()
        {
            Debug.WriteLine($"Client {Context.ConnectionId} join the HUB");
            AddToGroup(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Debug.WriteLine($"Client {Context.ConnectionId} left the HUB");
            RemoveFromGroup(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
