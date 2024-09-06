using AngularApp.Server;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Enable CORS
        builder.Services.AddCors();
        
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        /*
        Jwt configuration starts here
        var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
        var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
             options.IncludeErrorDetails = true;
             options.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 ValidateIssuerSigningKey = true,
                 ValidIssuer = jwtIssuer,
                 ValidAudience = jwtIssuer,
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
             };
         });
        Jwt configuration ends here
        */

        var app = builder.Build();
        
        /*
        app.Use(async (context, next) =>
        {
            System.Diagnostics.Debug.WriteLine("Under Use");
            // Do work that can write to the Response.
            await next.Invoke();
            // Do logging or other work that doesn't write to the Response.
        });
        */

        app.UseUserCustomMiddleWare();

        /*
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/User"), appBuilder =>
        {
            System.Diagnostics.Debug.WriteLine("Under Use when");
            // appBuilder.UseMiddleware<MyMiddlewareOne>();
        });
        */

        app.MapGet("/", () => "Hello, World!");
        app.MapGet("/secret", () => $"Hello My secret")
            .RequireAuthorization();

        /* 
        app.Map("/map1", HandleMapTest1);
        app.Map("/map2", HandleMapTest2);
        static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }

        static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 2");
            });
        }
        */

        app.UseDefaultFiles();
        app.UseStaticFiles();  

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseCors(options =>
            options.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod());

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        /*
        app.Run(async context =>
        {
            System.Diagnostics.Debug.WriteLine("Under Run");
            await context.Response.WriteAsync("Hello from 2nd delegate.");
        });
        */

        app.Run();

    }
}
