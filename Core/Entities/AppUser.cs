using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // related prop
    public Address? Address { get; set; } // not gonna ask for address when a user first sign up, but ask when checkout and 
    // store for when they come back
}
