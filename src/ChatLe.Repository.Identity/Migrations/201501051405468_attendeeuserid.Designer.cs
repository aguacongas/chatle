using ChatLe.Models;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using System;

namespace ChatLe.Repository.Identity.Migrations
{
    [ContextType(typeof(ChatLeIdentityDbContext))]
    public partial class attendeeuserid : IMigrationMetadata
    {
        string IMigrationMetadata.MigrationId
        {
            get
            {
                return "201501051405468_attendee-userid";
            }
        }
        
        string IMigrationMetadata.ProductVersion
        {
            get
            {
                return "7.0.0-beta1-11518";
            }
        }
        
        IModel IMigrationMetadata.TargetModel
        {
            get
            {
                var builder = new BasicModelBuilder();
                
                builder.Entity("ChatLe.Models.Attendee", b =>
                    {
                        b.Property<string>("ConversationId");
                        b.Property<string>("UserId");
                        b.Key("ConversationId", "UserId");
                        b.ForRelational().Table("Attendees");
                    });
                
                builder.Entity("ChatLe.Models.ChatLeUser", b =>
                    {
                        b.Property<int>("AccessFailedCount");
                        b.Property<string>("Email");
                        b.Property<bool>("EmailConfirmed");
                        b.Property<string>("Id");
                        b.Property<bool>("LockoutEnabled");
                        b.Property<DateTimeOffset>("LockoutEnd");
                        b.Property<string>("NormalizedUserName");
                        b.Property<string>("PasswordHash");
                        b.Property<string>("PhoneNumber");
                        b.Property<bool>("PhoneNumberConfirmed");
                        b.Property<string>("SecurityStamp");
                        b.Property<bool>("TwoFactorEnabled");
                        b.Property<string>("UserName");
                        b.Key("Id");
                        b.ForRelational().Table("AspNetUsers");
                    });
                
                builder.Entity("ChatLe.Models.Conversation", b =>
                    {
                        b.Property<string>("Id");
                        b.Key("Id");
                        b.ForRelational().Table("Conversations");
                    });
                
                builder.Entity("ChatLe.Models.Message", b =>
                    {
                        b.Property<string>("ConversationId");
                        b.Property<DateTime>("Date");
                        b.Property<string>("Id");
                        b.Property<string>("Text");
                        b.Property<string>("UserId");
                        b.Key("Id");
                        b.ForRelational().Table("Messages");
                    });
                
                builder.Entity("ChatLe.Models.NotificationConnection", b =>
                    {
                        b.Property<DateTime>("ConnectionDate");
                        b.Property<string>("ConnectionId");
                        b.Property<string>("NotificationType");
                        b.Property<string>("UserId");
                        b.Key("ConnectionId", "NotificationType");
                        b.ForRelational().Table("NotificationConnections");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityRole", b =>
                    {
                        b.Property<string>("Id");
                        b.Property<string>("Name");
                        b.Key("Id");
                        b.ForRelational().Table("AspNetRoles");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType");
                        b.Property<string>("ClaimValue");
                        b.Property<int>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<string>("RoleId");
                        b.Key("Id");
                        b.ForRelational().Table("AspNetRoleClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("ClaimType");
                        b.Property<string>("ClaimValue");
                        b.Property<int>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<string>("UserId");
                        b.Key("Id");
                        b.ForRelational().Table("AspNetUserClaims");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("LoginProvider");
                        b.Property<string>("ProviderDisplayName");
                        b.Property<string>("ProviderKey");
                        b.Property<string>("UserId");
                        b.Key("LoginProvider", "ProviderKey");
                        b.ForRelational().Table("AspNetUserLogins");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityUserRole`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.Property<string>("RoleId");
                        b.Property<string>("UserId");
                        b.Key("UserId", "RoleId");
                        b.ForRelational().Table("AspNetUserRoles");
                    });
                
                builder.Entity("ChatLe.Models.Attendee", b =>
                    {
                        b.ForeignKey("ChatLe.Models.Conversation", "ConversationId");
                        b.ForeignKey("ChatLe.Models.ChatLeUser", "UserId");
                    });
                
                builder.Entity("ChatLe.Models.Message", b =>
                    {
                        b.ForeignKey("ChatLe.Models.ChatLeUser", "UserId");
                        b.ForeignKey("ChatLe.Models.Conversation", "ConversationId");
                    });
                
                builder.Entity("ChatLe.Models.NotificationConnection", b =>
                    {
                        b.ForeignKey("ChatLe.Models.ChatLeUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityRoleClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("Microsoft.AspNet.Identity.IdentityRole", "RoleId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityUserClaim`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("ChatLe.Models.ChatLeUser", "UserId");
                    });
                
                builder.Entity("Microsoft.AspNet.Identity.IdentityUserLogin`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", b =>
                    {
                        b.ForeignKey("ChatLe.Models.ChatLeUser", "UserId");
                    });
                
                return builder.Model;
            }
        }
    }
}