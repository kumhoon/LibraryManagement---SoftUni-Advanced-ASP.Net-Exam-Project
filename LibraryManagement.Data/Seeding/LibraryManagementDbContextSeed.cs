namespace LibraryManagement.Data.Seeding
{
    using LibraryManagement.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class LibraryManagementDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

            // Seed roles
            string[] roles = ["Admin", "User"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed users
            var adminUser = new IdentityUser { UserName = "admin@library.com", Email = "admin@library.com", EmailConfirmed = true };
            var user1 = new IdentityUser { UserName = "user1@library.com", Email = "user1@library.com", EmailConfirmed = true };
            var user2 = new IdentityUser { UserName = "user2@library.com", Email = "user2@library.com", EmailConfirmed = true };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (await userManager.FindByEmailAsync(user1.Email) == null)
            {
                await userManager.CreateAsync(user1, "User123!");
                await userManager.AddToRoleAsync(user1, "User");
            }

            if (await userManager.FindByEmailAsync(user2.Email) == null)
            {
                await userManager.CreateAsync(user2, "User123!");
                await userManager.AddToRoleAsync(user2, "User");
            }

            // Seed genres 
            if (!context.Genres.Any())
            {
                context.Genres.AddRange(
                    new Genre { Id = Guid.NewGuid(), Name = "Science Fiction" },
                    new Genre { Id = Guid.NewGuid(), Name = "Fantasy" },
                    new Genre { Id = Guid.NewGuid(), Name = "Mystery" },
                    new Genre { Id = Guid.NewGuid(), Name = "Biography" }              
                );

                await context.SaveChangesAsync(); 
            }

            // Seed authors
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    new Author { Id = Guid.NewGuid(), Name = "Isaac Asimov", IsDeleted = false },
                    new Author { Id = Guid.NewGuid(), Name = "J.R.R. Tolkien", IsDeleted = false }
                );
                await context.SaveChangesAsync();
            }

            // Seed books
            if (!context.Books.Any())
            {
                var adminDbUser = await userManager.FindByEmailAsync(adminUser.Email)
                    ?? throw new InvalidOperationException($"Admin user with email '{adminUser.Email}' was not found.");

                var asimov = context.Authors.First(a => a.Name == "Isaac Asimov");
                var tolkien = context.Authors.First(a => a.Name == "J.R.R. Tolkien");

                var sciFi = context.Genres.First(g => g.Name == "Science Fiction");
                var fantasy = context.Genres.First(g => g.Name == "Fantasy");

                context.Books.AddRange(
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "Foundation",
                        Description = "A visionary tale of the fall and rise of civilizations.",
                        AuthorId = asimov.Id,
                        GenreId = sciFi.Id,
                        PublishedDate = new DateTime(1951, 1, 1),
                        BookCreatorId = adminDbUser.Id
                    },
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "I, Robot",
                        Description = "A collection of short stories exploring robotics and ethics.",
                        AuthorId = asimov.Id,
                        GenreId = sciFi.Id,
                        PublishedDate = new DateTime(1950, 1, 1),
                        BookCreatorId = adminDbUser.Id
                    },
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "The Hobbit",
                        Description = "A fantasy adventure featuring Bilbo Baggins and a dragon.",
                        AuthorId = tolkien.Id,
                        GenreId = fantasy.Id,
                        PublishedDate = new DateTime(1937, 9, 21),
                        BookCreatorId = adminDbUser.Id
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed members
            if (!context.Memberships.Any())
            {
                var userDb1 = await userManager.FindByEmailAsync(user1.Email);
                var userDb2 = await userManager.FindByEmailAsync(user2.Email);
                if (userDb1 == null || userDb2 == null)
                {
                    throw new InvalidOperationException($"User was not found.");
                }
                context.Memberships.AddRange(
                    new Member
                    {
                        Id = Guid.NewGuid(),
                        Name = "User One",
                        JoinDate = DateTime.UtcNow,
                        UserId = userDb1.Id,
                        Status = MembershipStatus.Approved
                    },
                    new Member
                    {
                        Id = Guid.NewGuid(),
                        Name = "User Two",
                        JoinDate = DateTime.UtcNow,
                        UserId = userDb2.Id,
                        Status = MembershipStatus.Approved
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
