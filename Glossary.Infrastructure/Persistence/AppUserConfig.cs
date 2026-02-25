using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Glossary.Infrastructure.Persistence
{
    public sealed class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> b)
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);
            b.Property(x=> x.Username).HasMaxLength(100).IsRequired();
            b.HasIndex(x=> x.Username).IsUnique();
            b.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();

            
        }
    }
}
