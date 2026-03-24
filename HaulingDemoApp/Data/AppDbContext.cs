using HaulingDemoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HaulingDemoApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<FuelReceipt> FuelReceipts { get; set; }
        public DbSet<FuelUsage> FuelUsages { get; set; }
        public DbSet<TyrePO> TyrePOs { get; set; }
        public DbSet<TyreProblem> TyreProblems { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<GoodReceipt> GoodReceipts { get; set; }
        public DbSet<GoodReceiptItem> GoodReceiptItems { get; set; }
        public DbSet<GoodIssue> GoodIssues { get; set; }
        public DbSet<GoodIssueItem> GoodIssueItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<FleetVehicle> FleetVehicles { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<PreventiveMaintenance> PreventiveMaintenances { get; set; }
        public DbSet<CorrectiveMaintenance> CorrectiveMaintenances { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalLine> JournalLines { get; set; }
        public DbSet<ProductionData> ProductionData { get; set; }

        // Hauling Operations
        public DbSet<HaulTrip> HaulTrips { get; set; }
        public DbSet<RouteMaster> RouteMasters { get; set; }
        public DbSet<WeighbridgeTicket> WeighbridgeTickets { get; set; }
        public DbSet<FuelAnalysis> FuelAnalyses { get; set; }
        public DbSet<UnitCostTracking> UnitCostTrackings { get; set; }
        public DbSet<DriverProductivity> DriverProductivities { get; set; }

        // Master Data
        public DbSet<Site> Sites { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<COA> COAs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure FuelReceipt
            modelBuilder.Entity<FuelReceipt>(entity =>
            {
                entity.ToTable("fuel_receipts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.No).IsRequired();
                entity.Property(e => e.Tanggal).IsRequired().HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Vendor).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Liter).HasPrecision(18, 2);
                entity.Property(e => e.JenisBBM).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HargaPerLiter).HasPrecision(18, 2);
                entity.Property(e => e.TotalHarga).HasPrecision(18, 2);
                entity.Property(e => e.NoTiket).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.Keterangan).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure FuelUsage
            modelBuilder.Entity<FuelUsage>(entity =>
            {
                entity.ToTable("fuel_usages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.No).IsRequired();
                entity.Property(e => e.Tanggal).IsRequired().HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Operator).IsRequired().HasMaxLength(100);
                entity.Property(e => e.HM).HasPrecision(18, 2);
                entity.Property(e => e.KM).HasPrecision(18, 2);
                entity.Property(e => e.Pemakaian).HasPrecision(18, 2);
                entity.Property(e => e.JamKerja).HasPrecision(18, 2);
                entity.Property(e => e.EFisiensi).HasPrecision(18, 2);
                entity.Property(e => e.Keterangan).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure TyrePO
            modelBuilder.Entity<TyrePO>(entity =>
            {
                entity.ToTable("tyres_po");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.No).IsRequired();
                entity.Property(e => e.Tanggal).IsRequired().HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Vendor).HasMaxLength(200);
                entity.Property(e => e.NoPO).HasMaxLength(50);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Qty).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure TyreProblem
            modelBuilder.Entity<TyreProblem>(entity =>
            {
                entity.ToTable("tyres_problems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.No).IsRequired();
                entity.Property(e => e.Tanggal).IsRequired().HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).HasMaxLength(50);
                entity.Property(e => e.SerialNumber).HasMaxLength(100);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Problem).HasMaxLength(500);
                entity.Property(e => e.Kerusakan).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.StartHM).HasPrecision(18, 2);
                entity.Property(e => e.EndHM).HasPrecision(18, 2);
                entity.Property(e => e.TotalHM).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MaterialDescription).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MinStock).HasPrecision(18, 2);
                entity.Property(e => e.MaxStock).HasPrecision(18, 2);
                entity.Property(e => e.Stock).HasPrecision(18, 2);
                entity.Property(e => e.QtyMinAlert).HasPrecision(18, 2);
                entity.Property(e => e.StockValue).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.LastPOPrice).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure PurchaseRequest
            modelBuilder.Entity<PurchaseRequest>(entity =>
            {
                entity.ToTable("purchase_requests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PRNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PRDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RequestedBy).HasMaxLength(100);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure PurchaseRequestItem
            modelBuilder.Entity<PurchaseRequestItem>(entity =>
            {
                entity.ToTable("purchase_request_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PurchaseRequestId).IsRequired();
                entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Quantity).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.Purpose).HasMaxLength(100);
                entity.Property(e => e.Priority).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.PurchaseRequest)
                    .WithMany()
                    .HasForeignKey(e => e.PurchaseRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.ToTable("purchase_orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PONumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PODate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Vendor).IsRequired().HasMaxLength(200);
                entity.Property(e => e.VendorCode).HasMaxLength(50);
                entity.Property(e => e.PRNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.DeliveryDate).HasColumnType("timestamp");
                entity.Property(e => e.DeliveryAddress).HasMaxLength(100);
                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
                entity.Property(e => e.Tax).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentTerms).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure PurchaseOrderItem
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.ToTable("purchase_order_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PurchaseOrderId).IsRequired();
                entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Quantity).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.DeliveredQty).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany()
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure GoodReceipt
            modelBuilder.Entity<GoodReceipt>(entity =>
            {
                entity.ToTable("good_receipts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GRNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.GRDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Vendor).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PONumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.ReceivedBy).HasMaxLength(100);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure GoodReceiptItem
            modelBuilder.Entity<GoodReceiptItem>(entity =>
            {
                entity.ToTable("good_receipt_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GoodReceiptId).IsRequired();
                entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.OrderedQty).HasPrecision(18, 2);
                entity.Property(e => e.ReceivedQty).HasPrecision(18, 2);
                entity.Property(e => e.AcceptedQty).HasPrecision(18, 2);
                entity.Property(e => e.RejectedQty).HasPrecision(18, 2);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.GoodReceipt)
                    .WithMany()
                    .HasForeignKey(e => e.GoodReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure GoodIssue
            modelBuilder.Entity<GoodIssue>(entity =>
            {
                entity.ToTable("good_issues");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GINumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.GIDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.RequestNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.IssuedBy).HasMaxLength(100);
                entity.Property(e => e.ReceivedBy).HasMaxLength(100);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure GoodIssueItem
            modelBuilder.Entity<GoodIssueItem>(entity =>
            {
                entity.ToTable("good_issue_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GoodIssueId).IsRequired();
                entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RequestedQty).HasPrecision(18, 2);
                entity.Property(e => e.IssuedQty).HasPrecision(18, 2);
                entity.Property(e => e.StockBefore).HasPrecision(18, 2);
                entity.Property(e => e.StockAfter).HasPrecision(18, 2);
                entity.Property(e => e.Purpose).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.GoodIssue)
                    .WithMany()
                    .HasForeignKey(e => e.GoodIssueId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Vendor
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("vendors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VendorCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.VendorName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VendorType).HasMaxLength(10).HasColumnName("VendorType");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Province).HasMaxLength(50).HasColumnName("Province");
                entity.Property(e => e.PostalCode).HasMaxLength(20).HasColumnName("PostalCode");
                entity.Property(e => e.Country).HasMaxLength(50).HasColumnName("Country");
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Fax).HasMaxLength(50).HasColumnName("Fax");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(50).HasColumnName("ContactPhone");
                entity.Property(e => e.TaxId).HasMaxLength(50);
                entity.Property(e => e.NIB).HasMaxLength(30).HasColumnName("NIB");
                entity.Property(e => e.BankName).HasMaxLength(50).HasColumnName("BankName");
                entity.Property(e => e.BankAccountName).HasMaxLength(50).HasColumnName("BankAccountName");
                entity.Property(e => e.BankAccountNumber).HasMaxLength(50).HasColumnName("BankAccountNumber");
                entity.Property(e => e.BankBranch).HasMaxLength(50).HasColumnName("BankBranch");
                entity.Property(e => e.PaymentTerms).HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.TotalPurchases).HasPrecision(18, 2);
                entity.Property(e => e.OutstandingBalance).HasPrecision(18, 2);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure FleetVehicle (updated with full hauling specs)
            modelBuilder.Entity<FleetVehicle>(entity =>
            {
                entity.ToTable("fleet_vehicles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UnitDescription).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.LicensePlate).HasMaxLength(20);
                entity.Property(e => e.ChassisNumber).HasMaxLength(50);
                entity.Property(e => e.EngineNumber).HasMaxLength(50);
                entity.Property(e => e.VehicleType).HasMaxLength(50);
                entity.Property(e => e.FuelType).HasMaxLength(50);
                entity.Property(e => e.GrossWeight).HasPrecision(18, 4);
                entity.Property(e => e.TareWeight).HasPrecision(18, 4);
                entity.Property(e => e.PayloadCapacity).HasPrecision(18, 4);
                entity.Property(e => e.MaxPayload).HasPrecision(18, 4);
                entity.Property(e => e.FuelTankCapacity).HasPrecision(18, 4);
                entity.Property(e => e.AvgFuelConsumption).HasPrecision(18, 4);
                entity.Property(e => e.BenchmarkLitrePerKM).HasPrecision(18, 4);
                entity.Property(e => e.BenchmarkLitrePerHour).HasPrecision(18, 4);
                entity.Property(e => e.FuelCardNumber).HasMaxLength(50);
                entity.Property(e => e.TyreSize).HasMaxLength(50);
                entity.Property(e => e.TyreCostPerUnit).HasPrecision(18, 4);
                entity.Property(e => e.AvgTyreLifeKM).HasPrecision(18, 4);
                entity.Property(e => e.PemasanganDate).HasColumnType("timestamp");
                entity.Property(e => e.HMAwal).HasPrecision(18, 4);
                entity.Property(e => e.KMAwal).HasPrecision(18, 4);
                entity.Property(e => e.HMakhir).HasPrecision(18, 4);
                entity.Property(e => e.KMakhir).HasPrecision(18, 4);
                entity.Property(e => e.TotalJam).HasPrecision(18, 4);
                entity.Property(e => e.TotalKM).HasPrecision(18, 4);
                entity.Property(e => e.FuelRatio).HasPrecision(18, 4);
                entity.Property(e => e.AvgHMHari).HasPrecision(18, 4);
                entity.Property(e => e.AvgKMHari).HasPrecision(18, 4);
                entity.Property(e => e.HMUsage).HasPrecision(18, 4);
                entity.Property(e => e.TotalFuel).HasPrecision(18, 4);
                entity.Property(e => e.CostCenter).HasMaxLength(50);
                entity.Property(e => e.RouteCode).HasMaxLength(50);
                entity.Property(e => e.AssignedDriverId).HasMaxLength(50);
                entity.Property(e => e.AcquisitionCost).HasPrecision(18, 4);
                entity.Property(e => e.DepreciationRate).HasPrecision(18, 4);
                entity.Property(e => e.AccumulatedDepreciation).HasPrecision(18, 4);
                entity.Property(e => e.BookValue).HasPrecision(18, 4);
                entity.Property(e => e.InsuranceExpiry).HasColumnType("timestamp");
                entity.Property(e => e.TaxExpiry).HasColumnType("timestamp");
                entity.Property(e => e.KIRExpiry).HasColumnType("timestamp");
                entity.Property(e => e.STNKExpiry).HasColumnType("timestamp");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure WorkOrder
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.ToTable("work_orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WONumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.WODate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.WOType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Priority).HasMaxLength(50);
                entity.Property(e => e.Problem).HasMaxLength(255);
                entity.Property(e => e.Cause).HasMaxLength(255);
                entity.Property(e => e.ActionTaken).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.ScheduledDate).HasColumnType("timestamp");
                entity.Property(e => e.StartDate).HasColumnType("timestamp");
                entity.Property(e => e.EndDate).HasColumnType("timestamp");
                entity.Property(e => e.EstimatedCost).HasPrecision(18, 2);
                entity.Property(e => e.ActualCost).HasPrecision(18, 2);
                entity.Property(e => e.AssignedTo).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure PreventiveMaintenance
            modelBuilder.Entity<PreventiveMaintenance>(entity =>
            {
                entity.ToTable("preventive_maintenance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PMNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PMDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.PMType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.ScheduledDate).HasColumnType("timestamp");
                entity.Property(e => e.StartDate).HasColumnType("timestamp");
                entity.Property(e => e.EndDate).HasColumnType("timestamp");
                entity.Property(e => e.HMValue).HasPrecision(18, 2);
                entity.Property(e => e.NextHMValue).HasPrecision(18, 2);
                entity.Property(e => e.AssignedTo).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure CorrectiveMaintenance
            modelBuilder.Entity<CorrectiveMaintenance>(entity =>
            {
                entity.ToTable("corrective_maintenance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CMNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CMDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MerkType).HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.CMType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Priority).HasMaxLength(50);
                entity.Property(e => e.Problem).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RootCause).HasMaxLength(255);
                entity.Property(e => e.Solution).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.BreakdownStart).HasColumnType("timestamp");
                entity.Property(e => e.BreakdownEnd).HasColumnType("timestamp");
                entity.Property(e => e.DowntimeHours).HasPrecision(18, 2);
                entity.Property(e => e.RepairCost).HasPrecision(18, 2);
                entity.Property(e => e.PartsCost).HasPrecision(18, 2);
                entity.Property(e => e.LaborCost).HasPrecision(18, 2);
                entity.Property(e => e.ReportedBy).HasMaxLength(100);
                entity.Property(e => e.AssignedTo).HasMaxLength(100);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("hr_employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NickName).HasMaxLength(100);
                entity.Property(e => e.PIN).HasMaxLength(50);
                entity.Property(e => e.RFID).HasMaxLength(50);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.BirthDate).HasColumnType("timestamp");
                entity.Property(e => e.BirthPlace).HasMaxLength(100);
                entity.Property(e => e.Religion).HasMaxLength(50);
                entity.Property(e => e.MaritalStatus).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.EmergencyContact).HasMaxLength(100);
                entity.Property(e => e.EmergencyPhone).HasMaxLength(100);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Level).HasMaxLength(50);
                entity.Property(e => e.EmployeeType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.JoinDate).HasColumnType("timestamp");
                entity.Property(e => e.ResignDate).HasColumnType("timestamp");
                entity.Property(e => e.PhotoUrl).HasMaxLength(255);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Attendance
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("hr_attendance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EmployeeName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AttendanceDate).HasColumnType("timestamp");
                entity.Property(e => e.CheckIn).HasColumnType("timestamp");
                entity.Property(e => e.CheckOut).HasColumnType("timestamp");
                entity.Property(e => e.Shift).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.WorkingHours).HasPrecision(18, 2);
                entity.Property(e => e.OvertimeHours).HasPrecision(18, 2);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Payroll
            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.ToTable("hr_payroll");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PayrollNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PayrollDate).HasColumnType("timestamp");
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EmployeeName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BasicSalary).HasPrecision(18, 2);
                entity.Property(e => e.Allowance).HasPrecision(18, 2);
                entity.Property(e => e.Overtime).HasPrecision(18, 2);
                entity.Property(e => e.Bonus).HasPrecision(18, 2);
                entity.Property(e => e.TotalEarning).HasPrecision(18, 2);
                entity.Property(e => e.AbsenceDeduction).HasPrecision(18, 2);
                entity.Property(e => e.TaxDeduction).HasPrecision(18, 2);
                entity.Property(e => e.InsuranceDeduction).HasPrecision(18, 2);
                entity.Property(e => e.OtherDeduction).HasPrecision(18, 2);
                entity.Property(e => e.TotalDeduction).HasPrecision(18, 2);
                entity.Property(e => e.NetSalary).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.PaidDate).HasColumnType("timestamp");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure ChartOfAccount
            modelBuilder.Entity<ChartOfAccount>(entity =>
            {
                entity.ToTable("chart_of_accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ParentAccountCode).HasMaxLength(20);
                entity.Property(e => e.NormalBalance).HasMaxLength(10);
                entity.Property(e => e.CostCenterRequired).HasMaxLength(50);
                entity.Property(e => e.TaxCode).HasMaxLength(50);
                entity.Property(e => e.Currency).HasMaxLength(50);
                entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
                entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
                entity.Property(e => e.Remarks).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Budget
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.ToTable("budgets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BudgetNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Division).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountName).HasMaxLength(200);
                entity.Property(e => e.PlannedAmount).HasPrecision(18, 2);
                entity.Property(e => e.ActualAmount).HasPrecision(18, 2);
                entity.Property(e => e.CommittedAmount).HasPrecision(18, 2);
                entity.Property(e => e.AvailableBudget).HasPrecision(18, 2);
                entity.Property(e => e.UtilizationPercent).HasPrecision(5, 2);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.ApprovedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure JournalEntry
            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.ToTable("journal_entries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EntryDate).HasColumnType("timestamp");
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.EntryType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SourceModule).HasMaxLength(100);
                entity.Property(e => e.SourceId).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalDebit).HasPrecision(18, 2);
                entity.Property(e => e.TotalCredit).HasPrecision(18, 2);
                entity.Property(e => e.PostedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure JournalLine
            modelBuilder.Entity<JournalLine>(entity =>
            {
                entity.ToTable("journal_lines");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountName).HasMaxLength(200);
                entity.Property(e => e.DC).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.CostCenter).HasMaxLength(100);
                entity.Property(e => e.Site).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.JournalEntry)
                    .WithMany(j => j.JournalLines)
                    .HasForeignKey(e => e.JournalEntryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ProductionData
            modelBuilder.Entity<ProductionData>(entity =>
            {
                entity.ToTable("production_data");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Division).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalTonase).HasPrecision(18, 2);
                entity.Property(e => e.TotalOperatingHours).HasPrecision(18, 2);
                entity.Property(e => e.TotalOverburden).HasPrecision(18, 2);
                entity.Property(e => e.HaulingDistance).HasPrecision(18, 2);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
                entity.Property(e => e.CostPerTon).HasPrecision(18, 4);
                entity.Property(e => e.FuelCost).HasPrecision(18, 2);
                entity.Property(e => e.MaintenanceCost).HasPrecision(18, 2);
                entity.Property(e => e.LaborCost).HasPrecision(18, 2);
                entity.Property(e => e.OtherCost).HasPrecision(18, 2);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure HaulTrip
            modelBuilder.Entity<HaulTrip>(entity =>
            {
                entity.ToTable("haul_trips");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TripNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TripDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DriverId).HasMaxLength(50);
                entity.Property(e => e.DriverName).HasMaxLength(100);
                entity.Property(e => e.RouteCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RouteName).HasMaxLength(100);
                entity.Property(e => e.Shift).HasMaxLength(20);
                entity.Property(e => e.MaterialType).HasMaxLength(50);
                entity.Property(e => e.OriginPit).HasMaxLength(50);
                entity.Property(e => e.DestinationStockpile).HasMaxLength(50);
                entity.Property(e => e.WBTicketIn).HasMaxLength(50);
                entity.Property(e => e.WBTicketOut).HasMaxLength(50);
                entity.Property(e => e.GrossWeight).HasPrecision(18, 4);
                entity.Property(e => e.TareWeight).HasPrecision(18, 4);
                entity.Property(e => e.NetWeight).HasPrecision(18, 4);
                entity.Property(e => e.PayloadTon).HasPrecision(18, 4);
                entity.Property(e => e.StartTime).HasColumnType("timestamp");
                entity.Property(e => e.EndTime).HasColumnType("timestamp");
                entity.Property(e => e.CycleTimeMinutes).HasPrecision(10, 2);
                entity.Property(e => e.LoadingTimeMinutes).HasPrecision(10, 2);
                entity.Property(e => e.HaulingTimeMinutes).HasPrecision(10, 2);
                entity.Property(e => e.DumpingTimeMinutes).HasPrecision(10, 2);
                entity.Property(e => e.ReturningTimeMinutes).HasPrecision(10, 2);
                entity.Property(e => e.DistanceKM).HasPrecision(18, 4);
                entity.Property(e => e.FuelConsumed).HasPrecision(18, 4);
                entity.Property(e => e.FuelRatioLperKM).HasPrecision(18, 4);
                entity.Property(e => e.FuelRatioLperTonKM).HasPrecision(18, 4);
                entity.Property(e => e.HMStart).HasPrecision(18, 4);
                entity.Property(e => e.HMEnd).HasPrecision(18, 4);
                entity.Property(e => e.RatePerTon).HasPrecision(18, 4);
                entity.Property(e => e.TripRevenue).HasPrecision(18, 4);
                entity.Property(e => e.TripCost).HasPrecision(18, 4);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure RouteMaster
            modelBuilder.Entity<RouteMaster>(entity =>
            {
                entity.ToTable("route_master");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RouteCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RouteName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Site).HasMaxLength(100);
                entity.Property(e => e.OriginPit).HasMaxLength(50);
                entity.Property(e => e.Destination).HasMaxLength(50);
                entity.Property(e => e.DistanceKM).HasPrecision(18, 4).HasColumnName("DistanceKM");
                entity.Property(e => e.EstimatedCycleTime).HasPrecision(18, 4);
                entity.Property(e => e.GradePercent).HasPrecision(18, 4);
                entity.Property(e => e.RoadType).HasMaxLength(50);
                entity.Property(e => e.EstimatedFuelPerTrip).HasPrecision(18, 4);
                entity.Property(e => e.CostPerKM).HasPrecision(18, 4);
                entity.Property(e => e.CostPerTonKM).HasPrecision(18, 4);
                entity.Property(e => e.RatePerTon).HasPrecision(18, 4);
                // P3 additional fields
                entity.Property(e => e.SiteCode).HasMaxLength(30).HasColumnName("SiteCode");
                entity.Property(e => e.OriginLocation).HasMaxLength(30).HasColumnName("OriginLocation");
                entity.Property(e => e.DestinationLocation).HasMaxLength(30).HasColumnName("DestinationLocation");
                entity.Property(e => e.TravelTimeMin).HasPrecision(10, 2).HasColumnName("TravelTimeMin");
                entity.Property(e => e.HaulCostPerKm).HasPrecision(10, 2).HasColumnName("HaulCostPerKm");
                entity.Property(e => e.FuelConsumptionPerKm).HasPrecision(10, 2).HasColumnName("FuelConsumptionPerKm");
                entity.Property(e => e.RouteType).HasMaxLength(20).HasColumnName("RouteType");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure WeighbridgeTicket
            modelBuilder.Entity<WeighbridgeTicket>(entity =>
            {
                entity.ToTable("weighbridge_tickets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TicketDate).HasColumnType("timestamp");
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TicketType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DriverName).HasMaxLength(50);
                entity.Property(e => e.DriverBadge).HasMaxLength(50);
                entity.Property(e => e.FirstWeight).HasPrecision(18, 4);
                entity.Property(e => e.SecondWeight).HasPrecision(18, 4);
                entity.Property(e => e.NetWeight).HasPrecision(18, 4);
                entity.Property(e => e.MaterialType).HasMaxLength(50);
                entity.Property(e => e.OriginPit).HasMaxLength(50);
                entity.Property(e => e.DestinationStockpile).HasMaxLength(50);
                entity.Property(e => e.TareCompensation).HasPrecision(18, 4);
                entity.Property(e => e.FinalNetWeight).HasPrecision(18, 4);
                entity.Property(e => e.VehicleType).HasMaxLength(50);
                entity.Property(e => e.AxleLoad).HasPrecision(18, 4);
                entity.Property(e => e.WeighbridgeOperator).HasMaxLength(50);
                entity.Property(e => e.FirstWeighTime).HasColumnType("timestamp");
                entity.Property(e => e.SecondWeighTime).HasColumnType("timestamp");
                entity.Property(e => e.TripNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure FuelAnalysis
            modelBuilder.Entity<FuelAnalysis>(entity =>
            {
                entity.ToTable("fuel_analyses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RouteCode).HasMaxLength(50);
                entity.Property(e => e.TotalFuelLitres).HasPrecision(18, 4);
                entity.Property(e => e.TotalKM).HasPrecision(18, 4);
                entity.Property(e => e.TotalTonKM).HasPrecision(18, 4);
                entity.Property(e => e.TotalTon).HasPrecision(18, 4);
                entity.Property(e => e.TotalTrips).HasPrecision(18, 4);
                entity.Property(e => e.TotalHours).HasPrecision(18, 4);
                entity.Property(e => e.LitrePerKM).HasPrecision(18, 4);
                entity.Property(e => e.LitrePerTon).HasPrecision(18, 4);
                entity.Property(e => e.LitrePerTonKM).HasPrecision(18, 4);
                entity.Property(e => e.LitrePerHour).HasPrecision(18, 4);
                entity.Property(e => e.FuelCost).HasPrecision(18, 4);
                entity.Property(e => e.FuelCostPerTon).HasPrecision(18, 4);
                entity.Property(e => e.BenchmarkLitrePerKM).HasPrecision(18, 4);
                entity.Property(e => e.VariancePercent).HasPrecision(18, 4);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure UnitCostTracking
            modelBuilder.Entity<UnitCostTracking>(entity =>
            {
                entity.ToTable("unit_cost_tracking");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CostCenter).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.FuelCost).HasPrecision(18, 4);
                entity.Property(e => e.MaintenanceCost).HasPrecision(18, 4);
                entity.Property(e => e.DriverCost).HasPrecision(18, 4);
                entity.Property(e => e.DepreciationCost).HasPrecision(18, 4);
                entity.Property(e => e.TyreCost).HasPrecision(18, 4);
                entity.Property(e => e.OtherCost).HasPrecision(18, 4);
                entity.Property(e => e.TotalCost).HasPrecision(18, 4);
                entity.Property(e => e.TotalTrips).HasPrecision(18, 4);
                entity.Property(e => e.TotalTon).HasPrecision(18, 4);
                entity.Property(e => e.TotalKM).HasPrecision(18, 4);
                entity.Property(e => e.TotalHours).HasPrecision(18, 4);
                entity.Property(e => e.CostPerTrip).HasPrecision(18, 4);
                entity.Property(e => e.CostPerTon).HasPrecision(18, 4);
                entity.Property(e => e.CostPerKM).HasPrecision(18, 4);
                entity.Property(e => e.CostPerHour).HasPrecision(18, 4);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure DriverProductivity
            modelBuilder.Entity<DriverProductivity>(entity =>
            {
                entity.ToTable("driver_productivity");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PeriodMonth).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PeriodYear).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Site).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DriverId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DriverName).HasMaxLength(100);
                entity.Property(e => e.UnitNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalTrips).HasPrecision(18, 4);
                entity.Property(e => e.TargetTrips).HasPrecision(18, 4);
                entity.Property(e => e.AchievementPercent).HasPrecision(18, 4);
                entity.Property(e => e.TotalTon).HasPrecision(18, 4);
                entity.Property(e => e.TotalKM).HasPrecision(18, 4);
                entity.Property(e => e.TotalHours).HasPrecision(18, 4);
                entity.Property(e => e.TotalFuelLitres).HasPrecision(18, 4);
                entity.Property(e => e.FuelEfficiency).HasPrecision(18, 4);
                entity.Property(e => e.LateCount).HasPrecision(18, 4);
                entity.Property(e => e.AccidentCount).HasPrecision(18, 4);
                entity.Property(e => e.ViolationCount).HasPrecision(18, 4);
                entity.Property(e => e.RitaseAllowance).HasPrecision(18, 4);
                entity.Property(e => e.IncentiveAmount).HasPrecision(18, 4);
                entity.Property(e => e.DeductionAmount).HasPrecision(18, 4);
                entity.Property(e => e.TotalPayable).HasPrecision(18, 4);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
