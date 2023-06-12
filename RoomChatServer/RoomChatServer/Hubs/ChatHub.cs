using Microsoft.AspNetCore.SignalR;

namespace RoomChatServer.Hubs
{
    public class ChatHub:Hub
    {   //this method will send notification to all client
        //if client have to communicate ,it will call SendMessage() method
        //if client have to recieve notification from server it will use <RecieveMessage> method
        public async Task SendMessage(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
        //Everyone will be notified except who have joined the chat
        public async Task JoinChat(string user, string message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }
    }
}
