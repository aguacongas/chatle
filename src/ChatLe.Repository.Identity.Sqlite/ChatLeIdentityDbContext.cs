using Microsoft.EntityFrameworkCore;

namespace ChatLe.Repository.Identity.Sqlite
{
    public class ChatLeIdentityDbContext: ChatLe.Models.ChatLeIdentityDbContext
	{
        public ChatLeIdentityDbContext()
        {
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=./ChatLeDbContext.db");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
