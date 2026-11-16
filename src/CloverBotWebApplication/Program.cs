using CloverBotWebApplication.Data;
using CloverBotWebApplication.ServiceInterface;
using CloverBotWebApplication.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration: connection string ---
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

// --- Register DbContext ---
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connStr));

// --- MVC ---
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// --- Register HttpClient factory for RpaService ---
builder.Services.AddHttpClient();

// --- Register ChatService (singleton) ---
builder.Services.AddSingleton<IChatService, ChatService>();

// --- Register RPA service ---
builder.Services.AddScoped<IRpaService, RpaService>();
builder.Services.AddSingleton<CsvLogger>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Chat route
app.MapControllerRoute(
    name: "clovey",
    pattern: "{controller=Chat}/{action=Index}/{id?}"
);

app.Run();
