using FuelManagementAPI.Data;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using FuelManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpContextAccessor();
// Service Registrations
builder.Services.AddScoped<DailyPriceUpdaterService>();
builder.Services.AddHostedService<DailyBackgroundPriceUpdateService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new UtcDateTimeConverterNewtonsoft());
    });


builder.Services.AddDbContext<FuelDbContext>(optiions =>
    optiions.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(FuelDbContext));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAttendantRepository, AttendantRepository>();
builder.Services.AddScoped<IFuelEntryRepository, FuelEntryRepository>();
builder.Services.AddScoped<IFuelSalesRepository, FuelSaleRepository>();
builder.Services.AddScoped<ILubeEntryRepository, LubeEntryRepository>();
builder.Services.AddScoped<ILubeSalesRepository, LubeSalesRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryTypeRepository, CategoryTypeRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<AuthService>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost",
//        builder => builder.WithOrigins("http://localhost:3000")
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .AllowCredentials());
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAndProd", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://fuel-manager-wheat.vercel.app/"
        )
        .AllowAnyHeader()
        .AllowCredentials()
        .AllowAnyMethod();
    });
});

var jwtSetting = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]);

builder.Services
    .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSetting["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSetting["Audience"],
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowLocalAndProd");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "API is running!");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FuelDbContext>();
    db.Database.Migrate();
}

app.Run($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

//app.Run();
