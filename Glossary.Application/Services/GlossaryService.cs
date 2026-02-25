using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Glossary.Application.Dtos;
using Glossary.Application.Exceptions;
using Glossary.Application.Interfaces;
using Glossary.Domain.Entities;

namespace Glossary.Application.Services
{
    public class GlossaryService : IGlossaryService
    {
        private readonly IGlossaryTermRepository _repo;

        public GlossaryService(IGlossaryTermRepository repo)
        {
            _repo = repo;
        }

        public async Task<GlossaryTermDto> CreateAsync(Guid userId, CreateTermRequest request, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;
            var term = GlossaryTerm.Create(request.Term, request.Definition, userId, now);

            await _repo.AddAsync(term, ct);
            await _repo.SaveChangesAsync(ct);
            return ToDto(term);
        }

        public async Task<GlossaryTermDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var term = await _repo.GetByIdAsync(id, ct);
            if (term is null)
                throw new NotFoundException($"Term '{id}' was not found.");
            return ToDto(term);
        }

        public async Task<IReadOnlyList<GlossaryTermDto>> GetPublishedAsync(CancellationToken ct)
        {
            var items = await _repo.ListPublishedAsync(ct);
            return items.OrderBy(x => x.Term, StringComparer.OrdinalIgnoreCase)
                .Select(ToDto)
                .ToList();
        }

        public async Task<IReadOnlyList<GlossaryTermDto>> GetAllAsync(CancellationToken ct)
        {
            var items = await _repo.ListAllAsync(ct);
            return items.OrderBy(x => x.Term, StringComparer.OrdinalIgnoreCase)
                .Select(ToDto)
                .ToList();
        }

        public async Task<GlossaryTermDto> UpdateDraftAsync(Guid userId, Guid id, UpdateTermRequest request, CancellationToken ct)
        {
            var term = await _repo.GetByIdAsync(id, ct);
            if (term is null)
                throw new NotFoundException($"Term '{id}' was not found.");

            EnsureCreator(userId, term);
            // Only drafts can be edited.
            term.UpdateDraft(request.Term, request.Definition);

            await _repo.SaveChangesAsync(ct);
            return ToDto(term);
        }

        public async Task PublishAsync(Guid userId, Guid id, DateTimeOffset now, CancellationToken ct)
        {
            var term = await _repo.GetByIdAsync(id, ct);
            if (term is null)
                throw new NotFoundException($"Term '{id} was not found.'");

            EnsureCreator(userId, term);
            term.Publish(now);

            await _repo.SaveChangesAsync(ct);
        }

        public async Task ArchiveAsync(Guid userId, Guid id, DateTimeOffset now, CancellationToken ct)
        {
            var term = await _repo.GetByIdAsync(id, ct);
            if (term is null)
                throw new NotFoundException($"Term '{id} was not found.'");

            EnsureCreator(userId, term);

            // Only published items can be archived.
            term.Archive(now);
            await _repo.SaveChangesAsync(ct);
        }

        public async Task DeleteDraftAsync(Guid userId, Guid id, CancellationToken ct)
        {
            var term = await _repo.GetByIdAsync(id, ct);
            if(term is null)
                throw new NotFoundException($"Term '{id}' was not found.");

            EnsureCreator(userId, term);

            if (term.Status != Domain.Enums.TermStatus.Draft)
                throw new ForbiddenException("Only draft terms can be deleted.");

            _repo.Remove(term);
            await _repo.SaveChangesAsync(ct);
        }

        private static void EnsureCreator(Guid userId, GlossaryTerm term)
        {
            if (term.CreatedByUserId != userId)
                throw new ForbiddenException("The user can only modify terms that they created.");
        }

        private static GlossaryTermDto ToDto(GlossaryTerm x) =>
            new(
                x.Id,
                x.Term,
                x.Definition,
                x.Status,
                x.CreatedByUserId,
                x.CreatedAt,
                x.PublishedAt,
                x.ArchivedAt
                );
    }
}
