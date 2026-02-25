using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Application.Dtos;

namespace Glossary.Application.Interfaces
{
    public interface IGlossaryService
    {
        Task<GlossaryTermDto> CreateAsync(Guid userId, CreateTermRequest request, CancellationToken ct);
        Task<GlossaryTermDto> GetByIdAsync(Guid id, CancellationToken ct);

        Task<IReadOnlyList<GlossaryTermDto>> GetPublishedAsync(CancellationToken ct);

        Task<IReadOnlyList<GlossaryTermDto>> GetAllAsync(CancellationToken ct);

        Task<GlossaryTermDto> UpdateDraftAsync(Guid userId, Guid id, UpdateTermRequest request, CancellationToken ct);

        Task PublishAsync(Guid userId, Guid id, DateTimeOffset now, CancellationToken ct);

        Task ArchiveAsync(Guid userId, Guid id, DateTimeOffset now, CancellationToken ct);

        Task DeleteDraftAsync(Guid userId, Guid id, CancellationToken ct);
    }
}
