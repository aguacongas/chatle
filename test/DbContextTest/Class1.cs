using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System;

namespace DbContextTest
{
    public class Row
    {
        public string Id { get; set; }
    }
    public class DbTables : DbContext
    {

        public DbSet<Row> Rows { get; set; }

        private static bool _created = false;

        public DbTables()
        {
            if (_created)
            {
                Database.AsRelational().ApplyMigrations();
                _created = true;
            }
        }

        protected override void OnConfiguring(DbContextOptions options)
        {
            options.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=app_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
