using Glossary.Domain.Enums;
using Glossary.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.Domain.Entities
{
    public sealed class GlossaryTerm
    {
        private static readonly string[] ForbiddenWords = ["lorem", "test", "sample"];

        private GlossaryTerm() { }

        public Guid Id { get; private set; }
        public string Term { get; private set; } = string.Empty;
        public string Definition { get; private set; } = string.Empty;
        public TermStatus Status { get; private set; }
        public Guid CreatedByUserId { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? PublishedAt { get; private set; }
        public DateTimeOffset? ArchivedAt { get; private set; }

        public static GlossaryTerm Create(string term, string definition, Guid createdByUserId, DateTimeOffset now)
        {
            var entity = new GlossaryTerm
            {
                Id = Guid.NewGuid(),
                Term = (term ?? string.Empty).Trim(),
                Definition = (definition ?? string.Empty).Trim(),
                Status = TermStatus.Draft,
                CreatedByUserId = createdByUserId,
                CreatedAt = now
            };

            return entity;
        }

        public void UpdateDraft(string term, string definition)
        {
            EnsureStatus(TermStatus.Draft, "Only draft terms can be edited.");

            Term = (term ?? string.Empty).Trim();
            Definition = (definition ?? string.Empty).Trim();
        }

        public void Publish(DateTimeOffset now)
        {
            EnsureStatus(TermStatus.Draft, "Only draft items can be published.");

            // Term must not be empty.
            // Definition length >= 30
            // Definition does not contain forbidden words.
            if (string.IsNullOrWhiteSpace(Term))
                throw new DomainRuleViolationException("Term must not be empty.");

            if(string.IsNullOrWhiteSpace(Definition) || Definition.Length < 30)
                throw new DomainRuleViolationException("Definition must not be empty and it must be at least 30 characters long.");

            if(ContainsForbiddenWord(Definition))
                throw new DomainRuleViolationException("Definition contains forbidden words!");

            Status = TermStatus.Published;
            PublishedAt = now;
        }
        
        public void Archive(DateTimeOffset now)
        {
            EnsureStatus(TermStatus.Published, "Only published terms can be archived.");

            Status = TermStatus.Archived;
            ArchivedAt = now;
        }

        private void EnsureStatus(TermStatus required, string message)
        {
            if (Status != required)
                throw new DomainRuleViolationException(message);
        }

        private static bool ContainsForbiddenWord(string text)
        {
            // Case-insensitive check
            var lower = text.ToLowerInvariant();
            foreach(var word in ForbiddenWords)
            {
                if (lower.Contains(word))
                    return true;
            }
            return false;
        }
    }
}
