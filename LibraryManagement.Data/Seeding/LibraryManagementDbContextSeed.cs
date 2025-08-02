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
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

            // 1) Roles 
            string[] roles = { "Admin" };
            foreach (var role in roles)
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole(role));

            // 2) Users
            await EnsureUser(userMgr, "admin@library.com", "Admin123!", addToRole: "Admin");
            await EnsureUser(userMgr, "user1@library.com", "User123!");
            await EnsureUser(userMgr, "user2@library.com", "User123!");

            // 3) Genres
            var genreNames = new[] { "Science Fiction", "Fantasy", "Mystery", "Biography" };
            foreach (var name in genreNames)
            {
                if (!context.Genres.Any(g => g.Name == name))
                    context.Genres.Add(new Genre { Id = Guid.NewGuid(), Name = name });
            }
            await context.SaveChangesAsync();

            // 4) Authors
            var authorNames = new[] { "Isaac Asimov", "J.R.R. Tolkien" };
            foreach (var name in authorNames)
            {
                if (!context.Authors.Any(a => a.Name == name))
                    context.Authors.Add(new Author { Id = Guid.NewGuid(), Name = name, IsDeleted = false });
            }
            await context.SaveChangesAsync();

            // 5) Books 
            var adminUser = await userMgr.FindByEmailAsync("admin@library.com")
                             ?? throw new InvalidOperationException("Admin missing");
            var booksToAdd = new[]
            {
                new { Title="Foundation", Author="Isaac Asimov", Genre="Science Fiction", Pub=new DateTime(1951,1,1) },
                new { Title="I, Robot", Author="Isaac Asimov", Genre="Science Fiction", Pub=new DateTime(1950,1,1) },
                new { Title="The Hobbit", Author="J.R.R. Tolkien", Genre="Fantasy", Pub=new DateTime(1937,9,21) }
            };
            foreach (var b in booksToAdd)
            {
                if (!context.Books.Any(x => x.Title == b.Title && x.Author.Name == b.Author))
                {
                    var author = context.Authors.First(a => a.Name == b.Author);
                    var genre = context.Genres.First(g => g.Name == b.Genre);
                    context.Books.Add(new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = b.Title,
                        Description = $"Seeded: {b.Title}",
                        AuthorId = author.Id,
                        GenreId = genre.Id,
                        PublishedDate = b.Pub,
                        BookCreatorId = adminUser.Id
                    });
                }
            }
            await context.SaveChangesAsync();

            // 6) Members
            var memberSeeds = new[]
            {
                new { Email="user1@library.com", Name="User One" },
                new { Email="user2@library.com", Name="User Two" }
            };
            foreach (var m in memberSeeds)
            {
                var user = await userMgr.FindByEmailAsync(m.Email)
                           ?? throw new InvalidOperationException($"User {m.Email} not found");

                if (!context.Memberships.Any(ms => ms.UserId == user.Id))
                {
                    context.Memberships.Add(new Member
                    {
                        Id = Guid.NewGuid(),
                        Name = m.Name,
                        JoinDate = DateTime.UtcNow,
                        UserId = user.Id,
                        Status = MembershipStatus.Approved
                    });
                }
            }
            // 7) Reviews
            var reviewSeeds = new[]
            {
                new {
                    MemberEmail = "user1@library.com",
                    BookTitle   = "I, Robot",
                    Rating      = 5,
                    Content     = "Absolutely loved the scope and vision!"
                },
                new {
                    MemberEmail = "user2@library.com",
                    BookTitle   = "Foundation",
                    Rating      = 4,
                    Content     = "Great adventure, though a bit slow at times."
                }
            };

            foreach (var r in reviewSeeds)
            {

                var identityUser = await userMgr.FindByEmailAsync(r.MemberEmail);
                var member = identityUser == null
                    ? null
                    : context.Memberships.FirstOrDefault(m => m.UserId == identityUser.Id);

                var book = context.Books.FirstOrDefault(b => b.Title == r.BookTitle);

                if (member != null && book != null)
                {
                    bool alreadyReviewed = context.Reviews
                        .Any(x => x.MemberId == member.Id && x.BookId == book.Id);

                    if (!alreadyReviewed)
                    {
                        context.Reviews.Add(new Review
                        {
                            Id = Guid.NewGuid(),
                            MemberId = member.Id,
                            BookId = book.Id,
                            Rating = r.Rating,
                            Content = r.Content,
                            CreatedAt = DateTime.UtcNow,
                            IsApproved = true
                        });
                    }
                }
            }

            await context.SaveChangesAsync();


            // 8) Seed borrowing records
            var borrowSeeds = new[]
            {           
                new
                {
                    MemberEmail = "user1@library.com",
                    BookTitle   = "I, Robot",
                    BorrowDate  = DateTime.UtcNow.AddDays(-7),
                    ReturnDate  = (DateTime?)null
                },
                new
                {
                    MemberEmail = "user2@library.com",
                    BookTitle   = "Foundation",
                    BorrowDate  = DateTime.UtcNow.AddDays(-30),
                    ReturnDate  = (DateTime?)DateTime.UtcNow.AddDays(-15)
                }
            };

            foreach (var b in borrowSeeds)
            {
                var member = await userMgr.FindByEmailAsync(b.MemberEmail) is IdentityUser u
                    ? context.Memberships.FirstOrDefault(m => m.UserId == u.Id)
                    : null;
                var book = context.Books.FirstOrDefault(bk => bk.Title == b.BookTitle);

                if (member != null && book != null)
                {
                    bool exists = context.BorrowingRecords.Any(x =>
                        x.MemberId == member.Id
                     && x.BookId == book.Id
                     && x.BorrowDate == b.BorrowDate);

                    if (!exists)
                    {
                        context.BorrowingRecords.Add(new BorrowingRecord
                        {
                            Id = Guid.NewGuid(),
                            MemberId = member.Id,
                            BookId = book.Id,
                            BorrowDate = b.BorrowDate,
                            ReturnDate = b.ReturnDate
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task EnsureUser(UserManager<IdentityUser> um, string email, string pw, string? addToRole = null)
        {
            var user = await um.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await um.CreateAsync(user, pw);
                if (addToRole != null)
                    await um.AddToRoleAsync(user, addToRole);
            }
        }
    }
}
