﻿using System.ComponentModel.DataAnnotations;

namespace JobCandidatesApi.Models;

public class Candidate
{
    [Key]
    [Required]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string CallTimeInterval { get; set; }

    public string LinkedInProfileUrl { get; set; }

    public string GitHubProfileUrl { get; set; }

    [Required]
    public string FreeTextComment { get; set; }
}
