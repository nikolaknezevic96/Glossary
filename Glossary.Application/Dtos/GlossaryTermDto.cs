using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glossary.Domain.Enums;

namespace Glossary.Application.Dtos;

    public sealed record GlossaryTermDto(
        Guid Id,
        string Term,
        string Definition,
        TermStatus Status,
        Guid CreatedByUserId,
        DateTimeOffset CreatedAt,
        DateTimeOffset? PublishedAt,
        DateTimeOffset? ArchivedAt
        );
    
        
    

