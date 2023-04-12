using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmailApp.Data
{
	public class CustomNameProvider : IUserIdProvider
	{
		public virtual string? GetUserId(HubConnectionContext context)
		{
			return context.User?.FindFirst(ClaimTypes.Name)?.Value;
		}
	}
}
