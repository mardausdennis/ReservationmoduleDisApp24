using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using web.Services;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Google.Apis.Calendar.v3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// Add Google authentication
builder.Services.AddAuthentication()
    .AddCookie("Cookies")
    .AddGoogle("GoogleAuth", googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        googleOptions.Events.OnCreatingTicket = ctx =>
        {
            // Store access token in user claims
            ctx.Identity.AddClaim(new Claim("GoogleAccessToken", ctx.AccessToken));
            return Task.CompletedTask;
        };
    });


var googleServiceAccountKeyPath = builder.Configuration["Google:ServiceAccountKeyPath"];

builder.Services.AddSingleton(new GoogleCalendarService(googleServiceAccountKeyPath));
builder.Services.AddSingleton(new FirebaseService(builder.Configuration["FirebaseConfig:ApiKey"], builder.Configuration["FirebaseConfig:AuthDomain"], builder.Configuration["FirebaseConfig:DatabaseUrl"]));
builder.Services.AddSingleton(new EmailService(builder.Configuration["SendGrid:ApiKey"]));


var authOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationOptions>>().Value;
var googleOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<GoogleOptions>>().Get(authOptions.DefaultChallengeScheme);
Console.WriteLine($"ClientId: {googleOptions.ClientId}");
Console.WriteLine($"ClientSecret: {googleOptions.ClientSecret}");
Console.WriteLine($"CallbackPath: {googleOptions.CallbackPath}");



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
