using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebApplication4.Models.Entities
{
    public class SortingFinderContext:DbContext
    {
        public SortingFinderContext() : base("SortingFinder") { }

        public DbSet<Word> Words { get; set; }

        public DbSet<Product> Products { get; set; }       

        public DbSet<WordControl> WordControls { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

    }
}