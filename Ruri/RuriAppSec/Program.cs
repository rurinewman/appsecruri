using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using RuriAppSec.Model;
using RuriAppSec.Pages.CustomTokenProviders;
using RuriAppSec.Pages.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<AuditLogTrailsService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PasswordServiceManager>();


builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ACCOUNT LOCKOUT
    options.Lockout.AllowedForNewUsers = true;
    //after 2 mins, allow unlocking of a locked account
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 3;
    // UNIQUE EMAIL
    //options.User.RequireUniqueEmail = true;



}).AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);

});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
//for custom 404 error
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseAuthorization();
app.MapRazorPages();
app.Run();
