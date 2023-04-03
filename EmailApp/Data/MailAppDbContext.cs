using EmailApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailApp.Data
{
	public class MailAppDbContext : DbContext
	{
		public MailAppDbContext(DbContextOptions<MailAppDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
            
        }

		public DbSet<MailUser> Users { get; set; }
		public DbSet<Message> Messages { get; set; }
	}
}
