using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WookieBooks.API.Interfaces;
using WookieBooks.API.Services;
using WookieBooks.Common;
using WookieBooks.Entities;
using WookieBooks.Repository;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["AppSettings:Issuer"],
        ValidAudience = configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
}).AddXmlSerializerFormatters();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFG API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


//Dependencies
builder.Services.AddDbContext<WookieBooksContext>(options =>
{
    options.UseInMemoryDatabase("WookieBooks");
    options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});
builder.Services.AddEffCollections();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

CreateUser(app);
CreateRestrictedUser(app);
CreateBook(app);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void CreateUser(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<WookieBooksContext>();
    var users = db?.Users;
    db?.Users.RemoveRange(users);
    db?.SaveChanges();
    var testUsers = new List<User>() {
        new User
        {
            Id = 1,
            UserName = "HGibbs",
            Password = "password",
            Author_Pseudonym = "HG"
        },
        new User
        {
            Id=2,
            UserName = "Anakin24",
            Password = "password",
            Author_Pseudonym = "_Darth Vader_"
        }
    };

    db?.Users.AddRange(testUsers);
    db?.SaveChanges();
}

static void CreateRestrictedUser(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<WookieBooksContext>();
    var restrictedUser = db?.RestrictedUsers;
    db?.RestrictedUsers.RemoveRange(restrictedUser);
    db?.SaveChanges();
    var testrestrictedUser = new List<RestrictedUser>() {
        new RestrictedUser
        {
            RestrictedUserId = 1,
            UserId = 2
        }
    };

    db?.RestrictedUsers.AddRange(testrestrictedUser);
    db?.SaveChanges();
}


static void CreateBook(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<WookieBooksContext>();
    var books = db?.Books;
    db?.Books.RemoveRange(books);
    db?.SaveChanges();
    var testBooks = new List<Book>() {
        new Book
        {
            BookId = 1,
            Title = "Test",
            Description = "Test",
            CoverImage = "Test",
            Author = "HG",
            Price = "2"
        },
        new Book
        {
            BookId = 2,
            Title = "Test1",
            Description = "Test1",
            CoverImage = "Test1",
            Author = "HG",
            Price = "21"
        },
        new Book
        {
            BookId = 3,
            Title = "Test12",
            Description = "Test12",
            CoverImage = "Test12",
            Author = "HG",
            Price = "212"
        }
    };

    db?.Books.AddRange(testBooks);
    db?.SaveChanges();
}

public partial class Program { }