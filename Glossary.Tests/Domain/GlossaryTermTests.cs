using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Glossary.Domain.Entities;
using Glossary.Domain.Enums;
using Glossary.Domain.Exceptions;
using Xunit;

namespace Glossary.Tests.Domain;

public sealed class GlossaryTermTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    [Fact]
    public void Create_ShouldCreateDraft()
    {
        var term = GlossaryTerm.Create("abc term", "a definition of the term that is longer than 30 characters.", UserId, Now);

        term.Status.Should().Be(TermStatus.Draft);
        term.CreatedByUserId.Should().Be(UserId);
        term.CreatedAt.Should().Be(Now);
    }

    [Fact]
    public void Publish_ShouldFail_WhenTermIsEmpty()
    {
        var term = GlossaryTerm.Create("", ValidDefinition(), UserId, Now);
        var act = () => term.Publish(Now);
        act.Should().Throw<DomainRuleViolationException>().WithMessage("*Term must not be empty.*");
    }

    [Fact]
    public void Publish_ShouldFail_WhenDefinitionIsTooShort()
    {
        var term = GlossaryTerm.Create("term", "too short", UserId, Now);
        var act = () => term.Publish(Now);
        act.Should().Throw<DomainRuleViolationException>().WithMessage("*Definition must not be empty and it must be at least 30 characters long.*");
    }

    [Theory]
    [InlineData("this definition contains lorem and is longer than thirty characters.")]
    [InlineData("this definition contains TEST and is longer than thirty characters.")]
    [InlineData("this definition contains SamPle and is longer than thirty characters.")]
    public void Publish_ShouldFail_WhenDefinitionContainsForbiddenWords(string definition)
    {
        var term = GlossaryTerm.Create("term", definition, UserId, Now);
        var act = () => term.Publish(Now);
        act.Should().Throw<DomainRuleViolationException>().WithMessage("*Definition contains forbidden words!*");
    }

    [Fact]
    public void Publish_ShouldSucceed_WhenValid()
    {
        var term = GlossaryTerm.Create("term", ValidDefinition(), UserId, Now);
        term.Publish(Now);
        term.Status.Should().Be(TermStatus.Published);
        term.PublishedAt.Should().Be(Now);
    }

    [Fact]
    public void Archive_ShouldFail_WhenNotPublished()
    {
        var term = GlossaryTerm.Create("term", ValidDefinition(), UserId, Now);
        var act = () => term.Archive(Now);
        act.Should().Throw<DomainRuleViolationException>().WithMessage("*Only published terms can be archived.*");
    }

    [Fact]
    public void Archive_ShouldSucceed_WhenPublished()
    {
        var term = GlossaryTerm.Create("term", ValidDefinition(), UserId, Now);
        term.Publish(Now);
        term.Archive(Now);

        term.Status.Should().Be(TermStatus.Archived);
        term.ArchivedAt.Should().Be(Now);
    }

    private static string ValidDefinition() =>
        "a valid definition that is longer than thirty characters.";


}
