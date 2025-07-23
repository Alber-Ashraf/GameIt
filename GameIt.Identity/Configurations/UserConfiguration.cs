using GameIt.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(
             new ApplicationUser
             {
                 Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                 Email = "user@gameit.com",
                 NormalizedEmail = "USER@GAMEIT.COM",
                 FirstName = "System",
                 LastName = "User",
                 UserName = "user@gameit.com",
                 NormalizedUserName = "USER@GAMEIT.COM",
                 PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                 EmailConfirmed = true
             },
             new ApplicationUser
             {
                    Id = "7e224968-33e4-4652-b7b7-8574d048cdb9",
                    Email = "employee@gameit.com",
                    NormalizedEmail = "EMPLOYEE@GAMEIT.COM",
                    FirstName = "System",
                    LastName = "Employee",
                    UserName = "employee@gameit.com",
                    NormalizedUserName = "EMPLOYEE@GAMEIT.COM",
                    PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                    EmailConfirmed = true
             },
             new ApplicationUser
             {
                 Id = "7e445865-a24d-4543-b6c6-9443d048cdb9",
                 Email = "admin@gameit.com",
                 NormalizedEmail = "ADMIN@GAMEIT.COM",
                 FirstName = "System",
                 LastName = "Admin",
                 UserName = "admin@gameit.com",
                 NormalizedUserName = "ADMIN@GAMEIT.COM",
                 PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                 EmailConfirmed = true
             });
    }
}
