using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Data.Entity.Metadata;

namespace ChatLe.Models
{
    
    public class ChatLeIdentityDbContext : IdentityDbContext<ChatLeUser>
    {
        public DbSet<Message> Messages { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Attendee> Attendees { get; set; }

        public ChatLeIdentityDbContext()
        {
            Trace.TraceInformation("[ApplicationDbContext] constructor");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Conversation>(b =>
            {
                b.Key(c => c.Id);
                b.ForRelational().Table("Conversations");
            });

            builder.Entity<Message>((Action<ModelBuilder.EntityBuilder<Message>>)((ModelBuilder.EntityBuilder<Message> b) =>
            {
                b.Key(m => m.Id);
                b.ForeignKey<Models.ChatLeUser>(m => m.UserId);
                b.ForeignKey<Conversation>(m => m.ConversationId);
                b.ForRelational().Table("Messages");
            }));

            builder.Entity<Attendee>(b =>
            {
                b.Key(a => new { a.ConversationId, a.UserId });
                b.ForeignKey<Conversation>(a => a.ConversationId);
                b.ForRelational().Table("Attendees");
            });

            var model = builder.Model;
            var conversationType = model.GetEntityType(typeof(Conversation));
            var messageType = model.GetEntityType(typeof(Message));
            var attendeeType = model.GetEntityType(typeof(Attendee));
            var fkcm = messageType.GetOrAddForeignKey(messageType.GetProperty("UserId"), conversationType.GetPrimaryKey());
            conversationType.AddNavigation("Messsages", fkcm, false);
            var fkca = attendeeType.GetOrAddForeignKey(attendeeType.GetProperty("ConversationId"), conversationType.GetPrimaryKey());
            conversationType.AddNavigation("Attendees", fkca, false);
        }
    }
}