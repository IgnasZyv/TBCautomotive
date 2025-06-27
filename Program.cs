using CarHostingWeb.Components;
using CarHostingWeb.Models;
using CarHostingWeb.Services;
using CarHostingWeb.Services.Authentication;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddAuthorizationCore();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();

// Add Cloudinary configuration
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));


// Register Cloudinary as a singleton
builder.Services.AddSingleton<CloudinaryDotNet.Cloudinary>(sp =>
{
    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>();
    var account = new CloudinaryDotNet.Account(
        config.Value.CloudName,
        config.Value.ApiKey,
        config.Value.ApiSecret);
    
    return new CloudinaryDotNet.Cloudinary(account);
});

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "Secrets/firebase-key.json");

var firestoreDb = FirestoreDb.Create("ernesto-car-website");
// Preparing firestore for injection.
builder.Services.AddSingleton(firestoreDb);
builder.Services.AddSingleton<CarService>();

// Register FirebaseAuthService as singleton so auth state persists
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddSingleton<FirebaseAuthService>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ImageUploadService>();
builder.Services.AddSingleton<LocalizationService>();

var app = builder.Build();

// Enable session middleware
app.UseSession();

var supportedCultures = new[] { "en", "es" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);


app.MapControllers();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


