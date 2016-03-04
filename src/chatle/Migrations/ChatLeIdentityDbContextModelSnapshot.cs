using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using ChatLe.Models;

namespace Chatle.Migrations
{
    [DbContext(typeof(ChatLeIdentityDbContext))]
    partial class ChatLeIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ChatLe.Models.Attendee<string>", b =>
                {
                    b.Property<string>("ConversationId");

                    b.Property<string>("UserId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("ConversationId", "UserId");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Attendee<string>");

                    b.HasAnnotation("Relational:TableName", "Attendees");
                });

            modelBuilder.Entity("ChatLe.Models.ChatLeUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("ChatLe.Models.Conversation<string>", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Conversation<string>");

                    b.HasAnnotation("Relational:TableName", "Conversations");
                });

            modelBuilder.Entity("ChatLe.Models.Message<string>", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConversationId");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Text");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "Message<string>");

                    b.HasAnnotation("Relational:TableName", "Messages");
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection<string>", b =>
                {
                    b.Property<string>("ConnectionId");

                    b.Property<string>("NotificationType");

                    b.Property<string>("ChatLeUserId");

                    b.Property<DateTime>("ConnectionDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("ConnectionId", "NotificationType");

                    b.HasAnnotation("Relational:DiscriminatorProperty", "Discriminator");

                    b.HasAnnotation("Relational:DiscriminatorValue", "NotificationConnection<string>");

                    b.HasAnnotation("Relational:TableName", "NotificationConnections");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("ChatLe.Models.Attendee", b =>
                {
                    b.HasBaseType("ChatLe.Models.Attendee<string>");


                    b.HasAnnotation("Relational:DiscriminatorValue", "Attendee");
                });

            modelBuilder.Entity("ChatLe.Models.Conversation", b =>
                {
                    b.HasBaseType("ChatLe.Models.Conversation<string>");


                    b.HasAnnotation("Relational:DiscriminatorValue", "Conversation");
                });

            modelBuilder.Entity("ChatLe.Models.Message", b =>
                {
                    b.HasBaseType("ChatLe.Models.Message<string>");


                    b.HasAnnotation("Relational:DiscriminatorValue", "Message");
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection", b =>
                {
                    b.HasBaseType("ChatLe.Models.NotificationConnection<string>");


                    b.HasAnnotation("Relational:DiscriminatorValue", "NotificationConnection");
                });

            modelBuilder.Entity("ChatLe.Models.Attendee<string>", b =>
                {
                    b.HasOne("ChatLe.Models.Conversation<string>")
                        .WithMany()
                        .HasForeignKey("ConversationId");
                });

            modelBuilder.Entity("ChatLe.Models.Message<string>", b =>
                {
                    b.HasOne("ChatLe.Models.Conversation<string>")
                        .WithMany()
                        .HasForeignKey("ConversationId");
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany()
                        .HasForeignKey("ChatLeUserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ChatLe.Models.Attendee", b =>
                {
                });

            modelBuilder.Entity("ChatLe.Models.Message", b =>
                {
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection", b =>
                {
                });
        }
    }
}
