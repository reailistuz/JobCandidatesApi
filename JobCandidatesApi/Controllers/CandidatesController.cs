using JobCandidatesApi.Data;
using JobCandidatesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JobCandidatesApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public CandidatesController(
        ApplicationDbContext context,
        IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateCandidate([FromBody] Candidate candidate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCandidate = await _context.Candidates.FindAsync(candidate.Email);
        if (existingCandidate == null)
        {
            _context.Candidates.Add(candidate);
        }
        else
        {
            existingCandidate.FirstName = candidate.FirstName;
            existingCandidate.LastName = candidate.LastName;
            existingCandidate.PhoneNumber = candidate.PhoneNumber;
            existingCandidate.CallTimeInterval = candidate.CallTimeInterval;
            existingCandidate.LinkedInProfileUrl = candidate.LinkedInProfileUrl;
            existingCandidate.GitHubProfileUrl = candidate.GitHubProfileUrl;
            existingCandidate.FreeTextComment = candidate.FreeTextComment;
            _context.Candidates.Update(existingCandidate);
        }

        await _context.SaveChangesAsync();
        return Ok(candidate);
    }
    [HttpGet("{email}")]
    public async Task<IActionResult> GetCandidateByEmail(string email)
    {
        if (!_cache.TryGetValue(email, out Candidate candidate))
        {
            candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == email);

            if (candidate != null)
            {
                _cache.Set(email, candidate, TimeSpan.FromMinutes(5)); // cache expiration time
            }
        }

        if (candidate == null)
        {
            return NotFound();
        }

        return Ok(candidate);
    }
}

