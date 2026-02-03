
using Microsoft.EntityFrameworkCore;
namespace XeniaRentalBackend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<XRS_Users> Users { get; set; }

        public DbSet<XRS_UserRole> UserRoles { get; set; }

        public DbSet<XRS_UserMapping> UserMapping { get; set; }

        public DbSet<XRS_AccountGroup> AccountGroups { get; set; }

        public DbSet<XRS_Accounts> Accounts { get; set; }

        public DbSet<XRS_Agent> Agent { get; set; }

        public DbSet<XRS_Bedspace> BedSpaces { get; set; }

        public DbSet<XRS_BedSpacePlan> BedSpacePlans { get; set; }

        public DbSet<XRS_BedspacePlanMessMapping> BedspacePlanMessMappings { get; set; }

        public DbSet<XRS_Charges> Charges { get; set; }

        public DbSet<XRS_Company> Company { get; set; }

        public DbSet<XRS_CompanySubscription> CompanySubscription { get; set; }

        public DbSet<XRS_Documents> Documents { get; set; }

        public DbSet<XRS_AccountLedger> Ledgers { get; set; }

        public DbSet<XRS_MessAttendance> MessAttendances { get; set; }

        public DbSet<XRS_Messtypes> MessTypes { get; set; }

        public DbSet<XRS_Properties> Properties { get; set; }

        public DbSet<XRS_Tenant> Tenants { get; set; }

        public DbSet<XRS_TenantAssignment> TenantAssignemnts { get; set; }

        public DbSet<XRS_TenantDocuments> TenantDocuments { get; set; }

        public DbSet<XRS_TenantChequeRegister> TenantChequeRegisters { get; set; }

        public DbSet<XRS_Units> Units { get; set; }

        public DbSet<XRS_AdvText> AdvText { get; set; }

        public DbSet<XRS_Banners> Banners { get; set; }

        public DbSet<XRS_SubscribePlan> SubscribePlan { get; set; }

        public DbSet<XRS_SubscriptionTransaction> SubscriptionTransaction { get; set; }

        public DbSet<XRS_Voucher> Vouchers { get; set; }

        public DbSet<XRS_PlanModuleMap> PlanModuleMap { get; set; }

        public DbSet<XRS_Module> Module { get; set; }

        public DbSet<XRS_VoucherDetails> VoucherDetails { get; set; }

        public DbSet<XRS_UnitChargesMapping> UnitChargesMappings { get; set; }

        public DbSet<XRS_OTPLog> tblOTPLogs { get; set; }

        public DbSet<XRS_NotificationSettings> tblNotifications { get; set; }

        public DbSet<XRS_CompanySettings> CompanySettings {  get; set; }

        public DbSet<XRS_Categories> Category { get; set; }
        public DbSet<XRS_Service> Services { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<XRS_Users>()
          .HasOne(u => u.UserRole)
          .WithMany()
          .HasForeignKey(u => u.UserType)
          .HasPrincipalKey(r => r.UserRoleId);

        }

    }

}

