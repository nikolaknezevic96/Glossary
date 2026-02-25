using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Domain.Entities;

namespace Glossary.Application.Interfaces
{
    public interface IGlossaryTermRepository
    {
        Task AddAsync(GlossaryTerm term, CancellationToken ct);
        Task<GlossaryTerm?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<List<GlossaryTerm>> ListAllAsync(CancellationToken ct);
        Task<List<GlossaryTerm>> ListPublishedAsync(CancellationToken ct);

        void Remove(GlossaryTerm term);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
