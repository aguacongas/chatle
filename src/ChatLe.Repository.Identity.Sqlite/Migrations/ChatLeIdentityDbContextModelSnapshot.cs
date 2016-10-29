using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ChatLe.Repository.Identity.Sqlite;

namespace ChatLe.Repository.Identity.Sqlite.Migrations
{
    [DbContext(typeof(ChatLeIdentityDbContext))]
    partial class ChatLeIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("ChatLe.Models.Attendee<string>", b =>
                {
                    b.Property<string>("ConversationId");

                    b.Property<string>("UserId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsConnected");

                    b.HasKey("ConversationId", "UserId");

                    b.HasIndex("ConversationId");

                    b.ToTable("Attendees");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Attendee<string>");
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

                    b.Property<DateTime>("LastLoginDate");

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
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ChatLe.Models.Conversation<string>", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Conversations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Conversation<string>");
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

                    b.HasIndex("ConversationId");

                    b.ToTable("Messages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Message<string>");
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

                    b.HasIndex("ChatLeUserId");

                    b.ToTable("NotificationConnections");

                    b.HasDiscriminator<string>("Discriminator").HasValue("NotificationConnection<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
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
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ChatLe.Models.Attendee", b =>
                {
                    b.HasBaseType("ChatLe.Models.Attendee<string>");


                    b.ToTable("Attendee");

                    b.HasDiscriminator().HasValue("Attendee");
                });

            modelBuilder.Entity("ChatLe.Models.Conversation", b =>
                {
                    b.HasBaseType("ChatLe.Models.Conversation<string>");


                    b.ToTable("Conversation");

                    b.HasDiscriminator().HasValue("Conversation");
                });

            modelBuilder.Entity("ChatLe.Models.Message", b =>
                {
                    b.HasBaseType("ChatLe.Models.Message<string>");


                    b.ToTable("Message");

                    b.HasDiscriminator().HasValue("Message");
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection", b =>
                {
                    b.HasBaseType("ChatLe.Models.NotificationConnection<string>");


                    b.ToTable("NotificationConnection");

                    b.HasDiscriminator().HasValue("NotificationConnection");
                });

            modelBuilder.Entity("ChatLe.Models.Attendee<string>", b =>
                {
                    b.HasOne("ChatLe.Models.Conversation<string>", "Conversation")
                        .WithMany("Attendees")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ChatLe.Models.Message<string>", b =>
                {
                    b.HasOne("ChatLe.Models.Conversation<string>", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");
                });

            modelBuilder.Entity("ChatLe.Models.NotificationConnection<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany("NotificationConnections")
                        .HasForeignKey("ChatLeUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChatLe.Models.ChatLeUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
