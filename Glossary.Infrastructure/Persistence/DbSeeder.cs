using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Domain.Entities;
using Glossary.Domain.Enums;
using Glossary.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Glossary.Infrastructure.Persistence
{
    public sealed class DbSeeder
    {
        private readonly GlossaryDbContext _db;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public DbSeeder(GlossaryDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync(CancellationToken ct)
        {
            if(!await _db.Users.AnyAsync(ct))
            {
                var user = new AppUser
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "author",
                };
                user.PasswordHash = _hasher.HashPassword(user, "password");

                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);
            }

            if(!await _db.GlossaryTerms.AnyAsync(ct))
            {
                var authorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var now = DateTimeOffset.UtcNow;

                var t1 = GlossaryTerm.Create(
                "abyssal plain",
                "The ocean floor offshore from the continental margin, usually very flat with a slight slope.",
                authorId,
                now);
                t1.Publish(now);

                var t2 = GlossaryTerm.Create(
                    "accrete",
                    "v. To add terranes (small land masses or pieces of crust) to another, usually larger, land mass.",
                    authorId,
                    now);
                t2.Publish(now);

                var t3 = GlossaryTerm.Create(
                    "alkaline",
                    "Term pertaining to a highly basic, as opposed to acidic, substance. For example, hydroxide or carbonate of sodium or potassium.",
                    authorId,
                    now);
                t3.Publish(now);

                _db.GlossaryTerms.AddRange(t1, t2, t3);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
