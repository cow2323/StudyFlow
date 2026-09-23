
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 


builder.Services.AddDbContext<StudyFlowDbContext>(options => {
    options.UseSqlite(builder.Configuration["ConnectionStrings:StudyFlowDbContextConnection"]); 
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
}

app.MapStaticAssets(); 
app.MapDefaultControllerRoute();
app.Run();
