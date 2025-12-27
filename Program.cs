using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using XeniaRentalBackend.Repositories.UserRole;
using XeniaRentalBackend.Repositories.AccountGroups;
using XeniaRentalBackend.Repositories.Auth;
using XeniaRentalBackend.Repositories.BedSpace;
using XeniaRentalBackend.Repositories.BedSpacePlan;
using XeniaRentalBackend.Repositories.Charges;
using XeniaRentalBackend.Repositories.Company;
using XeniaRentalBackend.Repositories.Documents;
using XeniaRentalBackend.Repositories.Ledger;
using XeniaRentalBackend.Repositories.MessDetails;
using XeniaRentalBackend.Repositories.MessTypes;
using XeniaRentalBackend.Repositories.Properties;
using XeniaRentalBackend.Repositories.Tenant;
using XeniaRentalBackend.Repositories.TenantAssignment;
using XeniaRentalBackend.Repositories.Units;
using XeniaRentalBackend.Repositories.Voucher;
using XeniaRentalBackend.Service.Common;
using XeniaRentalBackend.Service.Notification;
using XeniaRentalBackend.Models;
using XeniaRentalBackend.Repositories.Category;
using XeniaRentalBackend.Repositories.Unit;
using XeniaRentalBackend.Repositories.Dashboard;
using XeniaRentalBackend.Repositories.Service;
using XeniaRentalBackend.Repositories.Report;


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
builder.Services.AddScoped<IPropertiesRepository, PropertiesRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();





builder.Services.Configure<FtpSettings>(builder.Configuration.GetSection("FtpSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtHelperService>();



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
