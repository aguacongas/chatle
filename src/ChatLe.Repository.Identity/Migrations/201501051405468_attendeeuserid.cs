using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Model;
using System;

namespace ChatLe.Repository.Identity.Migrations
{
    public partial class attendeeuserid : Migration
    {
        public override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey("Attendees", "FK_Attendees_AspNetUsers_UserId", new[] { "UserId" }, "AspNetUsers", new[] { "Id" }, cascadeDelete: false);
        }
        
        public override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("Attendees", "FK_Attendees_AspNetUsers_UserId");
        }
    }
}