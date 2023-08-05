using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.ExternalConnectors;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.DataAccess.Repository;
using Pulsa.helper;
using Supabase;
using System.Globalization;
using System.Net;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAutoMapper(typeof(MappingProfiles)); 

services.AddSession();
// Add services to the container.
services.AddControllersWithViews();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/auth/index";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddScoped<Supabase.Client>( option =>
   new Supabase.Client(
        builder.Configuration["Supabase:Url"],
        builder.Configuration["Supabase:Key"],
        new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }));

// config database 
services.AddDbContext<PulsaDataContext>(
    o => o.UseNpgsql(configuration.GetConnectionString("puldaDB"))
);


services.AddTransient<PulsaDataContext>();
services.AddHttpClient();
services.AddHttpContextAccessor();

// add data access
services.AddTransient<Pulsa.DataAccess.Interface.ITagihanMasterRepository, Pulsa.DataAccess.Repository.TagihanMasterRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.ITagihanDetailRepository, Pulsa.DataAccess.Repository.TagihanDetailRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.ITopupRepository, Pulsa.DataAccess.Repository.TopupRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.ITopupMetodeRepository, Pulsa.DataAccess.Repository.TopupMetodeRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IUserSaldoHistoryRepository, Pulsa.DataAccess.Repository.UserSaldoHistoryRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IPenggunaRepository, Pulsa.DataAccess.Repository.PenggunaRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IProvider_h2hRepository, Pulsa.DataAccess.Repository.Provider_h2hRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.ISupplier_produkRepository, Pulsa.DataAccess.Repository.Supplier_produkRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IProdukRepository, Pulsa.DataAccess.Repository.ProdukRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IProdukDetailRepository, Pulsa.DataAccess.Repository.ProdukDetailRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IPenggunaTransaksiRepository, Pulsa.DataAccess.Repository.PenggunaTransaksiRepository>();
services.AddTransient<Pulsa.DataAccess.Interface.IPenggunaMutasiRepository, Pulsa.DataAccess.Repository.PenggunaMutasiRepository>();
services.AddTransient<IUnitOfWork, UnitOfWork>();

// add service
services.AddTransient<Pulsa.Service.Interface.ITagihanService, Pulsa.Service.Service.TagihanService>();
services.AddTransient<Pulsa.Service.Interface.ITopUpService, Pulsa.Service.Service.TopUpService>();
services.AddTransient<Pulsa.Service.Interface.ISerpulService, Pulsa.Service.Service.SerpulService>();
services.AddTransient<Pulsa.Service.Interface.IProdukService, Pulsa.Service.Service.ProdukService>();
services.AddTransient<Pulsa.Service.Interface.ITransaksiService, Pulsa.Service.Service.TransaksiService>();
services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture: "id-ID", uiCulture: "id-ID");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("id-ID") };
    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("id-ID") };
    options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new QueryStringRequestCultureProvider(),
            new CookieRequestCultureProvider()
        };
});

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
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=index}");

app.Run();
