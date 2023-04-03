using EmailApp.Data;
using EmailApp.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("mailappdb") ?? throw new InvalidOperationException("Connection string 'mailappdb' not found.");

builder.Services.AddDbContext<MailAppDbContext>(options => options.UseSqlServer(connectionString, builder =>
{
    builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null);
}));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IUserIdProvider, CustomNameProvider>();
builder.Services.AddSignalR();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.ExpireTimeSpan = TimeSpan.FromDays(1);
		options.SlidingExpiration = true;
		options.AccessDeniedPath = "/Login/";
		options.LoginPath = "/Login/";
		options.LogoutPath = "/Logout/";
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MailHub>("/mailhub");

app.MapRazorPages();

app.Run();
