using Pulsa.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Pulsa.Service.Interface;
using Pulsa.Service.Service;
using Pulsa.DataAccess.Interface;
using Pulsa.DataAccess.Repository;
using Pulsa.helper;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAutoMapper(typeof(MappingProfiles));


// Add services to the container.
services.AddControllersWithViews();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/auth/index";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    })  
    .AddGoogle(googleOptions => {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });


services.AddDbContext<PulsaDataContext>(
    o => o.UseNpgsql(configuration.GetConnectionString("puldaDB"))
);

// add data access
services.AddTransient<Pulsa.DataAccess.Interface.ITagihanMasterRepository, Pulsa.DataAccess.Repository.TagihanMasterRepository>();

// add service
services.AddTransient<IUnitOfWork, UnitOfWork>();
services.AddTransient<Pulsa.Service.Interface.ITagihanService, Pulsa.Service.Service.TagihanService>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=index}");

app.Run();
