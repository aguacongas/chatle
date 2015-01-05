using DbContextTest;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using System;

namespace DbContextTest.Migrations
{
    [ContextType(typeof(DbTables))]
    public partial class initial : IMigrationMetadata
    {
        string IMigrationMetadata.MigrationId
        {
            get
            {
                return "201501051028251_initial";
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