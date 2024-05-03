using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjTravelPlatform_release.Areas.CustomizedIdentity;
using prjTravelPlatform_release.Extended.MailSend;
//using prjTravelPlatform_release.Areas.Customer.ViewModel.Airline;
using prjTravelPlatformV3.Data;
using prjTravelPlatformV3.Models;
using System.Security.Claims;
using System.Text;
using CoreMVC_SignalR_Chat.Hubs;
using prjTravelPlatform_release.Areas.Customer.SendMailService;
using System.Web;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<dbTravalPlatformContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection")));
//builder.Services.AddDbContext<dbTravalPlatformContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DbTravelConnection")));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

//¥[¤J SignalR
builder.Services.AddSignalR();


builder.Services.AddSwaggerGen();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
	option.AddPolicy("AllowSpecificOrigin",
	   builder => builder.WithOrigins("https://lively-sea-0210e9700.5.azurestaticapps.net")
	   .AllowAnyMethod()
	   .AllowAnyHeader());
});


//cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(100);
        options.LoginPath = "/CustomizedIdentity/Login/Index";
        options.SlidingExpiration = true;
        options.LogoutPath = "/Customer/Home/Index";
        //options.AccessDeniedPath = "/CustomizedIdentity/Login/Index";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                var requestedPath = context.Request.Path + context.Request.QueryString;
                var role = context.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var loginPath = DetermineLoginPath(requestedPath, role);

                context.Response.Redirect(loginPath);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                var role = context.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var requestedPath = context.Request.Path + context.Request.QueryString;
                context.Response.Redirect(DetermineAccessDeniedPath(requestedPath, role));
                return Task.CompletedTask;
            }
        };
    });

#region jwt
//jwt
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidateAudience = true,
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
//        };
//    });
#endregion

//SignalR
builder.Services.AddSignalR();   

//mailService
builder.Services.AddTransient<MailService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddSingleton<TokenManager>();
builder.Services.AddScoped<PasswordResetService>();



var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "Areas",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

//Hub
app.MapHub<adHub>("/adHub");
app.MapHub<ChatHub>("/ChatHub");

app.Run();


string DetermineLoginPath(PathString requestedPath, string role)
{
    

    if (requestedPath.StartsWithSegments("/Employee") && role != "Employee")
    {
        string encodedRequestedPath = HttpUtility.UrlEncode(requestedPath.Value);
        return $"/CustomizedIdentity/Login/Employee?returnUrl={encodedRequestedPath}";
    }
    else
    {
        string encodedRequestedPath = HttpUtility.UrlEncode(requestedPath.Value);
        return $"/CustomizedIdentity/Login/Index?returnUrl={encodedRequestedPath}";
    }
}

string DetermineAccessDeniedPath(PathString requestedPath, string role)
{

    string encodedRequestedPath = HttpUtility.UrlEncode(requestedPath.Value);
    
    if (role == "Employee")
    {
        return $"/CustomizedIdentity/Login/Index?returnUrl={encodedRequestedPath}";
    }
    else
    {
        return $"/CustomizedIdentity/Login/Employee?returnUrl={encodedRequestedPath}";
    }
}
