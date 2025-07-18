namespace LibraryManagement.Web
{
    using Data;
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Repository;
    using LibraryManagement.Data.Seeding;
    using LibraryManagement.Services.Core;
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
            
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services
                .AddDbContext<LibraryManagementDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services
                     .AddIdentity<IdentityUser, IdentityRole>(options =>
                     {
                         options.SignIn.RequireConfirmedAccount = false;
                         options.Password.RequireNonAlphanumeric = false;
                         options.Password.RequireLowercase = false;
                         options.Password.RequireUppercase = false;
                         options.Password.RequireDigit = false;
                     })
                     .AddEntityFrameworkStores<LibraryManagementDbContext>()
                     .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddScoped(typeof(IRepository<,>), (typeof(BaseRepository<,>)));
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IGenreRepository, GenreRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IGenreService, GenreService>();
            builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();
            builder.Services.AddScoped<IBorrowingRecordRepository, BorrowingRecordRepository>();
            builder.Services.AddScoped<IBorrowingRecordService, BorrowingRecordService>();
            builder.Services.AddScoped<IFavouriteListRepository, FavouriteListRepository>();
            builder.Services.AddScoped<IFavouriteListService, FavouriteListService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            WebApplication? app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await LibraryManagementDbContextSeed.SeedAsync(services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.MapRazorPages();

            app.Run();
        }
    }
}
