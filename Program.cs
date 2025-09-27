using CarHostingWeb.Components;
using CarHostingWeb.Models;
using CarHostingWeb.Services;
using CarHostingWeb.Services.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Railway-specific configuration
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Railway uses port 8080
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure SignalR for Railway
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

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

// Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "Secrets/firebase-key.json");


// Instead of using a file path, use environment variable
var firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");

if (!string.IsNullOrEmpty(firebaseCredentialsJson))
{
    // Create credentials from JSON string
    var credential = GoogleCredential.FromJson(firebaseCredentialsJson);
    var firestoreDb = new FirestoreDbBuilder
    {
        ProjectId = "ernesto-car-website",
        Credential = credential
    }.Build();
    
    builder.Services.AddSingleton(firestoreDb);
}
else
{
    // Fallback for local development
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "Secrets/firebase-key.json");
    var firestoreDb = FirestoreDb.Create("ernesto-car-website");
    builder.Services.AddSingleton(firestoreDb);
}

// var firestoreDb = FirestoreDb.Create("ernesto-car-website");
// Preparing firestore for injection.
// builder.Services.AddSingleton(firestoreDb);

builder.Services.AddSingleton<CarService>();

// Register FirebaseAuthService as singleton so auth state persists
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ImageUploadService>();
builder.Services.AddSingleton<LocalizationService>();

var app = builder.Build();

// Railway-specific middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    
// Add headers for Railway
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        await next();
    });
}

// Enable session middleware
app.UseSession();

var supportedCultures = new[] { "en", "es" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("es")
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

// app.UseHttpsRedirection();

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


