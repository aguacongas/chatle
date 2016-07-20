using Microsoft.EntityFrameworkCore;

namespace ChatLe.Repository.Identity.SqlServer
{
    public class ChatLeIdentityDbContext: ChatLe.Models.ChatLeIdentityDbContext
	{
        public ChatLeIdentityDbContext()
        {
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=chatle;Trusted_Connection=True;MultipleActiveResultSets=true");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
