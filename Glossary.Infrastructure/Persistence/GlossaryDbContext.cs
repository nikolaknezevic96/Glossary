using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Domain.Entities;
using Glossary.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace Glossary.Infrastructure.Persistence
{
    public sealed class GlossaryDbContext : DbContext
    {
        public GlossaryDbContext(DbContextOptions<GlossaryDbContext> options) : base(options) { }

        public DbSet<GlossaryTerm> GlossaryTerms => Set<GlossaryTerm>();
        public DbSet<AppUser> Users => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GlossaryDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        
        
        
        
    }
}
