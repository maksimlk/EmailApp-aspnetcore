using Microsoft.AspNetCore.SignalR;

namespace EmailApp.Hubs
{
	public class MailHub : Hub
	{
		public Task SendMessage(string receiver)
		{
			return Clients.User(receiver).SendAsync("ReceiveMessage");
		}
	}
}
