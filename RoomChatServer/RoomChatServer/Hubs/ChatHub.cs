using Microsoft.AspNetCore.SignalR;

namespace RoomChatServer.Hubs
{
   
    
    
    public class ChatHub:Hub
    {
        private static Dictionary<string, string> connectedClients = new Dictionary<string, string>();



        //this method will send notification to all client
        //if client have to communicate ,it will call SendMessage() method
        //if client have to recieve notification from server it will use <RecieveMessage> method
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
        //Everyone will be notified except who have joined the chat
        public async Task JoinChat(string user, string message)
        {
            Context.Items["UserIdentifier"] = user;
            connectedClients[Context.ConnectionId] = user;
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
            await UpdateUsersList();


        }

        private async Task LeaveChat()
        {
            if (connectedClients.TryGetValue(Context.ConnectionId, out string user))
            {
                connectedClients.Remove(Context.ConnectionId);
                var message = $"{user} left the chat";
                await Clients.Others.SendAsync("ReceiveMessage", user, message);
            }
        }

        public async override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string userIdentifier = Context.Items["UserIdentifier"] as string;

            // Add the connection ID and user identifier to the concurrent dictionary
            connectedClients.TryAdd(connectionId, userIdentifier);

            await UpdateUsersList();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await LeaveChat();
            await UpdateUsersList();
            await base.OnDisconnectedAsync(exception);

        }

        private async Task UpdateUsersList()
        {
            var users = connectedClients.Values.ToList();
            await Clients.All.SendAsync("UsersInRoom", users);
        }

        public Task SendUsersConnected(string user)
        {
            var users = connectedClients.Values.ToList();

            return  Clients.All.SendAsync("UsersInRoom", users);
        }

    }
}
