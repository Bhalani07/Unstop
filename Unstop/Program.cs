using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;
using System.Text;
using Unstop.Services;
using Unstop.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<IJobService, JobService>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddHttpClient<IProfileService, ProfileService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddHttpClient<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient<IInterviewService, InterviewService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();

builder.Services.AddHttpClient<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();

builder.Services.AddHttpClient<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

builder.Services.AddHttpClient<IJobFairService, JobFairService>();
builder.Services.AddScoped<IJobFairService, JobFairService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddKendo();

//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRotativa();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
