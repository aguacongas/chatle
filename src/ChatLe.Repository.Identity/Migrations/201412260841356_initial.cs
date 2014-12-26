using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Model;
using System;

namespace ChatLe.Repository.Identity.Migrations
{
    public partial class initial : Migration
    {
        public override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Attendees",
                c => new
                    {
                        ConversationId = c.String(),
                        UserId = c.String()
                    })
                .PrimaryKey("PK_Attendees", t => new { t.ConversationId, t.UserId });
            
            migrationBuilder.CreateTable("AspNetUsers",
                c => new
                    {
                        Id = c.String(),
                        AccessFailedCount = c.Int(nullable: false),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        LockoutEnabled = c.Boolean(nullable: false),
                        LockoutEnd = c.DateTimeOffset(nullable: false),
                        NormalizedUserName = c.String(),
                        PasswordHash = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        SecurityStamp = c.String(),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        UserName = c.String()
                    })
                .PrimaryKey("PK_AspNetUsers", t => t.Id);
            
            migrationBuilder.CreateTable("Conversations",
                c => new
                    {
                        Id = c.String()
                    })
                .PrimaryKey("PK_Conversations", t => t.Id);
            
            migrationBuilder.CreateTable("Messages",
                c => new
                    {
                        Id = c.String(),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                        UserId = c.String(),
                        ConversationId = c.String()
                    })
                .PrimaryKey("PK_Messages", t => t.Id);
            
            migrationBuilder.CreateTable("NotificationConnections",
                c => new
                    {
                        ConnectionId = c.String(),
                        NotificationType = c.String(),
                        ConnectionDate = c.DateTime(nullable: false),
                        UserId = c.String()
                    })
                .PrimaryKey("PK_NotificationConnections", t => new { t.ConnectionId, t.NotificationType });
            
            migrationBuilder.CreateTable("AspNetRoles",
                c => new
                    {
                        Id = c.String(),
                        Name = c.String()
                    })
                .PrimaryKey("PK_AspNetRoles", t => t.Id);
            
            migrationBuilder.CreateTable("AspNetRoleClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        RoleId = c.String()
                    })
                .PrimaryKey("PK_AspNetRoleClaims", t => t.Id);
            
            migrationBuilder.CreateTable("AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        UserId = c.String()
                    })
                .PrimaryKey("PK_AspNetUserClaims", t => t.Id);
            
            migrationBuilder.CreateTable("AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ProviderDisplayName = c.String(),
                        UserId = c.String()
                    })
                .PrimaryKey("PK_AspNetUserLogins", t => new { t.LoginProvider, t.ProviderKey });
            
            migrationBuilder.CreateTable("AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(),
                        RoleId = c.String()
                    })
                .PrimaryKey("PK_AspNetUserRoles", t => new { t.UserId, t.RoleId });
            
            migrationBuilder.AddForeignKey("Attendees", "FK_Attendees_Conversations_ConversationId", new[] { "ConversationId" }, "Conversations", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("Messages", "FK_Messages_AspNetUsers_UserId", new[] { "UserId" }, "AspNetUsers", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("Messages", "FK_Messages_Conversations_ConversationId", new[] { "ConversationId" }, "Conversations", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("NotificationConnections", "FK_NotificationConnections_AspNetUsers_UserId", new[] { "UserId" }, "AspNetUsers", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("AspNetRoleClaims", "FK_AspNetRoleClaims_AspNetRoles_RoleId", new[] { "RoleId" }, "AspNetRoles", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("AspNetUserClaims", "FK_AspNetUserClaims_AspNetUsers_UserId", new[] { "UserId" }, "AspNetUsers", new[] { "Id" }, cascadeDelete: false);
            
            migrationBuilder.AddForeignKey("AspNetUserLogins", "FK_AspNetUserLogins_AspNetUsers_UserId", new[] { "UserId" }, "AspNetUsers", new[] { "Id" }, cascadeDelete: false);
        }
        
        public override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("Attendees", "FK_Attendees_Conversations_ConversationId");
            
            migrationBuilder.DropForeignKey("Messages", "FK_Messages_AspNetUsers_UserId");
            
            migrationBuilder.DropForeignKey("Messages", "FK_Messages_Conversations_ConversationId");
            
            migrationBuilder.DropForeignKey("NotificationConnections", "FK_NotificationConnections_AspNetUsers_UserId");
            
            migrationBuilder.DropForeignKey("AspNetRoleClaims", "FK_AspNetRoleClaims_AspNetRoles_RoleId");
            
            migrationBuilder.DropForeignKey("AspNetUserClaims", "FK_AspNetUserClaims_AspNetUsers_UserId");
            
            migrationBuilder.DropForeignKey("AspNetUserLogins", "FK_AspNetUserLogins_AspNetUsers_UserId");
            
            migrationBuilder.DropTable("Attendees");
            
            migrationBuilder.DropTable("AspNetUsers");
            
            migrationBuilder.DropTable("Conversations");
            
            migrationBuilder.DropTable("Messages");
            
            migrationBuilder.DropTable("NotificationConnections");
            
            migrationBuilder.DropTable("AspNetRoles");
            
            migrationBuilder.DropTable("AspNetRoleClaims");
            
            migrationBuilder.DropTable("AspNetUserClaims");
            
            migrationBuilder.DropTable("AspNetUserLogins");
            
            migrationBuilder.DropTable("AspNetUserRoles");
        }
    }
}