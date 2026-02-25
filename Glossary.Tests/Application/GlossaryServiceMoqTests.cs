using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Glossary.Application.Exceptions;
using Glossary.Application.Interfaces;
using Glossary.Application.Services;
using Glossary.Domain.Entities;
using Glossary.Domain.Enums;
using Glossary.Domain.Exceptions;
using Moq;
using Xunit;

namespace Glossary.Tests.Application;

public sealed class GlossaryServiceMoqTests
{
    [Fact]
    public async Task DeleteDraft_ShouldThrowForbidden_WhenNotCreator()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var creatorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var term = GlossaryTerm.Create("term", ValidDefinition(), creatorId, now);

        repo.Setup(r => r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);

        var act = async () => await service.DeleteDraftAsync(otherUserId, term.Id, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.VerifyAll();

    }

    [Fact]
    public async Task DeleteDraft_ShouldThrowForbidden_WhenNotDraft()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var creatorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var term = GlossaryTerm.Create("term", ValidDefinition(), creatorId, now);
        term.Publish(now);

        repo.Setup(r => r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);

        var act = async () => await service.DeleteDraftAsync(creatorId, term.Id, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>().WithMessage("*Only draft terms can be deleted.*");

        repo.Verify(r => r.Remove(It.IsAny<GlossaryTerm>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.VerifyAll();
    }

    [Fact]
    public async Task DeleteDraft_ShouldRemoveAndSave_WhenCreatorAndDraft()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);
        var creatorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var term = GlossaryTerm.Create("term", ValidDefinition(), creatorId, now);
        repo.Setup(r=> r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);
        repo.Setup(r => r.Remove(term));
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await service.DeleteDraftAsync(creatorId, term.Id, CancellationToken.None);

        repo.Verify(r => r.Remove(term), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyAll();
    }

    [Fact]
    public async Task Publish_ShouldThrowNotFound_WhenTermMissing()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((GlossaryTerm?)null);

        var act = async () => await service.PublishAsync(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.VerifyAll();
    }

    [Fact]
    public async Task Publish_ShouldThrowForbidden_WhenNotCreator()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var creatorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var term = GlossaryTerm.Create("term", ValidDefinition(), creatorId, now);
        repo.Setup(r=> r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);

        var act = async () => await service.PublishAsync(otherUserId, term.Id, now, CancellationToken.None);
        await act.Should().ThrowAsync<ForbiddenException>();

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.VerifyAll();
    }

    [Fact]
    public async Task Publish_ShouldSave_WhenValidAndCreator()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var creatorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var term = GlossaryTerm.Create("term", ValidDefinition(), creatorId, now);

        repo.Setup(r => r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await service.PublishAsync(creatorId, term.Id, now, CancellationToken.None);
        term.Status.Should().Be(TermStatus.Published);

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyAll();
    }

    [Fact]
    public async Task GetPublished_ShouldReturnAlphabetticallySorted()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var userId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var bterm = GlossaryTerm.Create("bterm", ValidDefinition(), userId, now);
        var aterm = GlossaryTerm.Create("aterm", ValidDefinition(), userId, now);
        var cterm = GlossaryTerm.Create("cterm", ValidDefinition(), userId, now);

        repo.Setup(r=> r.ListPublishedAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<GlossaryTerm> { bterm, aterm, cterm });
        var result = await service.GetPublishedAsync(CancellationToken.None);
        result.Select(x => x.Term).Should().ContainInOrder("aterm", "bterm", "cterm");

        repo.VerifyAll();
    }

    [Fact]
    public async Task UpdateDraft_ShouldThromDomainRuleViolation_WhenNotDraft()
    {
        var repo = new Mock<IGlossaryTermRepository>(MockBehavior.Strict);
        var service = new GlossaryService(repo.Object);

        var userId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var term = GlossaryTerm.Create("term", ValidDefinition(), userId, now);
        term.Publish(now);

        repo.Setup(r=> r.GetByIdAsync(term.Id, It.IsAny<CancellationToken>())).ReturnsAsync(term);

        var act = async () => await service.UpdateDraftAsync(userId, term.Id, new Glossary.Application.Dtos.UpdateTermRequest("new term", ValidDefinition()), CancellationToken.None);

        await act.Should().ThrowAsync<DomainRuleViolationException>();

        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        repo.VerifyAll();
    }

    public static string ValidDefinition() =>
    "This is a valid definition with more than 30 characters.";
    
}
