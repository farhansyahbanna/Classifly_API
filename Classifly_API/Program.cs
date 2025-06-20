using Classifly_API;
using Classifly_API.Data;
using Classifly_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

//var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Classifly API", Version = "v1" });

    // Konfigurasi JWT di Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});



builder.Services.AddDbContext<ClassiflyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<BorrowService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<DamageReportService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();


//Clodinary configuration
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

//SendGrid configuration
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));

builder.Services.AddTransient<IEmailService, SendGridEmailService>();

//JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value ?? "")), 
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User"))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAssertion(context => true) 
        .Build());


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



//Console.WriteLine("Enter password to hash:");
//var password = Console.ReadLine();

//CreatePasswordHash(password, out byte[] hash, out byte[] salt);

//Console.WriteLine($"PasswordHash: {Convert.ToBase64String(hash)}");
//Console.WriteLine($"PasswordSalt: {Convert.ToBase64String(salt)}");

//static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
//{
//    using var hmac = new HMACSHA512();
//    passwordSalt = hmac.Key;
//    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
//}

//if (isProduction)
//{
//    using var scope = app.Services.CreateScope();
//    var db = scope.ServiceProvider.GetRequiredService<ClassiflyDbContext>();
//    db.Database.Migrate();
//}

app.MapGet("/", () => "Selamat datang di Classifly API! Akses /swagger untuk dokumentasi API.");
app.Run();
