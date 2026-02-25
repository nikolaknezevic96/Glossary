using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Domain.Entities;
using Glossary.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Glossary.Infrastructure.Persistence
{
    public sealed class GlossaryTermConfig : IEntityTypeConfiguration<GlossaryTerm>
    {
        public void Configure(EntityTypeBuilder<GlossaryTerm> b)
        {
            b.ToTable("GlossaryTerms");
            b.HasKey(x => x.Id);
            b.Property(x=>x.Term).HasMaxLength(200).IsRequired();
            b.Property(x => x.Definition).HasMaxLength(4000).IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();

            b.Property(x => x.CreatedByUserId).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.PublishedAt);
            b.Property(x => x.ArchivedAt);
            // index for alphabetical listing
            b.HasIndex(x => x.Term);

            var authorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var t1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var t2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var t3Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

            b.HasData(
                new
                {
                    Id = t1Id,
                    Term = "abyssal plain",
                    Definition = "The ocean floor offshore from the continental margin, usually very flat with a slight slope.",
                    Status = TermStatus.Published,
                    CreatedByUserId = authorId,
                    CreatedAt = new DateTimeOffset(2026, 01, 01, 12, 0, 0, TimeSpan.Zero),
                    PublishedAt = new DateTimeOffset(2026, 01, 02, 12, 0, 0, TimeSpan.Zero),
                    ArchivedAt = (DateTimeOffset?)null
                },
                new
                {
                    Id = t2Id,
                    Term = "accrete",
                    Definition = "v. To add terranes (small land masses or pieces of crust) to another, usually larger, land mass.",
                    Status = TermStatus.Published,
                    CreatedByUserId = authorId,
                    CreatedAt = new DateTimeOffset(2026, 01, 01, 12, 0, 0, TimeSpan.Zero),
                    PublishedAt = new DateTimeOffset(2026, 01, 03, 12, 0, 0, TimeSpan.Zero),
                    ArchivedAt = (DateTimeOffset?)null
                },
                new
                {
                    Id = t3Id,
                    Term = "alkaline",
                    Definition = "Term pertaining to a highly basic, as opposed to acidic, substance. For example, hydroxide or carbonate of sodium or potassium.",
                    Status = TermStatus.Published,
                    CreatedByUserId = authorId,
                    CreatedAt = new DateTimeOffset(2026, 01, 04, 12, 0, 0, TimeSpan.Zero),
                    PublishedAt = new DateTimeOffset(2026, 01, 05, 12, 0, 0, TimeSpan.Zero),
                    ArchivedAt = (DateTimeOffset?)null
                });
        }
    }
}
