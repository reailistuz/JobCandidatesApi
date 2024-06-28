using System;
using System.Threading.Tasks;
using JobCandidatesApi.Controllers;
using JobCandidatesApi.Data;
using JobCandidatesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace JobCandidatesApi.Tests
{
    public class CandidatesControllerTests
    {
        private CandidatesController _controller;
        private Mock<ApplicationDbContext> _contextMock;
        private Mock<IMemoryCache> _cacheMock;

        public CandidatesControllerTests()
        {
            // Mock DbContext
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            // Mock MemoryCache
            _cacheMock = new Mock<IMemoryCache>();

            _controller = new CandidatesController(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task CreateOrUpdateCandidate_ValidCandidate_ReturnsOk()
        {
            // Arrange
            var candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                FreeTextComment = "Test comment"
                // Add other required properties as needed
            };

            _contextMock.Setup(x => x.Candidates.FindAsync(candidate.Email))
                        .ReturnsAsync((Candidate)null); // Mocking that candidate doesn't exist

            // Act
            var result = await _controller.CreateOrUpdateCandidate(candidate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdCandidate = Assert.IsType<Candidate>(okResult.Value);

            Assert.Equal(candidate.Email, createdCandidate.Email);
        }

        [Fact]
        public async Task GetCandidateByEmail_ExistingCandidateInCache_ReturnsOk()
        {
            // Arrange
            var email = "john.doe@example.com";
            var existingCandidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                FreeTextComment = "Test comment"
                // Add other required properties as needed
            };

            _cacheMock.Setup(x => x.TryGetValue(email, out existingCandidate))
                      .Returns(true); // Mocking that candidate exists in cache

            // Act
            var result = await _controller.GetCandidateByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCandidate = Assert.IsType<Candidate>(okResult.Value);

            Assert.Equal(existingCandidate.Email, returnedCandidate.Email);
        }

        //[Fact]
        //public async Task GetCandidateByEmail_NonExistingCandidate_ReturnsNotFound()
        //{
        //    // Arrange
        //    var email = "nonexistent@example.com";

        //    _cacheMock.Setup(x => x.TryGetValue(email, out It.IsAny<Candidate>()))
        //              .Returns(false);

        //    _contextMock.Setup(x => x.Candidates.FirstOrDefaultAsync(c => c.Email == email))
        //                .ReturnsAsync((Candidate)null);

        //    // Act
        //    var result = await _controller.GetCandidateByEmail(email);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}
    }
}
