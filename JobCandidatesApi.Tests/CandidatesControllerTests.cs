using JobCandidatesApi.Controllers;
using JobCandidatesApi.Data;
using JobCandidatesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace JobCandidatesApi.Tests;

public class CandidatesControllerTests
{
    private readonly CandidatesController _controller;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public CandidatesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);

        _cache = new MemoryCache(new MemoryCacheOptions());

        _controller = new CandidatesController(_context, _cache);
    }

    [Fact]
    public async Task CreateOrUpdateCandidate_ValidCandidate_ReturnsOk()
    {
        // Arrange
        var candidate = new Candidate
        {
            FirstName = "Test",
            LastName = "Tset",
            Email = "test@test.com",
            PhoneNumber = "1234567890",
            CallTimeInterval = "9:00-17:00",
            LinkedInProfileUrl = "https://www.linkedin.com/somelink",
            GitHubProfileUrl = "https://github.com/somelink",
            FreeTextComment = "Test comment"
        };

        // Act
        var result = await _controller.CreateOrUpdateCandidate(candidate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCandidate = Assert.IsType<Candidate>(okResult.Value);

        Assert.Equal(candidate.Email, returnedCandidate.Email);
        Assert.Equal(candidate.FirstName, returnedCandidate.FirstName);
    }

    [Fact]
    public async Task GetCandidateByEmail_ExistingCandidateInCache_ReturnsOk()
    {
        // Arrange
        var candidate = new Candidate
        {
            FirstName = "Test2",
            LastName = "Tset2",
            Email = "test2@test.com",
            PhoneNumber = "0987654321",
            CallTimeInterval = "10:00-16:00",
            LinkedInProfileUrl = "https://www.linkedin.com/somelink2",
            GitHubProfileUrl = "https://github.com/somelink2",
            FreeTextComment = "Test 2 comment"
        };

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetCandidateByEmail(candidate.Email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCandidate = Assert.IsType<Candidate>(okResult.Value);

        Assert.Equal(candidate.Email, returnedCandidate.Email);
        Assert.Equal(candidate.FirstName, returnedCandidate.FirstName);
    }

    [Fact]
    public async Task CreateOrUpdateCandidates_ValidCandidates_ReturnsOk()
    {
        // Arrange
        var candidates = new List<Candidate>
            {
                new Candidate
                {
                    FirstName = "Test3",
                    LastName = "Test3",
                    Email = "test3@test.com",
                    PhoneNumber = "1231231234",
                    CallTimeInterval = "10:00-18:00",
                    LinkedInProfileUrl = "https://www.linkedin.com/somelink3",
                    GitHubProfileUrl = "https://github.com/somelink3",
                    FreeTextComment = "Test Comment 3"
                },
                new Candidate
                {
                    FirstName = "Test4",
                    LastName = "Test44",
                    Email = "test4@test.com",
                    PhoneNumber = "4321432143",
                    CallTimeInterval = "9:00-16:00",
                    LinkedInProfileUrl = "https://www.linkedin.com/somelink4",
                    GitHubProfileUrl = "https://github.com/somelink44",
                    FreeTextComment = "Great comment 4"
                }
            };

        // Act
        var result = await _controller.CreateOrUpdateCandidates(candidates);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCandidates = Assert.IsType<List<Candidate>>(okResult.Value);

        Assert.Equal(2, returnedCandidates.Count);
        Assert.Equal(candidates[0].Email, returnedCandidates[0].Email);
        Assert.Equal(candidates[1].Email, returnedCandidates[1].Email);
    }
}
