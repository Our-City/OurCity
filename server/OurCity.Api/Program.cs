using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Configuration;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Middlewares;
using OurCity.Api.Services;
using OurCity.Api.Services.Authorization;
using OurCity.Api.Services.Authorization.CanCreatePosts;
using OurCity.Api.Services.Authorization.CanMutateThisPost;
using Scalar.AspNetCore;
using Serilog;

/*
 * Lots of code setup from ChatGPT, asking for various things at various points (e.g. how to set up cookie auth, how to setup serilog, etc)
 */

var builder = WebApplication.CreateBuilder(args);

//Logging
builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(builder.Configuration));

//CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? []
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    )
);

//Database
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

//Repository
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPostVoteRepository, PostVoteRepository>();
builder.Services.AddScoped<ICommentVoteRepository, CommentVoteRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();

//Service
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddSingleton<AwsS3Service>();

//Configruation
builder.Services.Configure<AwsS3Options>(builder.Configuration.GetSection(AwsS3Options.AWS));

//Controller
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Extensions.Remove("traceId");
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//Authentication
builder
    .Services.AddIdentity<User, UserRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "OurCityAuthToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

//Authorization
builder.Services.AddSingleton<IAuthorizationHandler, CanCreatePostsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanMutateThisPostHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddOurCityPolicies();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseCorrelationId();
app.UseSecurityHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //Multiple API documentation tools
    app.MapScalarApiReference();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.Run();

public partial class Program { }
