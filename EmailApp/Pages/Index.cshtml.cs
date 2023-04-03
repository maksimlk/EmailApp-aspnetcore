using EmailApp.Data;
using EmailApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;


namespace EmailApp.Pages
{
	[Authorize]
	public class IndexModel : PageModel, IDisposable
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly MailAppDbContext _context;
		private readonly NavigationManager _navmanager;

		private HubConnection? hubConnection;
		public IndexModel(ILogger<IndexModel> logger, MailAppDbContext context, NavigationManager navmanager)
		{
			_logger = logger;
			_context = context;
			_navmanager = navmanager;
		}

		public IList<Message> Messages { get; set; } = new List<Message>();
		public Message SampleMessage { get; set; } = new Message();

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[DataType(DataType.Text)]
			[StringLength(50)]
			public string RecipientName { get; set; }
			[Required]
			[DataType(DataType.Text)]
			[StringLength(255)]
			public string Subject { get; set; }
			[Required]
			[DataType(DataType.MultilineText)]
			public string Body { get; set; }
		}

		public IActionResult OnGet()
		{
			string currentUserName = User.Identity.Name;
			var currentUser = _context.Users.FirstOrDefault(user => user.UserName == currentUserName);
			if (currentUser == null)
				return LocalRedirect("/Login/");
			PopulateMessageList(currentUser);
			return Page();
		}

		public IActionResult OnGetSearch(string term)
		{
			var users = _context.Users.Where(u => u.UserName.StartsWith(term)).Select(u => u.UserName).ToList();
			return new JsonResult(users);
		}

		private void PopulateMessageList(MailUser currentUser)
		{
			var messages = _context.Messages.Where<Message>(m => m.Recipient == currentUser).ToList();
			if (!messages.IsNullOrEmpty())
				Messages = currentUser.Messages.OrderByDescending(message => message.ID).ToList();
		}

		public async Task<IActionResult> OnPostSendMessageAsync()
		{
			var recipientUserName = Input.RecipientName;
			var recipient = GetRecipientOrCreate(recipientUserName);
			var message = CreateMessage(recipient);
			SaveMessage(message);
			_logger.LogInformation(User.Identity.Name + " send a message to " + recipientUserName);
			return LocalRedirect("/Index/");
		}

		private MailUser GetRecipientOrCreate(string recipientUserName)
		{
			var recipient = _context.Users.FirstOrDefault(user => user.UserName == recipientUserName);
			if (recipient == null)
			{
				recipient = new MailUser { UserName = recipientUserName };
				_context.Users.Add(recipient);
				_context.SaveChanges();
			}
			return recipient;
		}

		private Message CreateMessage(MailUser recipient)
		{
			var message = new Message { Sender = User.Identity.Name, Recipient = recipient, Subject = Input.Subject, Body = Input.Body };
			return message;
		}
		private void SaveMessage(Message message)
		{
			_context.Messages.Add(message);
			_context.SaveChanges();
		}

		public async void Dispose()
		{
			if (hubConnection is not null)
			{
				await hubConnection.DisposeAsync();
			}
		}
	}
}