using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Application.Interfaces;
using Glossary.Domain.Entities;
using Glossary.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Glossary.Infrastructure.Persistence
{
    public sealed class GlossaryTermRepository : IGlossaryTermRepository
    {
        private readonly GlossaryDbContext _db;
        public GlossaryTermRepository(GlossaryDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(GlossaryTerm term, CancellationToken ct) =>
            _db.GlossaryTerms.AddAsync(term, ct).AsTask();

        public Task<GlossaryTerm?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.GlossaryTerms.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<List<GlossaryTerm>> ListAllAsync(CancellationToken ct) =>
            _db.GlossaryTerms.ToListAsync(ct);

        public Task<List<GlossaryTerm>> ListPublishedAsync(CancellationToken ct) =>
            _db.GlossaryTerms.AsNoTracking()
            .Where(x => x.Status == TermStatus.Published).ToListAsync(ct);

        public void Remove(GlossaryTerm term) => _db.GlossaryTerms.Remove(term);

        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
