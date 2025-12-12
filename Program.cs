using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using XeniaRentalApi.Repositories.UserRole;
using XeniaRentalApi.Repositories.AccountGroups;
using XeniaRentalApi.Repositories.Auth;
using XeniaRentalApi.Repositories.BedSpace;
using XeniaRentalApi.Repositories.BedSpacePlan;
using XeniaRentalApi.Repositories.Charges;
using XeniaRentalApi.Repositories.Company;
using XeniaRentalApi.Repositories.Documents;
using XeniaRentalApi.Repositories.Ledger;
using XeniaRentalApi.Repositories.MessDetails;
using XeniaRentalApi.Repositories.MessTypes;
using XeniaRentalApi.Repositories.Properties;
using XeniaRentalApi.Repositories.Tenant;
using XeniaRentalApi.Repositories.TenantAssignment;
using XeniaRentalApi.Repositories.Units;
using XeniaRentalApi.Repositories.Voucher;
using XeniaRentalApi.Service.Common;
using XeniaRentalApi.Service.Notification;
using XeniaRentalApi.Models;
using XeniaRentalApi.Repositories.Category;
using XeniaRentalApi.Repositories.Unit;
using XeniaRentalApi.Repositories.Dashboard;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Xenia Rental API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT like: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins(
                "https://rental.xeniapos.com",
                "https://rental.xeniapos.com/",
                "https://www.rental.xeniapos.com",
                "https://www.rental.xeniapos.com/"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});


builder.Services.AddSignalR();
builder.Services.AddWebSockets(options => { });


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountGroupRepository, AccountGroupRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBedSpaceRepository, BedSpaceRepository>();
builder.Services.AddScoped<IBedSpacePlanRepository, BedSpacePlanRepository>();
builder.Services.AddScoped<IChargesRepository, ChargesRepository>();
builder.Services.AddScoped<ICompanyRepsitory, CompanyRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IAccountLedgerRepository, AccountLedgerRepository>();
builder.Services.AddScoped<IMessAttendancesRepository, MessAttendancesRepository>();
builder.Services.AddScoped<IMessTypes, MessTypes>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantAssignmentRepository, TenantAssignmentRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<INotificationService, OTPService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDashboardRepsitory, DashboardRepository>();






builder.Services.Configure<FtpSettings>(builder.Configuration.GetSection("FtpSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtHelperService>();


builder.Services.AddScoped<AesEncryptionService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    string secretKey = configuration["EncryptionSettings:SecretKey"];
    string secretIv = configuration["EncryptionSettings:SecretIv"];

    if (string.IsNullOrEmpty(secretKey))
        throw new ArgumentNullException(nameof(secretKey), "Secret Key cannot be null. Check your appsettings.json.");

    if (string.IsNullOrEmpty(secretIv))
        throw new ArgumentNullException(nameof(secretIv), "Secret IV cannot be null. Check your appsettings.json.");

    return new AesEncryptionService(secretKey, secretIv);
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    Status = "Error",
                    Message = "Token is missing or invalid."
                });
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    Status = "Error",
                    Message = "You do not have permission to access this resource."
                });
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowAnonymous", policy => policy.RequireAssertion(_ => true));
});


var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
