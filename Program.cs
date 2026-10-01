
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 

//Adds Session support 

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});



//DB Context
builder.Services.AddDbContext<StudyFlowDbContext>(options => {
    options.UseSqlite(builder.Configuration["ConnectionStrings:StudyFlowDbContextConnection"]); 
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
}

app.UseSession();
app.MapStaticAssets(); 
app.MapDefaultControllerRoute();
app.Run();
