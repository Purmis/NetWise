using CatFactsWebApp.Data;
using CatFactsWebApp.Repositories;
using CatFactsWebApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja logowania
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Dodanie serwisów do kontenera DI
builder.Services.AddControllersWithViews();

// Konfiguracja Entity Framework z bazą danych w pamięci
builder.Services.AddDbContext<CatFactsDbContext>(options =>
    options.UseInMemoryDatabase("CatFactsDb"));

// Rejestracja repozytoriów (warstwa dostępu do danych)
builder.Services.AddScoped<ICatFactRepository, CatFactRepository>();

// Rejestracja serwisów biznesowych
builder.Services.AddScoped<ICatFactBusinessService, CatFactBusinessService>();

// Rejestracja HttpClient dla serwisu API
builder.Services.AddHttpClient<ICatFactService, CatFactService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "CatFactsWebApp/1.0");
});

// Konfiguracja cache'owania
builder.Services.AddMemoryCache();

// Konfiguracja sesji (opcjonalnie)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicjalizacja bazy danych z przykładowymi danymi
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatFactsDbContext>();
    context.Database.EnsureCreated();
}

// Konfiguracja pipeline'u HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Konfiguracja routingu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Dodatkowe routy dla API
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action}/{id?}");

app.Run();
