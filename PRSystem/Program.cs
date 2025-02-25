using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PRSystem.Data;
using Polly;
using Polly.Extensions.Http;
using PRSystem.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<SAPService>();
builder.Services.AddSingleton<SAPOdbc>();
builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Listen(IPAddress.Loopback, 5021);
    });

//builder.Services.AddSingleton(sp => new SAPOdbc(
//    server: "HANASERVERNBFI:30015",
//    user: "SYSTEM",
//    password: "Sb1@nbfi",
//    databaseNBFI: "Z_NBFI_SBOTEST",
//    databaseEPC: "Z_EPC_SBOTEST"
//));

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
