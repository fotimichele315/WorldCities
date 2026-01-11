using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WorldCities.Server.Data.GraphQL;



var builder = WebApplication.CreateBuilder(args);

// Add Serilog support
builder.Host.UseSerilog(
    (ctx,lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MSSqlServer(connectionString:
    "Server=tcp:worldcities2.database.windows.net,1433;Initial Catalog=worldcities;Persist Security Info=False;User ID=worldcities;Password=Malpselamps1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", 
    restrictedToMinimumLevel: LogEventLevel.Information,
    sinkOptions: new MSSqlServerSinkOptions
    { TableName = "LogEvents",
    AutoCreateSqlTable = true}).WriteTo.Console()
    );
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Application DBContext and SQL Serve support
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=tcp:worldcities2.database.windows.net,1433;Initial Catalog=worldcities;Persist Security Info=False;User ID=worldcities;Password=Malpselamps1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

//Add Asp.NET Core Identity support 

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDbContext>();    


builder.Services.AddScoped<JwtHandler>();

builder.Services.AddGraphQLServer().AddAuthorization().AddQueryType<Query>().AddMutationType<Mutation>().AddFiltering().AddSorting();


// Add authentication  services & middleware
builder.Services.AddAuthentication(opt => { 
opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => { 


options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters { 

    RequireExpirationTime = true,
    ValidateIssuer = true,  
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JwSettings:Issuer"],
    ValidAudience = builder.Configuration["JwSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwSettings:SecurityKey"]!))



}; }   );

 


var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/api/graphql");

app.MapFallbackToFile("/index.html");

app.Run();
