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
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
            builder.Services.AddScoped(typeof(IBookRepository), typeof(BookRepository));
            builder.Services.AddScoped(typeof(IBookService), typeof(BookService));

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
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
