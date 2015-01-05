using DbContextTest;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using System;

namespace DbContextTest.Migrations
{
    [ContextType(typeof(DbTables))]
    public class DbTablesModelSnapshot : ModelSnapshot
    {
        public override IModel Model
        {
            get
            {
                var builder = new BasicModelBuilder();
                
                builder.Entity("DbContextTest.Row", b =>
                    {
                        b.Property<string>("Id");
                        b.Key("Id");
                    });
                
                return builder.Model;
            }
        }
    }
}