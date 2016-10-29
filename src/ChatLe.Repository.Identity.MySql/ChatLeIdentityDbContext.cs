using Microsoft.EntityFrameworkCore;

namespace ChatLe.Repository.Identity.MySql
{
    public class ChatLeIdentityDbContext: ChatLe.Models.ChatLeIdentityDbContext
	{
        public ChatLeIdentityDbContext()
        {
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql("Server=localhost;database=chatle;uid=root;pwd=AKgrDOyzL9_w;");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
