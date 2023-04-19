using web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton(new GoogleCalendarService(builder.Configuration["GoogleConfig:ApiKey"]));
builder.Services.AddSingleton(new FirebaseService(builder.Configuration["FirebaseConfig:ApiKey"], builder.Configuration["FirebaseConfig:AuthDomain"], builder.Configuration["FirebaseConfig:DatabaseUrl"]));
builder.Services.AddSingleton(new EmailService(builder.Configuration["SendGrid:ApiKey"]));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
