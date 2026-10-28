
using Microsoft.EntityFrameworkCore;
using StudyFlow.Models;

using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 

//Adds Session support 

builder.Services.AddDistributedMemoryCache();


//Making sure users are  logged in to monitor unauthorized activity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath ="/User/Logout"; 
    options.AccessDeniedPath = "/User/Login";
}); 


//Keeps users logged in based on client-side data

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});



//DB Context
builder.Services.AddDbContext<StudyFlowDbContext>(options => {
    options.UseSqlite(builder.Configuration["ConnectionStrings:StudyFlowDbContextConnection"]); 
    });



builder.Services.AddDefaultIdentity<IdentityUser>(options=>
{
    options.SignIn.RequireConfirmedAccount = false; // (disables confirmed email requirement)
}).AddEntityFrameworkStores<StudyFlowDbContext>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    DbInit.Seed(app);
}

//
app.UseSession();
app.MapStaticAssets(); 


//Authorisation and Authentication
app.UseAuthentication();
app.UseAuthorization();


app.MapDefaultControllerRoute();
app.Run();
