using JobCandidatesApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace JobCandidatesApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }
}
