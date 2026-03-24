using HaulingDemoApp.Data;
using HaulingDemoApp.Models;
using HaulingDemoApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on PORT environment variable (Cloud Run)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register GCS Service
builder.Services.AddSingleton<GcsService>();

// Configure Database
var useProvider = builder.Configuration["ConnectionStrings:UseProvider"] ?? "PostgreSQL";

if (useProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    // Allow override from environment variable
    var postgresConnection = Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? builder.Configuration.GetConnectionString("PostgreSQL");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(postgresConnection));
}
else if (useProvider.Equals("MySQL", StringComparison.OrdinalIgnoreCase))
{
    var mysqlConnection = builder.Configuration.GetConnectionString("MySQL");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySQL(mysqlConnection));
}

var app = builder.Build();

// =====================================================
// P3 MIGRATION ENDPOINT (run to create vehicle/driver tables)
// =====================================================
app.MapPost("/api/migrate/p3-master", async (AppDbContext db) =>
{
    var results = new List<string>();
    try
    {
        // vehicle_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""vehicle_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""VehicleCode"" VARCHAR(30) NOT NULL,
                ""PoliceNumber"" VARCHAR(30),
                ""VehicleType"" VARCHAR(20) NOT NULL,
                ""Brand"" VARCHAR(30),
                ""Model"" VARCHAR(30),
                ""FuelType"" VARCHAR(20),
                ""SiteCode"" VARCHAR(30),
                ""CostCenter"" VARCHAR(30),
                ""CapacityVolume"" INT,
                ""CapacityWeight"" INT,
                ""YearMade"" INT,
                ""ChassisNumber"" VARCHAR(30),
                ""MachineNumber"" VARCHAR(30),
                ""Status"" VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(100),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("vehicle_master OK");

        // driver_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""driver_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""DriverCode"" VARCHAR(30) NOT NULL,
                ""FullName"" VARCHAR(100) NOT NULL,
                ""NIK"" VARCHAR(20),
                ""SIM"" VARCHAR(20),
                ""SIMType"" VARCHAR(20) NOT NULL DEFAULT 'B2',
                ""DateOfBirth"" DATE,
                ""Gender"" VARCHAR(10),
                ""Phone"" VARCHAR(50),
                ""Address"" VARCHAR(100),
                ""SiteCode"" VARCHAR(30),
                ""DepartmentCode"" VARCHAR(30),
                ""JoinDate"" DATE,
                ""Status"" VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(50),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("driver_master OK");

        // Seed Vehicle
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""vehicle_master"" (""VehicleCode"",""PoliceNumber"",""VehicleType"",""Brand"",""Model"",""FuelType"",""SiteCode"",""CostCenter"",""CapacityVolume"",""CapacityWeight"",""YearMade"",""ChassisNumber"",""MachineNumber"",""Status"") SELECT 'VH-TNG-001','B 1234 XYZ','HAULING','HINO','FM 260 JD','DIESEL','TNG','CC-OPS-TNG',16,30,2018,'WKB1JWGS001','KD15V001','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""vehicle_master"" WHERE ""VehicleCode"" = 'VH-TNG-001');
            INSERT INTO ""vehicle_master"" (""VehicleCode"",""PoliceNumber"",""VehicleType"",""Brand"",""Model"",""FuelType"",""SiteCode"",""CostCenter"",""CapacityVolume"",""CapacityWeight"",""YearMade"",""ChassisNumber"",""MachineNumber"",""Status"") SELECT 'VH-TNG-002','B 5678 XYZ','HAULING','SCANIA','G 480','DIESEL','TNG','CC-OPS-TNG',20,35,2019,'WKB2JWGS002','KD15V002','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""vehicle_master"" WHERE ""VehicleCode"" = 'VH-TNG-002');
            INSERT INTO ""vehicle_master"" (""VehicleCode"",""PoliceNumber"",""VehicleType"",""Brand"",""Model"",""FuelType"",""SiteCode"",""CostCenter"",""CapacityVolume"",""CapacityWeight"",""YearMade"",""ChassisNumber"",""MachineNumber"",""Status"") SELECT 'VH-SDK-001','B 9012 XYZ','HAULING','HINO','FM 260 JD','DIESEL','SDK','CC-OPS-SDK',16,30,2020,'WKB3JWGS003','KD15V003','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""vehicle_master"" WHERE ""VehicleCode"" = 'VH-SDK-001');
            INSERT INTO ""vehicle_master"" (""VehicleCode"",""PoliceNumber"",""VehicleType"",""Brand"",""Model"",""FuelType"",""SiteCode"",""CostCenter"",""CapacityVolume"",""CapacityWeight"",""YearMade"",""ChassisNumber"",""MachineNumber"",""Status"") SELECT 'VH-TNG-FT1','B FUEL XYZ','FUEL_TANKER','FUSO','FM 517','DIESEL','TNG','CC-OPS-TNG',10,15,2017,'WKB4JWSFT001','KD15F001','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""vehicle_master"" WHERE ""VehicleCode"" = 'VH-TNG-FT1');
            INSERT INTO ""vehicle_master"" (""VehicleCode"",""PoliceNumber"",""VehicleType"",""Brand"",""Model"",""FuelType"",""SiteCode"",""CostCenter"",""CapacityVolume"",""CapacityWeight"",""YearMade"",""ChassisNumber"",""MachineNumber"",""Status"") SELECT 'VH-SBB-001','B 3456 XYZ','HAULING','HINO','FM 260 JD','DIESEL','SBB','CC-OPS-TNG',16,30,2021,'WKB5JWGS005','KD15V005','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""vehicle_master"" WHERE ""VehicleCode"" = 'VH-SBB-001');
        ");
        results.Add("vehicle_master seed OK");

        // Seed Driver
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""driver_master"" (""DriverCode"",""FullName"",""NIK"",""SIM"",""SIMType"",""Gender"",""Phone"",""SiteCode"",""DepartmentCode"",""Status"") SELECT 'DRV-001','Ahmad Saputra','1234567890123456','SIM123456','B2','LAKI-LAKI','081234567890','TNG','DEPT-HAUL','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""driver_master"" WHERE ""DriverCode"" = 'DRV-001');
            INSERT INTO ""driver_master"" (""DriverCode"",""FullName"",""NIK"",""SIM"",""SIMType"",""Gender"",""Phone"",""SiteCode"",""DepartmentCode"",""Status"") SELECT 'DRV-002','Budi Santoso','2345678901234567','SIM234567','B2','LAKI-LAKI','081234567891','TNG','DEPT-HAUL','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""driver_master"" WHERE ""DriverCode"" = 'DRV-002');
            INSERT INTO ""driver_master"" (""DriverCode"",""FullName"",""NIK"",""SIM"",""SIMType"",""Gender"",""Phone"",""SiteCode"",""DepartmentCode"",""Status"") SELECT 'DRV-003','Dewi Lestari','3456789012345678','SIM345678','B2','PEREMPUAN','081234567892','SDK','DEPT-HAUL','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""driver_master"" WHERE ""DriverCode"" = 'DRV-003');
            INSERT INTO ""driver_master"" (""DriverCode"",""FullName"",""NIK"",""SIM"",""SIMType"",""Gender"",""Phone"",""SiteCode"",""DepartmentCode"",""Status"") SELECT 'DRV-004','Eko Prasetyo','4567890123456789','SIM456789','B1','LAKI-LAKI','081234567893','TNG','DEPT-HAUL','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""driver_master"" WHERE ""DriverCode"" = 'DRV-004');
        ");
        results.Add("driver_master seed OK");

        return Results.Ok(new { status = "P3 migration completed", results });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// =====================================================
// P4 MIGRATION ENDPOINT (run this first to add vendor columns)
// =====================================================
app.MapPost("/api/migrate/p4-master", async (AppDbContext db) =>
{
    var results = new List<string>();
    try
    {
        // Add new columns to vendors table
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""VendorType"" VARCHAR(10) DEFAULT 'SUPPLIER';
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""Province"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""PostalCode"" VARCHAR(20);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""Country"" VARCHAR(50) DEFAULT 'Indonesia';
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""Fax"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""ContactPhone"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""NIB"" VARCHAR(30);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""BankName"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""BankAccountName"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""BankAccountNumber"" VARCHAR(50);
            ALTER TABLE ""vendors"" ADD COLUMN IF NOT EXISTS ""BankBranch"" VARCHAR(50);
        ");
        results.Add("vendors P4 columns OK");

        // Add new columns to route_master
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""SiteCode"" VARCHAR(30);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""OriginLocation"" VARCHAR(30);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""DestinationLocation"" VARCHAR(30);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""TravelTimeMin"" DECIMAL(10,2);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""HaulCostPerKm"" DECIMAL(10,2);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""FuelConsumptionPerKm"" DECIMAL(10,2);
            ALTER TABLE ""route_master"" ADD COLUMN IF NOT EXISTS ""RouteType"" VARCHAR(20) DEFAULT 'HAUL';
        ");
        results.Add("route_master P3 columns OK");

        // tax_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""tax_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""TaxCode"" VARCHAR(20) NOT NULL,
                ""TaxName"" VARCHAR(100) NOT NULL,
                ""TaxType"" VARCHAR(20) NOT NULL DEFAULT 'VAT',
                ""TaxRate"" DECIMAL(5,2) NOT NULL,
                ""TaxRateType"" VARCHAR(10) NOT NULL DEFAULT 'PERCENTAGE',
                ""FixedAmount"" DECIMAL(18,2),
                ""CoaCode"" VARCHAR(30),
                ""TaxBase"" VARCHAR(10) NOT NULL DEFAULT 'EXCLUSIVE',
                ""ApplicableTo"" VARCHAR(20) DEFAULT 'ALL',
                ""Status"" VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("tax_master OK");

        // coa_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""coa_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""AccountCode"" VARCHAR(30) NOT NULL,
                ""AccountName"" VARCHAR(200) NOT NULL,
                ""AccountType"" VARCHAR(10) NOT NULL DEFAULT 'EXPENSE',
                ""AccountCategory"" VARCHAR(20) NOT NULL DEFAULT 'OPERATIONAL',
                ""AccountLevel"" INT NOT NULL DEFAULT 5,
                ""ParentAccountCode"" VARCHAR(30),
                ""NormalBalance"" VARCHAR(10) NOT NULL DEFAULT 'DEBIT',
                ""CostCenterRequired"" VARCHAR(10) DEFAULT 'N',
                ""SiteCode"" VARCHAR(30),
                ""TaxCode"" VARCHAR(20),
                ""Currency"" VARCHAR(20) NOT NULL DEFAULT 'IDR',
                ""Description"" VARCHAR(50),
                ""Status"" VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("coa_master OK");

        // Seed Tax
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'PPN-11','Pajak Pertambahan Nilai 11%','VAT',11.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'PPN-11');
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'PPN-12','Pajak Pertambahan Nilai 12%','VAT',12.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'PPN-12');
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'PPH-21-2','PPh Pasal 21 2%','INCOME_TAX',2.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'PPH-21-2');
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'PPH-23-2','PPh Pasal 23 2%','INCOME_TAX',2.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'PPH-23-2');
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'PPH-4-2','PPh Final Pasal 4 Ayat 2 10%','INCOME_TAX',10.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'PPH-4-2');
            INSERT INTO ""tax_master"" (""TaxCode"",""TaxName"",""TaxType"",""TaxRate"",""TaxRateType"",""TaxBase"",""Status"") SELECT 'NO-TAX','Tanpa Pajak / Non-Taxable','NONE',0.00,'PERCENTAGE','EXCLUSIVE','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""tax_master"" WHERE ""TaxCode"" = 'NO-TAX');
        ");
        results.Add("tax_master seed OK");

        // Seed COA
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '1-1000','ASET LANCAR','ASSET','OPERATIONAL',1,'DEBIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '1-1000');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '1-1100','Kas & Bank','ASSET','OPERATIONAL',2,'1-1000','DEBIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '1-1100');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '1-1200','Piutang Usaha','ASSET','OPERATIONAL',2,'1-1000','DEBIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '1-1200');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '2-2000','KEWAJIBAN LANCAR','LIABILITY','OPERATIONAL',1,'CREDIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '2-2000');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '2-2100','Hutang Usaha','LIABILITY','OPERATIONAL',2,'2-2000','CREDIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '2-2100');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '4-4000','PENDAPATAN OPERASIONAL','REVENUE','OPERATIONAL',1,'CREDIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '4-4000');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '4-4100','Pendapatan Hauling','REVENUE','OPERATIONAL',2,'4-4000','CREDIT','N','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '4-4100');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '5-5000','BEBAN OPERASIONAL','EXPENSE','OPERATIONAL',1,'DEBIT','Y','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '5-5000');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '5-5100','Beban BBM & Pelumas','EXPENSE','OPERATIONAL',2,'5-5000','DEBIT','Y','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '5-5100');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '5-5200','Beban Gaji Driver','EXPENSE','OPERATIONAL',2,'5-5000','DEBIT','Y','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '5-5200');
            INSERT INTO ""coa_master"" (""AccountCode"",""AccountName"",""AccountType"",""AccountCategory"",""AccountLevel"",""ParentAccountCode"",""NormalBalance"",""CostCenterRequired"",""Status"") SELECT '5-5300','Beban Perawatan Unit','EXPENSE','OPERATIONAL',2,'5-5000','DEBIT','Y','ACTIVE' WHERE NOT EXISTS (SELECT 1 FROM ""coa_master"" WHERE ""AccountCode"" = '5-5300');
        ");
        results.Add("coa_master seed OK");

        return Results.Ok(new { status = "P4 migration completed", results });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// =====================================================
// DATABASE MIGRATION ENDPOINT
// =====================================================
app.MapPost("/api/migrate/create-missing-tables", async (AppDbContext db) =>
{
    var results = new List<string>();
    try
    {
        // fleet_vehicles
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""fleet_vehicles"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""UnitDescription"" VARCHAR(255) NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""MerkType"" VARCHAR(100),
                ""Category"" VARCHAR(50),
                ""LicensePlate"" VARCHAR(50),
                ""ChassisNumber"" VARCHAR(50),
                ""EngineNumber"" VARCHAR(50),
                ""VehicleType"" VARCHAR(50),
                ""FuelType"" VARCHAR(50),
                ""GrossWeight"" DECIMAL(18,4),
                ""TareWeight"" DECIMAL(18,4),
                ""PayloadCapacity"" DECIMAL(18,4),
                ""MaxPayload"" DECIMAL(18,4),
                ""FuelTankCapacity"" DECIMAL(18,4),
                ""AvgFuelConsumption"" DECIMAL(18,4),
                ""BenchmarkLitrePerKM"" DECIMAL(18,4),
                ""BenchmarkLitrePerHour"" DECIMAL(18,4),
                ""FuelCardNumber"" VARCHAR(50),
                ""TyreSize"" VARCHAR(50),
                ""TyreQuantity"" INT DEFAULT 0,
                ""TyreCostPerUnit"" DECIMAL(18,4),
                ""AvgTyreLifeKM"" DECIMAL(18,4),
                ""PemasanganDate"" TIMESTAMP,
                ""HMAwal"" DECIMAL(18,4) DEFAULT 0,
                ""KMAwal"" DECIMAL(18,4) DEFAULT 0,
                ""HMakhir"" DECIMAL(18,4) DEFAULT 0,
                ""KMakhir"" DECIMAL(18,4) DEFAULT 0,
                ""TotalJam"" DECIMAL(18,4) DEFAULT 0,
                ""TotalKM"" DECIMAL(18,4) DEFAULT 0,
                ""FuelRatio"" DECIMAL(18,4) DEFAULT 0,
                ""AvgHMHari"" DECIMAL(18,4) DEFAULT 0,
                ""AvgKMHari"" DECIMAL(18,4) DEFAULT 0,
                ""HMUsage"" DECIMAL(18,4) DEFAULT 0,
                ""TotalFuel"" DECIMAL(18,4) DEFAULT 0,
                ""CostCenter"" VARCHAR(50),
                ""RouteCode"" VARCHAR(50),
                ""AssignedDriverId"" VARCHAR(50),
                ""AcquisitionCost"" DECIMAL(18,4),
                ""DepreciationRate"" DECIMAL(18,4),
                ""AccumulatedDepreciation"" DECIMAL(18,4),
                ""BookValue"" DECIMAL(18,4),
                ""UsefulLifeYear"" VARCHAR(50),
                ""InsuranceExpiry"" TIMESTAMP,
                ""TaxExpiry"" TIMESTAMP,
                ""KIRExpiry"" TIMESTAMP,
                ""STNKExpiry"" TIMESTAMP,
                ""Status"" VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(255) DEFAULT '',
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("fleet_vehicles OK");

        // haul_trips
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""haul_trips"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""TripNumber"" VARCHAR(50),
                ""TripDate"" DATE NOT NULL,
                ""Site"" VARCHAR(100),
                ""UnitNo"" VARCHAR(50),
                ""DriverId"" VARCHAR(50),
                ""DriverName"" VARCHAR(100),
                ""RouteCode"" VARCHAR(50),
                ""RouteName"" VARCHAR(100),
                ""Shift"" VARCHAR(20),
                ""MaterialType"" VARCHAR(50),
                ""OriginPit"" VARCHAR(100),
                ""DestinationStockpile"" VARCHAR(100),
                ""WBTicketIn"" VARCHAR(50),
                ""WBTicketOut"" VARCHAR(50),
                ""GrossWeight"" DECIMAL(18,4),
                ""TareWeight"" DECIMAL(18,4),
                ""NetWeight"" DECIMAL(18,4),
                ""PayloadTon"" DECIMAL(18,4),
                ""StartTime"" TIMESTAMP,
                ""EndTime"" TIMESTAMP,
                ""CycleTimeMinutes"" DECIMAL(18,4),
                ""LoadingTimeMinutes"" DECIMAL(18,4),
                ""HaulingTimeMinutes"" DECIMAL(18,4),
                ""DumpingTimeMinutes"" DECIMAL(18,4),
                ""ReturningTimeMinutes"" DECIMAL(18,4),
                ""DistanceKM"" DECIMAL(18,4),
                ""FuelConsumed"" DECIMAL(18,4),
                ""FuelRatioLperKM"" DECIMAL(18,4),
                ""FuelRatioLperTonKM"" DECIMAL(18,4),
                ""HMStart"" DECIMAL(18,4),
                ""HMEnd"" DECIMAL(18,4),
                ""RatePerTon"" DECIMAL(18,4),
                ""TripRevenue"" DECIMAL(18,2),
                ""TripCost"" DECIMAL(18,2),
                ""Status"" VARCHAR(50),
                ""IsWBValidated"" BOOLEAN DEFAULT FALSE,
                ""IsRevenuePosted"" BOOLEAN DEFAULT FALSE,
                ""Remarks"" VARCHAR(500),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("haul_trips OK");

        // route_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""route_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""RouteCode"" VARCHAR(50) NOT NULL,
                ""RouteName"" VARCHAR(100),
                ""Site"" VARCHAR(100),
                ""OriginPit"" VARCHAR(100),
                ""Destination"" VARCHAR(100),
                ""DistanceKM"" DECIMAL(18,4),
                ""EstimatedCycleTime"" DECIMAL(18,4),
                ""GradePercent"" DECIMAL(18,4),
                ""RoadType"" VARCHAR(50),
                ""EstimatedFuelPerTrip"" DECIMAL(18,4),
                ""CostPerKM"" DECIMAL(18,4),
                ""CostPerTonKM"" DECIMAL(18,4),
                ""RatePerTon"" DECIMAL(18,4),
                ""Remarks"" VARCHAR(255),
                ""Status"" VARCHAR(50) DEFAULT 'ACTIVE',
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("route_master OK");

        // weighbridge_tickets
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""weighbridge_tickets"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""TicketNumber"" VARCHAR(50) NOT NULL,
                ""TicketDate"" DATE NOT NULL,
                ""Site"" VARCHAR(100),
                ""TicketType"" VARCHAR(20),
                ""UnitNo"" VARCHAR(50),
                ""DriverName"" VARCHAR(100),
                ""DriverBadge"" VARCHAR(50),
                ""FirstWeight"" DECIMAL(18,4),
                ""SecondWeight"" DECIMAL(18,4),
                ""NetWeight"" DECIMAL(18,4),
                ""MaterialType"" VARCHAR(50),
                ""OriginPit"" VARCHAR(100),
                ""DestinationStockpile"" VARCHAR(100),
                ""TareCompensation"" DECIMAL(18,4),
                ""FinalNetWeight"" DECIMAL(18,4),
                ""VehicleType"" VARCHAR(50),
                ""AxleLoad"" DECIMAL(18,4),
                ""WeighbridgeOperator"" VARCHAR(100),
                ""FirstWeighTime"" TIMESTAMP,
                ""SecondWeighTime"" TIMESTAMP,
                ""TripNumber"" VARCHAR(50),
                ""IsLinked"" BOOLEAN DEFAULT FALSE,
                ""Status"" VARCHAR(50),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("weighbridge_tickets OK");

        // fuel_analyses
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""fuel_analyses"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""Site"" VARCHAR(100),
                ""PeriodMonth"" VARCHAR(10),
                ""PeriodYear"" VARCHAR(10),
                ""TotalFuelLitre"" DECIMAL(18,4),
                ""TotalDistanceKM"" DECIMAL(18,4),
                ""TotalPayloadTon"" DECIMAL(18,4),
                ""TotalTonKM"" DECIMAL(18,4),
                ""TotalHours"" DECIMAL(18,4),
                ""LitrePerKM"" DECIMAL(18,4),
                ""LitrePerTon"" DECIMAL(18,4),
                ""LitrePerTonKM"" DECIMAL(18,4),
                ""LitrePerHour"" DECIMAL(18,4),
                ""BenchmarkLitrePerKM"" DECIMAL(18,4),
                ""BenchmarkLitrePerTonKM"" DECIMAL(18,4),
                ""VariancePercent"" DECIMAL(18,4),
                ""Status"" VARCHAR(50),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("fuel_analyses OK");

        // unit_cost_trackings
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""unit_cost_trackings"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""Site"" VARCHAR(100),
                ""PeriodMonth"" VARCHAR(10),
                ""PeriodYear"" VARCHAR(10),
                ""FuelCost"" DECIMAL(18,2),
                ""MaintenanceCost"" DECIMAL(18,2),
                ""DriverCost"" DECIMAL(18,2),
                ""DepreciationCost"" DECIMAL(18,2),
                ""TyreCost"" DECIMAL(18,2),
                ""OtherCost"" DECIMAL(18,2),
                ""TotalCost"" DECIMAL(18,2),
                ""TotalTrips"" INT,
                ""TotalTon"" DECIMAL(18,4),
                ""TotalKM"" DECIMAL(18,4),
                ""TotalHours"" DECIMAL(18,4),
                ""CostPerTrip"" DECIMAL(18,2),
                ""CostPerTon"" DECIMAL(18,4),
                ""CostPerKM"" DECIMAL(18,4),
                ""CostPerHour"" DECIMAL(18,4),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("unit_cost_trackings OK");

        // driver_productivities
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""driver_productivities"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""DriverId"" VARCHAR(50) NOT NULL,
                ""DriverName"" VARCHAR(100),
                ""Site"" VARCHAR(100),
                ""PeriodMonth"" VARCHAR(10),
                ""PeriodYear"" VARCHAR(10),
                ""TotalTrips"" INT,
                ""TotalRitase"" INT,
                ""TotalTon"" DECIMAL(18,4),
                ""TotalKM"" DECIMAL(18,4),
                ""TargetTrips"" INT,
                ""TargetRitase"" INT,
                ""AchievementPercent"" DECIMAL(18,4),
                ""RitaseRate"" DECIMAL(18,2),
                ""RitaseAllowance"" DECIMAL(18,2),
                ""IncentiveAmount"" DECIMAL(18,2),
                ""DeductionAmount"" DECIMAL(18,2),
                ""TotalPayable"" DECIMAL(18,2),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("driver_productivities OK");

        // ==================== OLD TABLES ====================
        // hr_employees
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""hr_employees"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""EmployeeCode"" VARCHAR(50) NOT NULL,
                ""FullName"" VARCHAR(100) NOT NULL,
                ""NickName"" VARCHAR(100),
                ""PIN"" VARCHAR(50),
                ""RFID"" VARCHAR(50),
                ""Gender"" VARCHAR(10),
                ""BirthDate"" TIMESTAMP,
                ""BirthPlace"" VARCHAR(100),
                ""Religion"" VARCHAR(50),
                ""MaritalStatus"" VARCHAR(50),
                ""Address"" VARCHAR(255),
                ""Phone"" VARCHAR(50),
                ""Email"" VARCHAR(100),
                ""EmergencyContact"" VARCHAR(100),
                ""EmergencyPhone"" VARCHAR(100),
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100) NOT NULL,
                ""Position"" VARCHAR(100) NOT NULL,
                ""Level"" VARCHAR(50),
                ""EmployeeType"" VARCHAR(50),
                ""Status"" VARCHAR(50) DEFAULT 'ACTIVE',
                ""JoinDate"" TIMESTAMP,
                ""ResignDate"" TIMESTAMP,
                ""PhotoUrl"" VARCHAR(255),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("hr_employees OK");

        // chart_of_accounts
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""chart_of_accounts"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""AccountCode"" VARCHAR(20) NOT NULL,
                ""AccountName"" VARCHAR(200) NOT NULL,
                ""AccountType"" VARCHAR(50) NOT NULL,
                ""ParentAccountCode"" VARCHAR(20),
                ""NormalBalance"" VARCHAR(10) DEFAULT 'DEBIT',
                ""CostCenterRequired"" VARCHAR(50),
                ""TaxCode"" VARCHAR(50),
                ""Currency"" VARCHAR(50) DEFAULT 'IDR',
                ""OpeningBalance"" DECIMAL(18,2) DEFAULT 0,
                ""CurrentBalance"" DECIMAL(18,2) DEFAULT 0,
                ""IsActive"" BOOLEAN DEFAULT TRUE,
                ""Remarks"" VARCHAR(100),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("chart_of_accounts OK");

        // budgets
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""budgets"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""BudgetNumber"" VARCHAR(50) NOT NULL,
                ""PeriodMonth"" VARCHAR(10) NOT NULL,
                ""PeriodYear"" VARCHAR(4) NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100) NOT NULL,
                ""Division"" VARCHAR(100) NOT NULL,
                ""AccountCode"" VARCHAR(50) NOT NULL,
                ""AccountName"" VARCHAR(200),
                ""PlannedAmount"" DECIMAL(18,2) DEFAULT 0,
                ""ActualAmount"" DECIMAL(18,2) DEFAULT 0,
                ""CommittedAmount"" DECIMAL(18,2) DEFAULT 0,
                ""AvailableBudget"" DECIMAL(18,2) DEFAULT 0,
                ""UtilizationPercent"" DECIMAL(5,2) DEFAULT 0,
                ""Status"" VARCHAR(50) DEFAULT 'DRAFT',
                ""Remarks"" VARCHAR(255),
                ""ApprovedDate"" TIMESTAMP,
                ""ApprovedBy"" VARCHAR(100),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("budgets OK");

        // journal_entries
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""journal_entries"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""EntryNumber"" VARCHAR(50) NOT NULL,
                ""EntryDate"" TIMESTAMP NOT NULL,
                ""PeriodMonth"" VARCHAR(10) NOT NULL,
                ""PeriodYear"" VARCHAR(4) NOT NULL,
                ""EntryType"" VARCHAR(50) NOT NULL,
                ""SourceModule"" VARCHAR(100),
                ""SourceId"" VARCHAR(50),
                ""Description"" VARCHAR(255),
                ""Status"" VARCHAR(50) DEFAULT 'DRAFT',
                ""TotalDebit"" DECIMAL(18,2) DEFAULT 0,
                ""TotalCredit"" DECIMAL(18,2) DEFAULT 0,
                ""PostedBy"" VARCHAR(100),
                ""PostedAt"" TIMESTAMP,
                ""CreatedBy"" VARCHAR(100),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("journal_entries OK");

        // journal_lines
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""journal_lines"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""JournalEntryId"" INT NOT NULL,
                ""AccountCode"" VARCHAR(50) NOT NULL,
                ""AccountName"" VARCHAR(200),
                ""DC"" VARCHAR(10) DEFAULT 'D',
                ""Amount"" DECIMAL(18,2) DEFAULT 0,
                ""CostCenter"" VARCHAR(100),
                ""Site"" VARCHAR(100),
                ""Description"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("journal_lines OK");

        // production_data
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""production_data"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PeriodMonth"" VARCHAR(10) NOT NULL,
                ""PeriodYear"" VARCHAR(4) NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Division"" VARCHAR(100) NOT NULL,
                ""TotalTonase"" DECIMAL(18,2) DEFAULT 0,
                ""TotalOperatingHours"" DECIMAL(18,2) DEFAULT 0,
                ""TotalOverburden"" DECIMAL(18,2) DEFAULT 0,
                ""HaulingDistance"" DECIMAL(18,2) DEFAULT 0,
                ""TotalCost"" DECIMAL(18,2) DEFAULT 0,
                ""CostPerTon"" DECIMAL(18,2) DEFAULT 0,
                ""FuelCost"" DECIMAL(18,2) DEFAULT 0,
                ""MaintenanceCost"" DECIMAL(18,2) DEFAULT 0,
                ""LaborCost"" DECIMAL(18,2) DEFAULT 0,
                ""OtherCost"" DECIMAL(18,2) DEFAULT 0,
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("production_data OK");

        // hr_attendance
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""hr_attendance"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""EmployeeCode"" VARCHAR(50) NOT NULL,
                ""EmployeeName"" VARCHAR(100) NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100) NOT NULL,
                ""AttendanceDate"" DATE NOT NULL,
                ""CheckIn"" TIMESTAMP,
                ""CheckOut"" TIMESTAMP,
                ""Shift"" VARCHAR(50),
                ""Status"" VARCHAR(50),
                ""WorkingHours"" DECIMAL(18,4),
                ""OvertimeHours"" DECIMAL(18,4),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("hr_attendance OK");

        // hr_payroll
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""hr_payroll"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PayrollNumber"" VARCHAR(50) NOT NULL,
                ""PayrollDate"" TIMESTAMP,
                ""PeriodMonth"" VARCHAR(10) NOT NULL,
                ""PeriodYear"" VARCHAR(4) NOT NULL,
                ""EmployeeCode"" VARCHAR(50) NOT NULL,
                ""EmployeeName"" VARCHAR(100) NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100) NOT NULL,
                ""Position"" VARCHAR(100) NOT NULL,
                ""BasicSalary"" DECIMAL(18,2) DEFAULT 0,
                ""Allowance"" DECIMAL(18,2) DEFAULT 0,
                ""Overtime"" DECIMAL(18,2) DEFAULT 0,
                ""Bonus"" DECIMAL(18,2) DEFAULT 0,
                ""TotalEarning"" DECIMAL(18,2) DEFAULT 0,
                ""AbsenceDeduction"" DECIMAL(18,2) DEFAULT 0,
                ""TaxDeduction"" DECIMAL(18,2) DEFAULT 0,
                ""InsuranceDeduction"" DECIMAL(18,2) DEFAULT 0,
                ""OtherDeduction"" DECIMAL(18,2) DEFAULT 0,
                ""TotalDeduction"" DECIMAL(18,2) DEFAULT 0,
                ""NetSalary"" DECIMAL(18,2) DEFAULT 0,
                ""Status"" VARCHAR(50) DEFAULT 'DRAFT',
                ""Remarks"" VARCHAR(255),
                ""PaidDate"" TIMESTAMP,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("hr_payroll OK");

        // work_orders
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""work_orders"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""WONumber"" VARCHAR(50) NOT NULL,
                ""WODate"" TIMESTAMP,
                ""Site"" VARCHAR(100) NOT NULL,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""MerkType"" VARCHAR(100),
                ""Category"" VARCHAR(50),
                ""WOType"" VARCHAR(50) DEFAULT 'PREVENTIVE',
                ""Priority"" VARCHAR(50) DEFAULT 'MEDIUM',
                ""Problem"" VARCHAR(255),
                ""Cause"" VARCHAR(255),
                ""ActionTaken"" VARCHAR(255),
                ""Status"" VARCHAR(50) DEFAULT 'OPEN',
                ""ScheduledDate"" TIMESTAMP,
                ""StartDate"" TIMESTAMP,
                ""EndDate"" TIMESTAMP,
                ""EstimatedCost"" DECIMAL(18,2),
                ""ActualCost"" DECIMAL(18,2),
                ""AssignedTo"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("work_orders OK");

        // preventive_maintenance
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""preventive_maintenance"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PMNumber"" VARCHAR(50) NOT NULL,
                ""PMDate"" TIMESTAMP,
                ""Site"" VARCHAR(100) NOT NULL,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""MerkType"" VARCHAR(100),
                ""PMType"" VARCHAR(50) DEFAULT 'DAILY',
                ""Description"" VARCHAR(255),
                ""Status"" VARCHAR(50) DEFAULT 'SCHEDULED',
                ""ScheduledDate"" TIMESTAMP,
                ""StartDate"" TIMESTAMP,
                ""EndDate"" TIMESTAMP,
                ""HMValue"" DECIMAL(18,2),
                ""NextHMValue"" DECIMAL(18,2),
                ""AssignedTo"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("preventive_maintenance OK");

        // corrective_maintenance
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""corrective_maintenance"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""CMNumber"" VARCHAR(50) NOT NULL,
                ""CMDate"" TIMESTAMP,
                ""Site"" VARCHAR(100) NOT NULL,
                ""UnitNo"" VARCHAR(50) NOT NULL,
                ""MerkType"" VARCHAR(100),
                ""Category"" VARCHAR(50),
                ""CMType"" VARCHAR(50) DEFAULT 'CORRECTIVE',
                ""Priority"" VARCHAR(50) DEFAULT 'MEDIUM',
                ""Problem"" VARCHAR(255) NOT NULL,
                ""RootCause"" VARCHAR(255),
                ""Solution"" VARCHAR(255),
                ""Status"" VARCHAR(50) DEFAULT 'REPORTED',
                ""BreakdownStart"" TIMESTAMP,
                ""BreakdownEnd"" TIMESTAMP,
                ""DowntimeHours"" DECIMAL(18,2),
                ""RepairCost"" DECIMAL(18,2),
                ""PartsCost"" DECIMAL(18,2),
                ""LaborCost"" DECIMAL(18,2),
                ""ReportedBy"" VARCHAR(100),
                ""AssignedTo"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("corrective_maintenance OK");

        // purchase_requests
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""purchase_requests"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PRNumber"" VARCHAR(50) NOT NULL,
                ""PRDate"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100) NOT NULL,
                ""RequestedBy"" VARCHAR(100),
                ""ApprovedBy"" VARCHAR(100),
                ""Status"" VARCHAR(50) DEFAULT 'DRAFT',
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("purchase_requests OK");

        // purchase_request_items
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""purchase_request_items"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PurchaseRequestId"" INT NOT NULL,
                ""PartNumber"" VARCHAR(50) NOT NULL,
                ""Description"" VARCHAR(255) NOT NULL,
                ""Unit"" VARCHAR(20) NOT NULL,
                ""Quantity"" DECIMAL(18,2) DEFAULT 0,
                ""EstimatedPrice"" DECIMAL(18,2) DEFAULT 0,
                ""TotalPrice"" DECIMAL(18,2) DEFAULT 0,
                ""Purpose"" VARCHAR(100),
                ""Priority"" VARCHAR(50) DEFAULT 'NORMAL',
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("purchase_request_items OK");

        // purchase_orders
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""purchase_orders"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PONumber"" VARCHAR(50) NOT NULL,
                ""PODate"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Vendor"" VARCHAR(200) NOT NULL,
                ""VendorCode"" VARCHAR(50),
                ""PRNumber"" VARCHAR(50),
                ""Status"" VARCHAR(50) DEFAULT 'DRAFT',
                ""DeliveryDate"" TIMESTAMP,
                ""DeliveryAddress"" VARCHAR(100),
                ""SubTotal"" DECIMAL(18,2) DEFAULT 0,
                ""Tax"" DECIMAL(18,2) DEFAULT 0,
                ""Discount"" DECIMAL(18,2) DEFAULT 0,
                ""TotalAmount"" DECIMAL(18,2) DEFAULT 0,
                ""PaymentTerms"" VARCHAR(50),
                ""Remarks"" VARCHAR(255),
                ""ApprovedBy"" VARCHAR(100),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("purchase_orders OK");

        // purchase_order_items
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""purchase_order_items"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""PurchaseOrderId"" INT NOT NULL,
                ""PartNumber"" VARCHAR(50) NOT NULL,
                ""Description"" VARCHAR(255) NOT NULL,
                ""Unit"" VARCHAR(20) NOT NULL,
                ""Quantity"" DECIMAL(18,2) DEFAULT 0,
                ""UnitPrice"" DECIMAL(18,2) DEFAULT 0,
                ""TotalPrice"" DECIMAL(18,2) DEFAULT 0,
                ""DeliveredQty"" DECIMAL(18,2) DEFAULT 0,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("purchase_order_items OK");

        // good_receipts
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""good_receipts"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""GRNumber"" VARCHAR(50) NOT NULL,
                ""GRDate"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Vendor"" VARCHAR(200) NOT NULL,
                ""PONumber"" VARCHAR(50),
                ""Status"" VARCHAR(50) DEFAULT 'RECEIVED',
                ""ReceivedBy"" VARCHAR(100),
                ""ApprovedBy"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("good_receipts OK");

        // good_receipt_items
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""good_receipt_items"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""GoodReceiptId"" INT NOT NULL,
                ""PartNumber"" VARCHAR(50) NOT NULL,
                ""Description"" VARCHAR(255) NOT NULL,
                ""Unit"" VARCHAR(20) NOT NULL,
                ""OrderedQty"" DECIMAL(18,2) DEFAULT 0,
                ""ReceivedQty"" DECIMAL(18,2) DEFAULT 0,
                ""AcceptedQty"" DECIMAL(18,2) DEFAULT 0,
                ""RejectedQty"" DECIMAL(18,2) DEFAULT 0,
                ""Location"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("good_receipt_items OK");

        // good_issues
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""good_issues"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""GINumber"" VARCHAR(50) NOT NULL,
                ""GIDate"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Department"" VARCHAR(100),
                ""RequestNumber"" VARCHAR(50),
                ""Status"" VARCHAR(50) DEFAULT 'ISSUED',
                ""IssuedBy"" VARCHAR(100),
                ""ReceivedBy"" VARCHAR(100),
                ""ApprovedBy"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("good_issues OK");

        // good_issue_items
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""good_issue_items"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""GoodIssueId"" INT NOT NULL,
                ""PartNumber"" VARCHAR(50) NOT NULL,
                ""Description"" VARCHAR(255) NOT NULL,
                ""Unit"" VARCHAR(20) NOT NULL,
                ""RequestedQty"" DECIMAL(18,2) DEFAULT 0,
                ""IssuedQty"" DECIMAL(18,2) DEFAULT 0,
                ""StockBefore"" DECIMAL(18,2) DEFAULT 0,
                ""StockAfter"" DECIMAL(18,2) DEFAULT 0,
                ""Purpose"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("good_issue_items OK");

        // vendors
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""vendors"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""VendorCode"" VARCHAR(50) NOT NULL,
                ""VendorName"" VARCHAR(255) NOT NULL,
                ""Address"" VARCHAR(255),
                ""City"" VARCHAR(100),
                ""Phone"" VARCHAR(50),
                ""Email"" VARCHAR(100),
                ""ContactPerson"" VARCHAR(100),
                ""TaxId"" VARCHAR(50),
                ""PaymentTerms"" VARCHAR(50),
                ""Category"" VARCHAR(50) NOT NULL,
                ""Status"" VARCHAR(50) DEFAULT 'ACTIVE',
                ""TotalPurchases"" DECIMAL(18,2) DEFAULT 0,
                ""OutstandingBalance"" DECIMAL(18,2) DEFAULT 0,
                ""Remarks"" VARCHAR(500),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("vendors OK");

        // fuel_receipts
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""fuel_receipts"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""No"" INTEGER NOT NULL,
                ""Tanggal"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""Vendor"" VARCHAR(200),
                ""Liter"" DECIMAL(18,4) DEFAULT 0,
                ""JenisBBM"" VARCHAR(50),
                ""HargaPerLiter"" DECIMAL(18,4) DEFAULT 0,
                ""TotalHarga"" DECIMAL(18,4) DEFAULT 0,
                ""NoTiket"" VARCHAR(50),
                ""StartTime"" TIME,
                ""EndTime"" TIME,
                ""Keterangan"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("fuel_receipts OK");

        // fuel_usages
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""fuel_usages"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""No"" INTEGER NOT NULL,
                ""Tanggal"" TIMESTAMP NOT NULL,
                ""Site"" VARCHAR(100) NOT NULL,
                ""UnitNo"" VARCHAR(50),
                ""Operator"" VARCHAR(100),
                ""HM"" DECIMAL(18,2),
                ""KM"" DECIMAL(18,2),
                ""Pemakaian"" DECIMAL(18,4) DEFAULT 0,
                ""JamKerja"" DECIMAL(18,4) DEFAULT 0,
                ""EFisiensi"" DECIMAL(18,4) DEFAULT 0,
                ""Keterangan"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("fuel_usages OK");

        // ============ P1 MASTER DATA TABLES ============
        // site_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""site_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""SiteCode"" VARCHAR(20) NOT NULL,
                ""SiteName"" VARCHAR(100) NOT NULL,
                ""Region"" VARCHAR(100),
                ""Address"" VARCHAR(255),
                ""City"" VARCHAR(50),
                ""Province"" VARCHAR(50),
                ""SiteType"" VARCHAR(50) DEFAULT 'MINING',
                ""Currency"" VARCHAR(20) DEFAULT 'IDR',
                ""TimeZone"" VARCHAR(20) DEFAULT 'Asia/Makassar',
                ""Status"" VARCHAR(20) DEFAULT 'ACTIVE',
                ""ContactPerson"" VARCHAR(100),
                ""Phone"" VARCHAR(50),
                ""Email"" VARCHAR(100),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("site_master OK");

        // cost_center_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""cost_center_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""CostCenterCode"" VARCHAR(30) NOT NULL,
                ""CostCenterName"" VARCHAR(100) NOT NULL,
                ""SiteCode"" VARCHAR(20) NOT NULL,
                ""DepartmentCode"" VARCHAR(20),
                ""Type"" VARCHAR(30) DEFAULT 'OPERATIONAL',
                ""ParentCode"" VARCHAR(30),
                ""Level"" VARCHAR(5) DEFAULT '1',
                ""AllocatedBudget"" DECIMAL(18,2),
                ""UsedBudget"" DECIMAL(18,2),
                ""CommittedBudget"" DECIMAL(18,2),
                ""AvailableBudget"" DECIMAL(18,2),
                ""Status"" VARCHAR(20) DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("cost_center_master OK");

        // users
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""users"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Username"" VARCHAR(50) NOT NULL,
                ""PasswordHash"" VARCHAR(255) NOT NULL,
                ""FullName"" VARCHAR(100) NOT NULL,
                ""Email"" VARCHAR(100),
                ""Role"" VARCHAR(20) DEFAULT 'USER',
                ""SiteCode"" VARCHAR(20),
                ""Department"" VARCHAR(50),
                ""Position"" VARCHAR(50),
                ""EmployeeCode"" VARCHAR(50),
                ""IsActive"" BOOLEAN DEFAULT TRUE,
                ""LastLoginIP"" VARCHAR(50),
                ""LastLoginAt"" TIMESTAMP,
                ""CreatedBy"" VARCHAR(50),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("users OK");

        // user_sessions
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""user_sessions"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Username"" VARCHAR(50) NOT NULL,
                ""SessionToken"" VARCHAR(100) NOT NULL,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""ExpiresAt"" TIMESTAMP NOT NULL,
                ""IsRevoked"" BOOLEAN DEFAULT FALSE
            );");
        results.Add("user_sessions OK");

        // audit_logs
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""audit_logs"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Username"" VARCHAR(50),
                ""Action"" VARCHAR(50) NOT NULL,
                ""Module"" VARCHAR(50),
                ""RecordId"" VARCHAR(50),
                ""Description"" VARCHAR(500),
                ""IPAddress"" VARCHAR(50),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("audit_logs OK");

        // Seed default admin user (password: admin123)
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""users"" (""Username"", ""PasswordHash"", ""FullName"", ""Email"", ""Role"", ""IsActive"", ""CreatedBy"")
            SELECT 'admin', '$2a$11$8RO64a4/dLAh75zOtgZXGOL0lri.oznHitAqPUWZgUNAEPaMjIFHy', 'System Administrator', 'admin@lfn.com', 'ADMIN', true, 'system'
            WHERE NOT EXISTS (SELECT 1 FROM ""users"" WHERE ""Username"" = 'admin');
        ");
        results.Add("users seed OK");

        // Seed default sites
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""site_master"" (""SiteCode"", ""SiteName"", ""Region"", ""SiteType"", ""Status"")
            SELECT 'TNG', 'Tanjung', 'Kalimantan Selatan', 'MINING', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""site_master"" WHERE ""SiteCode"" = 'TNG');
            INSERT INTO ""site_master"" (""SiteCode"", ""SiteName"", ""Region"", ""SiteType"", ""Status"")
            SELECT 'SDK', 'Sungai Dua', 'Kalimantan Selatan', 'MINING', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""site_master"" WHERE ""SiteCode"" = 'SDK');
            INSERT INTO ""site_master"" (""SiteCode"", ""SiteName"", ""Region"", ""SiteType"", ""Status"")
            SELECT 'SBB', 'Sebamban', 'Kalimantan Selatan', 'MINING', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""site_master"" WHERE ""SiteCode"" = 'SBB');
        ");
        results.Add("site_master seed OK");

        // Seed default cost centers
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""cost_center_master"" (""CostCenterCode"", ""CostCenterName"", ""SiteCode"", ""Type"", ""Status"")
            SELECT 'CC-OPS-TNG', 'Operations Tanjung', 'TNG', 'OPERATIONAL', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""cost_center_master"" WHERE ""CostCenterCode"" = 'CC-OPS-TNG');
            INSERT INTO ""cost_center_master"" (""CostCenterCode"", ""CostCenterName"", ""SiteCode"", ""Type"", ""Status"")
            SELECT 'CC-MNT-TNG', 'Maintenance Tanjung', 'TNG', 'OPERATIONAL', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""cost_center_master"" WHERE ""CostCenterCode"" = 'CC-MNT-TNG');
            INSERT INTO ""cost_center_master"" (""CostCenterCode"", ""CostCenterName"", ""SiteCode"", ""Type"", ""Status"")
            SELECT 'CC-OPS-SDK', 'Operations Sungai Dua', 'SDK', 'OPERATIONAL', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""cost_center_master"" WHERE ""CostCenterCode"" = 'CC-OPS-SDK');
            INSERT INTO ""cost_center_master"" (""CostCenterCode"", ""CostCenterName"", ""SiteCode"", ""Type"", ""Status"")
            SELECT 'CC-MNT-SDK', 'Maintenance Sungai Dua', 'SDK', 'OPERATIONAL', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""cost_center_master"" WHERE ""CostCenterCode"" = 'CC-MNT-SDK');
            INSERT INTO ""cost_center_master"" (""CostCenterCode"", ""CostCenterName"", ""SiteCode"", ""Type"", ""Status"")
            SELECT 'CC-ADMIN', 'Umum & Administrasi', 'TNG', 'ADMIN', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""cost_center_master"" WHERE ""CostCenterCode"" = 'CC-ADMIN');
        ");
        results.Add("cost_center_master seed OK");

        // ============ P2 MASTER DATA TABLES ============
        // department_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""department_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""DeptCode"" VARCHAR(20) NOT NULL,
                ""DeptName"" VARCHAR(100) NOT NULL,
                ""ParentCode"" VARCHAR(20),
                ""Level"" VARCHAR(10) NOT NULL DEFAULT 'DEPARTMENT',
                ""CostCenter"" VARCHAR(50),
                ""SiteCode"" VARCHAR(20),
                ""HeadName"" VARCHAR(100),
                ""HeadTitle"" VARCHAR(50),
                ""Status"" VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("department_master OK");

        // uom_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""uom_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""UomCode"" VARCHAR(20) NOT NULL,
                ""UomName"" VARCHAR(50) NOT NULL,
                ""UomType"" VARCHAR(20) NOT NULL DEFAULT 'VOLUME',
                ""BaseUomCode"" VARCHAR(20),
                ""ConversionFactor"" DECIMAL(18,6),
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("uom_master OK");

        // material_master
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""material_master"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""MaterialCode"" VARCHAR(30) NOT NULL,
                ""MaterialName"" VARCHAR(200) NOT NULL,
                ""MaterialGroup"" VARCHAR(20),
                ""MaterialType"" VARCHAR(20) NOT NULL DEFAULT 'ITEM',
                ""UomCode"" VARCHAR(20),
                ""Brand"" VARCHAR(50),
                ""Spec"" VARCHAR(50),
                ""UnitPrice"" DECIMAL(18,2),
                ""Currency"" VARCHAR(20) NOT NULL DEFAULT 'IDR',
                ""SiteCode"" VARCHAR(20),
                ""Status"" VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
                ""Remarks"" VARCHAR(255),
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("material_master OK");

        // approval_workflow
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""approval_workflow"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""WorkflowName"" VARCHAR(30) NOT NULL,
                ""ModuleType"" VARCHAR(30) NOT NULL,
                ""SiteCode"" VARCHAR(30),
                ""CostCenter"" VARCHAR(30),
                ""ApprovalOrder"" INT NOT NULL DEFAULT 1,
                ""ApproverRole"" VARCHAR(30) NOT NULL,
                ""ApproverName"" VARCHAR(100),
                ""ApprovalLevel"" VARCHAR(20) NOT NULL DEFAULT 'REQUIRED',
                ""MinAmount"" DECIMAL(18,2),
                ""MaxAmount"" DECIMAL(18,2),
                ""Status"" VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ""UpdatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );");
        results.Add("approval_workflow OK");

        // Seed UOM
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'L', 'Liter', 'VOLUME', 'Standard volume unit'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'L');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'GAL', 'Gallon', 'VOLUME', 'US Gallon'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'GAL');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'KG', 'Kilogram', 'WEIGHT', 'Standard weight unit'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'KG');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'PCS', 'Pieces', 'COUNT', 'Unit count'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'PCS');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'UNIT', 'Unit', 'COUNT', 'Generic unit'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'UNIT');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'HRS', 'Hours', 'TIME', 'Labor hours'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'HRS');
            INSERT INTO ""uom_master"" (""UomCode"", ""UomName"", ""UomType"", ""Remarks"")
            SELECT 'IDR', 'Indonesian Rupiah', 'CURRENCY', 'Local currency'
            WHERE NOT EXISTS (SELECT 1 FROM ""uom_master"" WHERE ""UomCode"" = 'IDR');
        ");
        results.Add("uom_master seed OK");

        // Seed Department
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DIV-OPS', 'Operations Division', 'DIVISION', NULL, 'TNG', 'John Manager', 'Operations Director', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DIV-OPS');
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""ParentCode"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DEPT-HAUL', 'Hauling Department', 'DIV-OPS', 'DEPARTMENT', 'CC-OPS-TNG', 'Bob Supervisor', 'Hauling Supervisor', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DEPT-HAUL');
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""ParentCode"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DEPT-MNT', 'Maintenance Department', 'DIV-OPS', 'DEPARTMENT', 'CC-MNT-TNG', 'Charlie Mechanic', 'Maintenance Manager', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DEPT-MNT');
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""ParentCode"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DEPT-ADMIN', 'Administration', 'DIV-OPS', 'DEPARTMENT', 'CC-ADMIN', 'Alice Admin', 'Admin Manager', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DEPT-ADMIN');
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DIV-FIN', 'Finance Division', 'DIVISION', NULL, 'TNG', 'David Finance', 'Finance Director', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DIV-FIN');
            INSERT INTO ""department_master"" (""DeptCode"", ""DeptName"", ""ParentCode"", ""Level"", ""CostCenter"", ""SiteCode"", ""HeadName"", ""HeadTitle"", ""Status"")
            SELECT 'DEPT-ACC', 'Accounting', 'DIV-FIN', 'DEPARTMENT', 'CC-ADMIN', 'Eve Accounting', 'Accounting Head', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""department_master"" WHERE ""DeptCode"" = 'DEPT-ACC');
        ");
        results.Add("department_master seed OK");

        // Seed Material
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'FUEL-DIESEL', 'Solar / Diesel', 'FUEL', 'ITEM', 'L', 15000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'FUEL-DIESEL');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'TYRE-OTR-24.00', 'OTR Tyre 24.00R25', 'TYRE', 'ITEM', 'PCS', 8500000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'TYRE-OTR-24.00');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'PART-ENGINE-OIL', 'Engine Oil 15W-40', 'PART', 'ITEM', 'L', 85000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'PART-ENGINE-OIL');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'PART-BRAKE-PAD', 'Brake Pad Set', 'PART', 'ITEM', 'PCS', 450000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'PART-BRAKE-PAD');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'SVC-MAINTENANCE', 'Maintenance Labor', 'SERVICE', 'SERVICE', 'HRS', 75000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'SVC-MAINTENANCE');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'PART-FILTER-OIL', 'Oil Filter', 'PART', 'ITEM', 'PCS', 125000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'PART-FILTER-OIL');
            INSERT INTO ""material_master"" (""MaterialCode"", ""MaterialName"", ""MaterialGroup"", ""MaterialType"", ""UomCode"", ""UnitPrice"", ""Currency"", ""Status"")
            SELECT 'PART-TIRE-CHAIN', 'Tire Chain', 'PART', 'ITEM', 'PCS', 2500000.00, 'IDR', 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""material_master"" WHERE ""MaterialCode"" = 'PART-TIRE-CHAIN');
        ");
        results.Add("material_master seed OK");

        // Seed Approval Workflow
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'PR Manager Approval', 'PR', 1, 'MANAGER', 'REQUIRED', 0, 10000000, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'PR Manager Approval');
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'PR Director Approval', 'PR', 2, 'ADMIN', 'REQUIRED', 10000001, NULL, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'PR Director Approval');
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'PO Manager Approval', 'PO', 1, 'MANAGER', 'REQUIRED', 0, 25000000, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'PO Manager Approval');
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'PO Director Approval', 'PO', 2, 'ADMIN', 'REQUIRED', 25000001, NULL, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'PO Director Approval');
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'GR Auto-Approve', 'GR', 1, 'USER', 'REQUIRED', 0, NULL, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'GR Auto-Approve');
            INSERT INTO ""approval_workflow"" (""WorkflowName"", ""ModuleType"", ""ApprovalOrder"", ""ApproverRole"", ""ApprovalLevel"", ""MinAmount"", ""MaxAmount"", ""Status"")
            SELECT 'Fuel Usage Approval', 'FUEL_USAGE', 1, 'MANAGER', 'REQUIRED', 0, NULL, 'ACTIVE'
            WHERE NOT EXISTS (SELECT 1 FROM ""approval_workflow"" WHERE ""WorkflowName"" = 'Fuel Usage Approval');
        ");
        results.Add("approval_workflow seed OK");


        return Results.Ok(new { message = "All tables created/migrated", results });
        return Results.Ok(new { message = "All tables created/migrated", results });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

// =====================================================
// API ENDPOINTS
// =====================================================

// Health Check
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.Now,
    database = useProvider
});

// =====================================================
// GCS (Cloud Storage) ENDPOINTS - Raw Data Storage
// =====================================================

// Upload file langsung ke Google Cloud Storage
app.MapPost("/api/upload-to-gcs", async (HttpRequest request, GcsService gcsService) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Invalid form content type" });

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var site = form["site"].ToString();
    var datatype = form["datatype"].ToString();

    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file uploaded" });

    if (string.IsNullOrWhiteSpace(site))
        return Results.BadRequest(new { error = "Site is required" });

    if (string.IsNullOrWhiteSpace(datatype))
        return Results.BadRequest(new { error = "DataType is required" });

    try
    {
        // Generate object name dengan folder structure: {datatype}/{site}/{date}/{filename}_{timestamp}.csv
        var objectName = gcsService.GenerateObjectName(site, datatype, file.FileName);

        // Upload ke GCS
        using var stream = file.OpenReadStream();
        var publicUrl = await gcsService.UploadStreamAsync(
            stream,
            objectName,
            file.ContentType
        );

        return Results.Ok(new
        {
            message = "File uploaded successfully to Google Cloud Storage",
            objectName = objectName,
            publicUrl = publicUrl,
            bucket = "lfn-hauling-raw-data",
            site = site,
            datatype = datatype,
            fileName = file.FileName,
            size = file.Length
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Upload Failed",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// List semua files dalam bucket GCS dengan filter
app.MapGet("/api/gcs-files", async (GcsService gcsService, string? prefix, string? site, string? datatype) =>
{
    try
    {
        // Build prefix berdasarkan filter
        var filterPrefix = string.Empty;

        if (!string.IsNullOrWhiteSpace(datatype) && !string.IsNullOrWhiteSpace(site))
        {
            filterPrefix = $"{datatype}/{site}/";
        }
        else if (!string.IsNullOrWhiteSpace(datatype))
        {
            filterPrefix = $"{datatype}/";
        }
        else if (!string.IsNullOrWhiteSpace(prefix))
        {
            filterPrefix = prefix;
        }

        var files = await gcsService.ListFilesAsync(filterPrefix);

        return Results.Ok(new
        {
            message = "Files retrieved successfully",
            prefix = filterPrefix,
            count = files.Count,
            files = files
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Failed to List Files",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// Download file dari Google Cloud Storage
app.MapGet("/api/download-from-gcs", async (GcsService gcsService, string objectName) =>
{
    if (string.IsNullOrWhiteSpace(objectName))
        return Results.BadRequest(new { error = "Object name is required" });

    try
    {
        var stream = await gcsService.DownloadToStreamAsync(objectName);

        // Determine content type
        var contentType = Path.GetExtension(objectName).ToLower() switch
        {
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        return Results.File(
            stream,
            contentType,
            Path.GetFileName(objectName)
        );
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Download Failed",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// Delete file dari Google Cloud Storage
app.MapDelete("/api/gcs-files", async (GcsService gcsService, string objectName) =>
{
    if (string.IsNullOrWhiteSpace(objectName))
        return Results.BadRequest(new { error = "Object name is required" });

    try
    {
        await gcsService.DeleteFileAsync(objectName);

        return Results.Ok(new
        {
            message = "File deleted successfully",
            objectName = objectName
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Delete Failed",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// Cek apakah file ada di GCS
app.MapGet("/api/gcs-files/exists", async (GcsService gcsService, string objectName) =>
{
    if (string.IsNullOrWhiteSpace(objectName))
        return Results.BadRequest(new { error = "Object name is required" });

    try
    {
        var exists = await gcsService.FileExistsAsync(objectName);

        return Results.Ok(new
        {
            exists = exists,
            objectName = objectName
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Check Failed",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// Upload CSV data to database dengan validasi duplikasi
app.MapPost("/api/upload", async (HttpRequest request, AppDbContext db) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Invalid form content type" });

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var site = form["site"].ToString();
    var datatype = form["datatype"].ToString();

    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file uploaded" });

    if (string.IsNullOrWhiteSpace(site))
        return Results.BadRequest(new { error = "Site is required" });

    if (string.IsNullOrWhiteSpace(datatype))
        return Results.BadRequest(new { error = "DataType is required" });

    try
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;
        int skippedCount = 0;
        int errorCount = 0;
        var skippedRecords = new List<string>();
        var errorRecords = new List<string>();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                if (string.Equals(datatype, "receipt", StringComparison.OrdinalIgnoreCase))
                {
                    var noTiket = GetValue(values, headers, "NoTiket");

                    // Cek duplikasi berdasarkan no_tiket
                    var existingReceipt = await db.FuelReceipts
                        .FirstOrDefaultAsync(r => r.NoTiket == noTiket);

                    if (existingReceipt != null)
                    {
                        skippedCount++;
                        skippedRecords.Add($"Receipt with NoTiket '{noTiket}' already exists");
                        continue;
                    }

                    // Parse Tanggal dengan support multiple format
                    var tanggalStr = GetValue(values, headers, "Tanggal");
                    DateTime tanggal = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(tanggalStr))
                    {
                        // Coba format Indonesia (dd/MM/yyyy)
                        if (DateTime.TryParseExact(tanggalStr, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalId))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalId, DateTimeKind.Unspecified);
                        }
                        // Coba format ISO (yyyy-MM-dd)
                        else if (DateTime.TryParseExact(tanggalStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalIso))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalIso, DateTimeKind.Unspecified);
                        }
                        // Coba parse default
                        else if (DateTime.TryParse(tanggalStr, out var tanggalDefault))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalDefault, DateTimeKind.Unspecified);
                        }
                    }
                    else
                    {
                        tanggal = DateTime.Now;
                    }

                    // Parse StartTime dengan support format HH:mm atau H:mm
                    var startTimeStr = GetValue(values, headers, "Start Time");
                    TimeSpan startTime = TimeSpan.Zero;
                    if (!string.IsNullOrWhiteSpace(startTimeStr))
                    {
                        // Replace common separators dan coba parse
                        var normalizedTime = startTimeStr.Replace(".", ":").Trim();
                        if (TimeSpan.TryParse(normalizedTime, out var startTimeParsed))
                        {
                            startTime = startTimeParsed;
                        }
                    }

                    // Parse EndTime dengan support format HH:mm atau H:mm
                    var endTimeStr = GetValue(values, headers, "End Time");
                    TimeSpan endTime = TimeSpan.Zero;
                    if (!string.IsNullOrWhiteSpace(endTimeStr))
                    {
                        var normalizedTime = endTimeStr.Replace(".", ":").Trim();
                        if (TimeSpan.TryParse(normalizedTime, out var endTimeParsed))
                        {
                            endTime = endTimeParsed;
                        }
                    }

                    var receipt = new FuelReceipt
                    {
                        No = int.TryParse(GetValue(values, headers, "No"), out var no) ? no : i,
                        Tanggal = tanggal,
                        Site = site,
                        Vendor = GetValue(values, headers, "Vendor") ?? GetValue(values, headers, "Supplier"),
                        Liter = decimal.TryParse(GetValue(values, headers, "Liter"), out var liter) ? liter : 0,
                        JenisBBM = GetValue(values, headers, "JenisBBM"),
                        HargaPerLiter = decimal.TryParse(GetValue(values, headers, "HargaPerLiter"), out var harga) ? harga : 0,
                        TotalHarga = decimal.TryParse(GetValue(values, headers, "TotalHarga"), out var total) ? total : 0,
                        NoTiket = noTiket,
                        StartTime = startTime,
                        EndTime = endTime,
                        Keterangan = GetValue(values, headers, "Keterangan")
                    };
                    db.FuelReceipts.Add(receipt);
                    insertedCount++;
                }
                else if (string.Equals(datatype, "usage", StringComparison.OrdinalIgnoreCase))
                {
                    var unitNo = GetValue(values, headers, "UnitNo");

                    // Parse Tanggal dengan support multiple format
                    var tanggalStr = GetValue(values, headers, "Tanggal");
                    DateTime tanggal = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(tanggalStr))
                    {
                        // Coba format Indonesia (dd/MM/yyyy)
                        if (DateTime.TryParseExact(tanggalStr, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalId))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalId, DateTimeKind.Unspecified);
                        }
                        // Coba format ISO (yyyy-MM-dd)
                        else if (DateTime.TryParseExact(tanggalStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalIso))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalIso, DateTimeKind.Unspecified);
                        }
                        // Coba parse default
                        else if (DateTime.TryParse(tanggalStr, out var tanggalDefault))
                        {
                            tanggal = DateTime.SpecifyKind(tanggalDefault, DateTimeKind.Unspecified);
                        }
                    }
                    else
                    {
                        tanggal = DateTime.Now;
                    }

                    // Cek duplikasi berdasarkan site, tanggal, dan unit_no
                    var existingUsage = await db.FuelUsages
                        .FirstOrDefaultAsync(u =>
                            u.Site.ToLower() == site.ToLower() &&
                            u.Tanggal == tanggal &&
                            u.UnitNo.ToLower() == unitNo.ToLower());

                    if (existingUsage != null)
                    {
                        skippedCount++;
                        skippedRecords.Add($"Usage for Unit '{unitNo}' on {tanggal:yyyy-MM-dd} already exists");
                        continue;
                    }

                    var usage = new FuelUsage
                    {
                        No = int.TryParse(GetValue(values, headers, "No"), out var no) ? no : i,
                        Tanggal = tanggal,
                        Site = site,
                        UnitNo = unitNo,
                        Operator = GetValue(values, headers, "Operator"),
                        HM = decimal.TryParse(GetValue(values, headers, "HM"), out var awal) ? awal : 0,
                        KM = decimal.TryParse(GetValue(values, headers, "KM"), out var akhir) ? akhir : 0,
                        Pemakaian = decimal.TryParse(GetValue(values, headers, "Pemakaian"), out var pakai) ? pakai : 0,
                        JamKerja = decimal.TryParse(GetValue(values, headers, "JamKerja"), out var jam) ? jam : 0,
                        EFisiensi = decimal.TryParse(GetValue(values, headers, "EFisiensi")?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var efisiensi) ? efisiensi : 0,
                        Keterangan = GetValue(values, headers, "Keterangan")
                    };
                    db.FuelUsages.Add(usage);
                    insertedCount++;
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                // Log error tapi continue processing
                string errorMsg = $"Line {i + 1}: {ex.Message}";
                Console.WriteLine(errorMsg);
                errorRecords.Add(errorMsg);
            }
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = $"Error saving to database: {ex.InnerException?.Message ?? ex.Message}" });
        }

        return Results.Ok(new
        {
            message = $"File {file.FileName} processed successfully",
            site = site,
            datatype = datatype,
            totalRows = lines.Length - 1,
            inserted = insertedCount,
            skipped = skippedCount,
            errors = errorCount,
            skippedRecords = skippedRecords.Take(10).ToList(), // Show first 10 skipped records
            errorRecords = errorRecords.Take(10).ToList(), // Show first 10 error records
            warning = skippedCount > 0 ? "Some records were skipped because they already exist in the database." : null
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error processing file: {ex.Message}" });
    }
});

// Download Template
app.MapGet("/download-template/{type}", (string type) =>
{
    var filePath = type switch
    {
        "receipt" => Path.Combine("wwwroot/templates", "fuel_receipt_template_v1.csv"),
        "usage" => Path.Combine("wwwroot/templates", "fuel_usage_template_v1.csv"),
        "tyres-po" => Path.Combine("wwwroot/templates", "tyres_po_template.csv"),
        "tyres-problem" => Path.Combine("wwwroot/templates", "tyres_problem_template.csv"),
        _ => null
    };

    if (filePath == null || !System.IO.File.Exists(filePath))
        return Results.NotFound(new { error = "Template not found" });

    var fileBytes = System.IO.File.ReadAllBytes(filePath);

    return Results.File(
        fileBytes,
        "text/csv",
        Path.GetFileName(filePath)
    );
});

// Download Data from Database with validation
app.MapGet("/api/download-data", async (AppDbContext db, string? site, string? datatype) =>
{
    // Validasi DataType (wajib)
    if (string.IsNullOrWhiteSpace(datatype))
    {
        return Results.BadRequest(new { error = "Parameter 'datatype' wajib diisi. Silakan pilih tipe data terlebih dahulu." });
    }

    // Site tidak wajib - jika tidak diisi, ambil semua data
    var siteFilter = !string.IsNullOrWhiteSpace(site) ? site.Trim() : null;

    // Tentukan file berdasarkan datatype
    var fileName = (siteFilter != null ? siteFilter.ToLower().Replace(" ", "_") : "all") + "_" + datatype.ToLower() + $"_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

    // Generate CSV dari Database
    var csvContent = await GenerateCsvFromDatabase(db, site, datatype);

    if (string.IsNullOrEmpty(csvContent))
    {
        return Results.NotFound(new { error = $"Tidak ada data ditemukan untuk site: {site}, tipe: {datatype}" });
    }

    // Tambahkan BOM UTF-8 agar Excel membaca dengan benar
    var bom = new byte[] { 0xEF, 0xBB, 0xBF };
    var contentBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
    var fileBytes = new byte[bom.Length + contentBytes.Length];
    Buffer.BlockCopy(bom, 0, fileBytes, 0, bom.Length);
    Buffer.BlockCopy(contentBytes, 0, fileBytes, bom.Length, contentBytes.Length);

    return Results.File(
        fileBytes,
        "text/csv",
        fileName
    );
});

// Get Fuel Receipts by Site and Date Range
app.MapGet("/api/fuel-receipts", async (AppDbContext db, string? site = null, DateTime? startDate = null, DateTime? endDate = null) =>
{
    var query = db.FuelReceipts.AsQueryable();

    if (!string.IsNullOrWhiteSpace(site) && site != "all")
        query = query.Where(r => r.Site.ToLower() == site.ToLower());

    if (startDate.HasValue)
        query = query.Where(r => r.Tanggal >= startDate.Value);

    if (endDate.HasValue)
        query = query.Where(r => r.Tanggal <= endDate.Value);

    var results = await query
        .OrderBy(r => r.No)
        .ThenBy(r => r.Tanggal)
        .ToListAsync();

    return Results.Ok(results);
});

// Get Fuel Usages by Site and Date Range
app.MapGet("/api/fuel-usages", async (AppDbContext db, string? site = null, DateTime? startDate = null, DateTime? endDate = null) =>
{
    var query = db.FuelUsages.AsQueryable();

    if (!string.IsNullOrWhiteSpace(site) && site != "all")
        query = query.Where(u => u.Site.ToLower() == site.ToLower());

    if (startDate.HasValue)
        query = query.Where(u => u.Tanggal >= startDate.Value);

    if (endDate.HasValue)
        query = query.Where(u => u.Tanggal <= endDate.Value);

    var results = await query
        .OrderBy(u => u.No)
        .ThenBy(u => u.Tanggal)
        .ToListAsync();

    return Results.Ok(results);
});

// ===== FUEL RECEIPT CRUD =====
app.MapPost("/api/fuel-receipts", async (FuelReceipt receipt, AppDbContext db) =>
{
    // === VALIDASI NEGATIF ===
    if (receipt.Liter < 0) return Results.BadRequest(new { error = "Liter tidak boleh negatif" });
    if (receipt.HargaPerLiter < 0) return Results.BadRequest(new { error = "HargaPerLiter tidak boleh negatif" });
    if (receipt.TotalHarga < 0) return Results.BadRequest(new { error = "TotalHarga tidak boleh negatif" });
    if (receipt.NoTiket == null || receipt.NoTiket.Trim() == "") return Results.BadRequest(new { error = "NoTiket wajib diisi" });
    if (receipt.Site == null || receipt.Site.Trim() == "") return Results.BadRequest(new { error = "Site wajib diisi" });
    if (receipt.Vendor == null || receipt.Vendor.Trim() == "") return Results.BadRequest(new { error = "Vendor wajib diisi" });
    if (receipt.Tanggal > DateTime.UtcNow.AddDays(1)) return Results.BadRequest(new { error = "Tanggal tidak boleh di masa depan" });

    // === DUPLICATE SLIP CHECK ===
    var existingSlip = await db.FuelReceipts.FirstOrDefaultAsync(r => r.NoTiket == receipt.NoTiket && r.Site == receipt.Site);
    if (existingSlip != null) return Results.BadRequest(new { error = $"NoTiket '{receipt.NoTiket}' sudah ada di site '{receipt.Site}'. Kemungkinan klaim ganda!" });

    var maxNo = db.FuelReceipts.Any() ? db.FuelReceipts.Max(r => r.No) : 0;
    receipt.No = maxNo + 1;
    receipt.CreatedAt = DateTime.UtcNow;
    receipt.UpdatedAt = DateTime.UtcNow;
    db.FuelReceipts.Add(receipt);
    await db.SaveChangesAsync();
    return Results.Created($"/api/fuel-receipts/{receipt.Id}", receipt);
});

app.MapPut("/api/fuel-receipts/{id}", async (int id, FuelReceipt updated, AppDbContext db) =>
{
    if (updated.Liter < 0) return Results.BadRequest(new { error = "Liter tidak boleh negatif" });
    if (updated.HargaPerLiter < 0) return Results.BadRequest(new { error = "HargaPerLiter tidak boleh negatif" });
    if (updated.TotalHarga < 0) return Results.BadRequest(new { error = "TotalHarga tidak boleh negatif" });
    if (updated.Tanggal > DateTime.UtcNow.AddDays(1)) return Results.BadRequest(new { error = "Tanggal tidak boleh di masa depan" });

    var receipt = await db.FuelReceipts.FindAsync(id);
    if (receipt == null) return Results.NotFound();
    receipt.Tanggal = updated.Tanggal;
    receipt.Site = updated.Site;
    receipt.Vendor = updated.Vendor;
    receipt.Liter = updated.Liter;
    receipt.JenisBBM = updated.JenisBBM;
    receipt.HargaPerLiter = updated.HargaPerLiter;
    receipt.TotalHarga = updated.TotalHarga;
    receipt.NoTiket = updated.NoTiket;
    receipt.StartTime = updated.StartTime;
    receipt.EndTime = updated.EndTime;
    receipt.Keterangan = updated.Keterangan;
    receipt.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(receipt);
});

app.MapDelete("/api/fuel-receipts/{id}", async (int id, AppDbContext db) =>
{
    var receipt = await db.FuelReceipts.FindAsync(id);
    if (receipt == null) return Results.NotFound();
    db.FuelReceipts.Remove(receipt);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ===== FUEL USAGE CRUD =====
app.MapPost("/api/fuel-usages", async (FuelUsage usage, AppDbContext db) =>
{
    // === VALIDASI NEGATIF ===
    if (usage.HM < 0) return Results.BadRequest(new { error = "HM tidak boleh negatif" });
    if (usage.KM < 0) return Results.BadRequest(new { error = "KM tidak boleh negatif" });
    if (usage.Pemakaian < 0) return Results.BadRequest(new { error = "Pemakaian BBM tidak boleh negatif" });
    if (usage.JamKerja < 0) return Results.BadRequest(new { error = "JamKerja tidak boleh negatif" });
    if (usage.EFisiensi < 0) return Results.BadRequest(new { error = "Efisiensi tidak boleh negatif" });
    if (usage.Site == null || usage.Site.Trim() == "") return Results.BadRequest(new { error = "Site wajib diisi" });
    if (usage.UnitNo == null || usage.UnitNo.Trim() == "") return Results.BadRequest(new { error = "UnitNo wajib diisi" });
    if (usage.Operator == null || usage.Operator.Trim() == "") return Results.BadRequest(new { error = "Operator/Driver wajib diisi" });
    if (usage.Tanggal > DateTime.UtcNow.AddDays(1)) return Results.BadRequest(new { error = "Tanggal tidak boleh di masa depan" });

    // === DUPLICATE CHECK: tidak boleh duplikat per (UnitNo + Tanggal) ===
    var duplicate = await db.FuelUsages.FirstOrDefaultAsync(u => u.UnitNo == usage.UnitNo && u.Tanggal.Date == usage.Tanggal.Date);
    if (duplicate != null) return Results.BadRequest(new { error = $"Fuel usage untuk Unit {usage.UnitNo} di tanggal {usage.Tanggal:yyyy-MM-dd} sudah ada (No: {duplicate.No}). Harap gunakan data yang sudah ada." });

    var maxNo = db.FuelUsages.Any() ? db.FuelUsages.Max(u => u.No) : 0;
    usage.No = maxNo + 1;
    usage.CreatedAt = DateTime.UtcNow;
    usage.UpdatedAt = DateTime.UtcNow;
    db.FuelUsages.Add(usage);
    await db.SaveChangesAsync();
    return Results.Created($"/api/fuel-usages/{usage.Id}", usage);
});

app.MapPut("/api/fuel-usages/{id}", async (int id, FuelUsage updated, AppDbContext db) =>
{
    var usage = await db.FuelUsages.FindAsync(id);
    if (usage == null) return Results.NotFound();
    usage.Tanggal = updated.Tanggal;
    usage.Site = updated.Site;
    usage.UnitNo = updated.UnitNo;
    usage.Operator = updated.Operator;
    usage.HM = updated.HM;
    usage.KM = updated.KM;
    usage.Pemakaian = updated.Pemakaian;
    usage.JamKerja = updated.JamKerja;
    usage.EFisiensi = updated.EFisiensi;
    usage.Keterangan = updated.Keterangan;
    usage.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(usage);
});

app.MapDelete("/api/fuel-usages/{id}", async (int id, AppDbContext db) =>
{
    var usage = await db.FuelUsages.FindAsync(id);
    if (usage == null) return Results.NotFound();
    db.FuelUsages.Remove(usage);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ===== TYRE PO CRUD =====
app.MapPost("/api/tyres/po", async (TyrePO tyrePO, AppDbContext db) =>
{
    var maxNo = db.TyrePOs.Any() ? db.TyrePOs.Max(t => t.No) : 0;
    tyrePO.No = maxNo + 1;
    tyrePO.CreatedAt = DateTime.UtcNow;
    tyrePO.UpdatedAt = DateTime.UtcNow;
    db.TyrePOs.Add(tyrePO);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tyres/po/{tyrePO.Id}", tyrePO);
});

app.MapPut("/api/tyres/po/{id}", async (int id, TyrePO updated, AppDbContext db) =>
{
    var tyrePO = await db.TyrePOs.FindAsync(id);
    if (tyrePO == null) return Results.NotFound();
    tyrePO.Tanggal = updated.Tanggal;
    tyrePO.Site = updated.Site;
    tyrePO.Vendor = updated.Vendor;
    tyrePO.NoPO = updated.NoPO;
    tyrePO.MerkType = updated.MerkType;
    tyrePO.Size = updated.Size;
    tyrePO.Qty = updated.Qty;
    tyrePO.UnitPrice = updated.UnitPrice;
    tyrePO.TotalPrice = updated.TotalPrice;
    tyrePO.Status = updated.Status;
    tyrePO.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(tyrePO);
});

app.MapDelete("/api/tyres/po/{id}", async (int id, AppDbContext db) =>
{
    var tyrePO = await db.TyrePOs.FindAsync(id);
    if (tyrePO == null) return Results.NotFound();
    db.TyrePOs.Remove(tyrePO);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ===== TYRE PROBLEM CRUD =====
app.MapPost("/api/tyres/problems", async (TyreProblem tyreProblem, AppDbContext db) =>
{
    var maxNo = db.TyreProblems.Any() ? db.TyreProblems.Max(t => t.No) : 0;
    tyreProblem.No = maxNo + 1;
    tyreProblem.CreatedAt = DateTime.UtcNow;
    tyreProblem.UpdatedAt = DateTime.UtcNow;
    db.TyreProblems.Add(tyreProblem);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tyres/problems/{tyreProblem.Id}", tyreProblem);
});

app.MapPut("/api/tyres/problems/{id}", async (int id, TyreProblem updated, AppDbContext db) =>
{
    var tyreProblem = await db.TyreProblems.FindAsync(id);
    if (tyreProblem == null) return Results.NotFound();
    tyreProblem.Tanggal = updated.Tanggal;
    tyreProblem.Site = updated.Site;
    tyreProblem.UnitNo = updated.UnitNo;
    tyreProblem.SerialNumber = updated.SerialNumber;
    tyreProblem.MerkType = updated.MerkType;
    tyreProblem.Size = updated.Size;
    tyreProblem.Problem = updated.Problem;
    tyreProblem.Kerusakan = updated.Kerusakan;
    tyreProblem.Location = updated.Location;
    tyreProblem.StartHM = updated.StartHM;
    tyreProblem.EndHM = updated.EndHM;
    tyreProblem.TotalHM = updated.TotalHM;
    tyreProblem.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(tyreProblem);
});

app.MapDelete("/api/tyres/problems/{id}", async (int id, AppDbContext db) =>
{
    var tyreProblem = await db.TyreProblems.FindAsync(id);
    if (tyreProblem == null) return Results.NotFound();
    db.TyreProblems.Remove(tyreProblem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// =====================================================
// HELPER FUNCTIONS
// =====================================================

string GetValue(string[] values, string[] headers, string columnName)
{
    var index = Array.FindIndex(headers, h => h.Trim().Equals(columnName, StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index < values.Length ? values[index].Trim() : "";
}

async Task<string> GenerateCsvFromDatabase(AppDbContext db, string? site, string datatype)
{
    var lines = new List<string>();
    var siteFilter = !string.IsNullOrWhiteSpace(site) ? site!.Trim().ToLower() : null;

    if (datatype.Equals("receipt", StringComparison.OrdinalIgnoreCase))
    {
        var query = db.FuelReceipts.AsQueryable();

        // Filter by site if provided
        if (siteFilter != null)
            query = query.Where(r => r.Site.ToLower() == siteFilter);

        var receipts = await query
            .OrderByDescending(r => r.Tanggal)
            .ThenBy(r => r.No)
            .ToListAsync();

        if (receipts.Any())
        {
            lines.Add("No;Tanggal;Site;Vendor;Liter;JenisBBM;HargaPerLiter;NoTiket;Start Time;End Time;Keterangan");
            foreach (var r in receipts)
            {
                lines.Add($"{r.No};{r.Tanggal:yyyy-MM-dd};{r.Site};{r.Vendor};{r.Liter};{r.JenisBBM};{r.HargaPerLiter};{r.NoTiket};{r.StartTime:hh\\:mm};{r.EndTime:hh\\:mm};{r.Keterangan}");
            }
        }
    }
    else if (datatype.Equals("usage", StringComparison.OrdinalIgnoreCase))
    {
        var query = db.FuelUsages.AsQueryable();

        // Filter by site if provided
        if (siteFilter != null)
            query = query.Where(u => u.Site.ToLower() == siteFilter);

        var usages = await query
            .OrderByDescending(u => u.Tanggal)
            .ThenBy(u => u.No)
            .ToListAsync();

        if (usages.Any())
        {
            lines.Add("No;Tanggal;Site;UnitNo;Operator;HM;KM;Pemakaian;JamKerja;EFisiensi;Keterangan");
            foreach (var u in usages)
            {
                lines.Add($"{u.No};{u.Tanggal:yyyy-MM-dd};{u.Site};{u.UnitNo};{u.Operator};{u.HM};{u.KM};{u.Pemakaian};{u.JamKerja};{u.EFisiensi.ToString(System.Globalization.CultureInfo.InvariantCulture)};{u.Keterangan}");
            }
        }
    }

    return string.Join("\n", lines);
}

// Get all unique sites
app.MapGet("/api/sites", async (AppDbContext db) =>
{
    try
    {
        var receiptSites = await db.FuelReceipts
            .Select(r => r.Site)
            .Distinct()
            .ToListAsync();

        var usageSites = await db.FuelUsages
            .Select(u => u.Site)
            .Distinct()
            .ToListAsync();

        var allSites = receiptSites
            .Union(usageSites)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .OrderBy(s => s)
            .ToList();

        return Results.Ok(new
        {
            sites = allSites,
            count = allSites.Count
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Failed to retrieve sites",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

app.MapDelete("/api/cleansing", async (AppDbContext db) =>
{
    try
    {
        var receiptsDeleted = await db.FuelReceipts.ExecuteDeleteAsync();
        var usagesDeleted = await db.FuelUsages.ExecuteDeleteAsync();

        return Results.Ok(new
        {
            message = "All data cleansed successfully",
            receiptsDeleted = receiptsDeleted,
            usagesDeleted = usagesDeleted
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Cleansing Failed", detail: ex.Message, statusCode: 500);
    }
});

// Get statistics
app.MapGet("/api/statistics", async (AppDbContext db, string? site) =>
{
    try
    {
        var receiptsQuery = db.FuelReceipts.AsQueryable();
        var usagesQuery = db.FuelUsages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(site))
        {
            receiptsQuery = receiptsQuery.Where(r => r.Site.ToLower() == site.ToLower());
            usagesQuery = usagesQuery.Where(u => u.Site.ToLower() == site.ToLower());
        }

        var totalReceipts = await receiptsQuery.CountAsync();
        var totalUsages = await usagesQuery.CountAsync();

        var totalLiter = await receiptsQuery.SumAsync(r => r.Liter);
        var totalUsage = await usagesQuery.SumAsync(u => u.Pemakaian);

        var totalHarga = await receiptsQuery.SumAsync(r => r.TotalHarga);

        return Results.Ok(new
        {
            totalReceipts = totalReceipts,
            totalUsages = totalUsages,
            totalRecords = totalReceipts + totalUsages,
            totalLiter = totalLiter,
            totalUsage = totalUsage,
            totalHarga = totalHarga
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Failed to retrieve statistics",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// ============ TYRE API ENDPOINTS ============

// Get Tyre PO by Site
app.MapGet("/api/tyres/po", async (AppDbContext db, string? site = null) =>
{
    var query = db.TyrePOs.AsQueryable();
    if (!string.IsNullOrWhiteSpace(site) && site != "all")
        query = query.Where(r => r.Site.ToLower() == site.ToLower());

    var results = await query.OrderByDescending(r => r.Tanggal).ThenBy(r => r.No).ToListAsync();
    return Results.Ok(results);
});

// Get Tyre Problems by Site
app.MapGet("/api/tyres/problems", async (AppDbContext db, string? site = null) =>
{
    var query = db.TyreProblems.AsQueryable();
    if (!string.IsNullOrWhiteSpace(site) && site != "all")
        query = query.Where(r => r.Site.ToLower() == site.ToLower());

    var results = await query.OrderByDescending(r => r.Tanggal).ThenBy(r => r.No).ToListAsync();
    return Results.Ok(results);
});

// Upload Tyre PO
app.MapPost("/api/tyres/po-upload", async (HttpRequest request, AppDbContext db) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Invalid form content type" });

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var site = form["site"].ToString();

    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file uploaded" });

    try
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var tanggalStr = GetValue(values, headers, "Tanggal");
                DateTime tanggal = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(tanggalStr))
                {
                    if (DateTime.TryParseExact(tanggalStr, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalId))
                        tanggal = DateTime.SpecifyKind(tanggalId, DateTimeKind.Unspecified);
                    else if (DateTime.TryParseExact(tanggalStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalIso))
                        tanggal = DateTime.SpecifyKind(tanggalIso, DateTimeKind.Unspecified);
                }

                var tyrePo = new TyrePO
                {
                    No = int.TryParse(GetValue(values, headers, "No"), out var no) ? no : i,
                    Tanggal = tanggal,
                    Site = site,
                    Vendor = GetValue(values, headers, "Vendor"),
                    NoPO = GetValue(values, headers, "NoPO"),
                    MerkType = GetValue(values, headers, "MerkType"),
                    Size = GetValue(values, headers, "Size"),
                    Qty = decimal.TryParse(GetValue(values, headers, "Qty"), out var qty) ? qty : 0,
                    UnitPrice = decimal.TryParse(GetValue(values, headers, "UnitPrice"), out var price) ? price : 0,
                    TotalPrice = decimal.TryParse(GetValue(values, headers, "TotalPrice"), out var total) ? total : 0,
                    Status = GetValue(values, headers, "Status")
                };
                db.TyrePOs.Add(tyrePo);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Uploaded {insertedCount} records", inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// Upload Tyre Problem
app.MapPost("/api/tyres/problem-upload", async (HttpRequest request, AppDbContext db) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Invalid form content type" });

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var site = form["site"].ToString();

    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file uploaded" });

    try
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var tanggalStr = GetValue(values, headers, "Tanggal");
                DateTime tanggal = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(tanggalStr))
                {
                    if (DateTime.TryParseExact(tanggalStr, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalId))
                        tanggal = DateTime.SpecifyKind(tanggalId, DateTimeKind.Unspecified);
                    else if (DateTime.TryParseExact(tanggalStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var tanggalIso))
                        tanggal = DateTime.SpecifyKind(tanggalIso, DateTimeKind.Unspecified);
                }

                var tyreProblem = new TyreProblem
                {
                    No = int.TryParse(GetValue(values, headers, "No"), out var no) ? no : i,
                    Tanggal = tanggal,
                    Site = site,
                    UnitNo = GetValue(values, headers, "UnitNo"),
                    SerialNumber = GetValue(values, headers, "SerialNumber"),
                    MerkType = GetValue(values, headers, "MerkType"),
                    Size = GetValue(values, headers, "Size"),
                    Problem = GetValue(values, headers, "Problem"),
                    Kerusakan = GetValue(values, headers, "Kerusakan"),
                    Location = GetValue(values, headers, "Location"),
                    StartHM = decimal.TryParse(GetValue(values, headers, "StartHM"), out var startHM) ? startHM : 0,
                    EndHM = decimal.TryParse(GetValue(values, headers, "EndHM"), out var endHM) ? endHM : 0,
                    TotalHM = decimal.TryParse(GetValue(values, headers, "TotalHM"), out var totalHM) ? totalHM : 0
                };
                db.TyreProblems.Add(tyreProblem);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Uploaded {insertedCount} records", inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// ==================== INVENTORY API ENDPOINTS ====================

// Get all inventory items
app.MapGet("/api/inventories", async (AppDbContext db, string? location = null, string? alert = null) =>
{
    var query = db.Inventories.AsQueryable();

    if (!string.IsNullOrEmpty(location) && location.ToLower() != "all")
    {
        query = query.Where(i => i.Location == location);
    }

    var items = await query.OrderBy(i => i.PartNumber).ToListAsync();

    // Filter by alert if requested
    if (alert == "min")
    {
        items = items.Where(i => i.Stock <= i.MinStock).ToList();
    }
    else if (alert == "max")
    {
        items = items.Where(i => i.Stock >= i.MaxStock).ToList();
    }

    return Results.Ok(items);
});

// Get inventory locations for filter
app.MapGet("/api/inventories/locations", async (AppDbContext db) =>
{
    var locations = await db.Inventories.Select(i => i.Location).Distinct().OrderBy(l => l).ToListAsync();
    return Results.Ok(locations);
});

// Get inventory statistics
app.MapGet("/api/inventories/stats", async (AppDbContext db) =>
{
    var items = await db.Inventories.ToListAsync();
    var totalItems = items.Count;
    var minStockAlert = items.Count(i => i.Stock <= i.MinStock);
    var maxStockAlert = items.Count(i => i.Stock >= i.MaxStock);
    var totalStockValue = items.Sum(i => i.StockValue);
    var lowStock = items.Where(i => i.Stock <= i.MinStock).ToList();

    return Results.Ok(new
    {
        totalItems,
        minStockAlert,
        maxStockAlert,
        totalStockValue,
        lowStockItems = lowStock
    });
});

// Add inventory item
app.MapPost("/api/inventories", async (Inventory inventory, AppDbContext db) =>
{
    try
    {
        // === VALIDASI NEGATIF ===
        if (inventory.Stock < 0) return Results.BadRequest(new { error = "Stock tidak boleh negatif" });
        if (inventory.MinStock < 0) return Results.BadRequest(new { error = "MinStock tidak boleh negatif" });
        if (inventory.MaxStock < 0) return Results.BadRequest(new { error = "MaxStock tidak boleh negatif" });
        if (inventory.UnitPrice < 0) return Results.BadRequest(new { error = "UnitPrice tidak boleh negatif" });
        if (inventory.QtyMinAlert < 0) return Results.BadRequest(new { error = "QtyMinAlert tidak boleh negatif" });
        if (inventory.MinStock > inventory.MaxStock) return Results.BadRequest(new { error = $"MinStock ({inventory.MinStock}) tidak boleh lebih besar dari MaxStock ({inventory.MaxStock})" });

        // Calculate stock value
        inventory.StockValue = inventory.Stock * inventory.UnitPrice;
        inventory.CreatedAt = DateTime.UtcNow;
        inventory.UpdatedAt = DateTime.UtcNow;

        db.Inventories.Add(inventory);
        await db.SaveChangesAsync();
        return Results.Ok(inventory);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Update inventory item
app.MapPut("/api/inventories/{id}", async (int id, Inventory updated, AppDbContext db) =>
{
    try
    {
        var inventory = await db.Inventories.FindAsync(id);
        if (inventory == null)
            return Results.NotFound(new { error = "Item not found" });

        inventory.PartNumber = updated.PartNumber;
        inventory.MaterialDescription = updated.MaterialDescription;
        inventory.Unit = updated.Unit;
        inventory.Location = updated.Location;
        inventory.MinStock = updated.MinStock;
        inventory.MaxStock = updated.MaxStock;
        inventory.Stock = updated.Stock;
        inventory.QtyMinAlert = updated.QtyMinAlert;
        inventory.UnitPrice = updated.UnitPrice;
        inventory.LastPOPrice = updated.LastPOPrice;
        inventory.StockValue = updated.Stock * updated.UnitPrice;
        inventory.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(inventory);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Delete inventory item
app.MapDelete("/api/inventories/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var inventory = await db.Inventories.FindAsync(id);
        if (inventory == null)
            return Results.NotFound(new { error = "Item not found" });

        db.Inventories.Remove(inventory);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Item deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Bulk upload inventory from CSV
app.MapPost("/api/inventories/upload", async (HttpRequest request, AppDbContext db) =>
{
    try
    {
        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        var location = form["location"].FirstOrDefault() ?? "DEFAULT";

        if (file == null || file.Length == 0)
            return Results.BadRequest(new { error = "No file uploaded" });

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var stock = decimal.TryParse(GetValue(values, headers, "Stock"), out var s) ? s : 0;
                var unitPrice = decimal.TryParse(GetValue(values, headers, "UnitPrice"), out var up) ? up : 0;

                var inventory = new Inventory
                {
                    PartNumber = GetValue(values, headers, "PartNumber"),
                    MaterialDescription = GetValue(values, headers, "MaterialDescription"),
                    Unit = GetValue(values, headers, "Unit"),
                    Location = string.IsNullOrEmpty(GetValue(values, headers, "Location")) ? location : GetValue(values, headers, "Location"),
                    MinStock = decimal.TryParse(GetValue(values, headers, "MinStock"), out var min) ? min : 0,
                    MaxStock = decimal.TryParse(GetValue(values, headers, "MaxStock"), out var max) ? max : 0,
                    Stock = stock,
                    QtyMinAlert = decimal.TryParse(GetValue(values, headers, "QtyMinAlert"), out var qtyMin) ? qtyMin : 0,
                    UnitPrice = unitPrice,
                    LastPOPrice = decimal.TryParse(GetValue(values, headers, "LastPOPrice"), out var lastPO) ? lastPO : 0,
                    StockValue = stock * unitPrice,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Inventories.Add(inventory);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Uploaded {insertedCount} records", inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// ==================== PURCHASE REQUEST API ENDPOINTS ====================

// Get all purchase requests
app.MapGet("/api/purchase-requests", async (AppDbContext db, string? site = null, string? status = null) =>
{
    var query = db.PurchaseRequests.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
    {
        query = query.Where(pr => pr.Site == site);
    }

    if (!string.IsNullOrEmpty(status) && status != "all")
    {
        query = query.Where(pr => pr.Status == status);
    }

    var requests = await query.OrderByDescending(pr => pr.PRDate).ToListAsync();

    // Get items for each request
    var requestIds = requests.Select(r => r.Id).ToList();
    var allItems = await db.PurchaseRequestItems
        .Where(i => requestIds.Contains(i.PurchaseRequestId))
        .ToListAsync();

    var result = requests.Select(pr => new
    {
        pr.Id,
        pr.PRNumber,
        pr.PRDate,
        pr.Site,
        pr.Department,
        pr.RequestedBy,
        pr.ApprovedBy,
        pr.Status,
        pr.Remarks,
        Items = allItems.Where(i => i.PurchaseRequestId == pr.Id).ToList(),
        TotalAmount = allItems.Where(i => i.PurchaseRequestId == pr.Id).Sum(i => i.TotalPrice)
    }).ToList();

    return Results.Ok(result);
});

// Get single purchase request with items
app.MapGet("/api/purchase-requests/{id}", async (int id, AppDbContext db) =>
{
    var pr = await db.PurchaseRequests.FindAsync(id);
    if (pr == null)
        return Results.NotFound(new { error = "Purchase Request not found" });

    var items = await db.PurchaseRequestItems
        .Where(i => i.PurchaseRequestId == id)
        .ToListAsync();

    return Results.Ok(new
    {
        pr.Id,
        pr.PRNumber,
        pr.PRDate,
        pr.Site,
        pr.Department,
        pr.RequestedBy,
        pr.ApprovedBy,
        pr.Status,
        pr.Remarks,
        Items = items,
        TotalAmount = items.Sum(i => i.TotalPrice)
    });
});

// Create purchase request with items
app.MapPost("/api/purchase-requests", async (HttpContext context, AppDbContext db) =>
{
    try
    {
        var body = await context.Request.ReadFromJsonAsync<PurchaseRequestInput>();
        if (body == null)
            return Results.BadRequest(new { error = "Invalid data" });

        // Generate PR Number
        var prCount = await db.PurchaseRequests.CountAsync() + 1;
        var prNumber = $"PR-{DateTime.Now:yyyyMMdd}-{prCount:D4}";

        var pr = new PurchaseRequest
        {
            PRNumber = prNumber,
            PRDate = body.PRDate ?? DateTime.Now,
            Site = body.Site,
            Department = body.Department,
            RequestedBy = body.RequestedBy ?? "",
            ApprovedBy = body.ApprovedBy ?? "",
            Status = body.Status ?? "DRAFT",
            Remarks = body.Remarks ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.PurchaseRequests.Add(pr);
        await db.SaveChangesAsync();

        // Add items
        if (body.Items != null && body.Items.Count > 0)
        {
            foreach (var item in body.Items)
            {
                var prItem = new PurchaseRequestItem
                {
                    PurchaseRequestId = pr.Id,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    EstimatedPrice = item.EstimatedPrice,
                    TotalPrice = item.Quantity * item.EstimatedPrice,
                    Purpose = item.Purpose ?? "",
                    Priority = item.Priority ?? "NORMAL",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.PurchaseRequestItems.Add(prItem);
            }
            await db.SaveChangesAsync();
        }

        // Fetch complete data
        var result = await db.PurchaseRequests.FindAsync(pr.Id);
        var items = await db.PurchaseRequestItems.Where(i => i.PurchaseRequestId == pr.Id).ToListAsync();

        return Results.Ok(new
        {
            result!.Id,
            result.PRNumber,
            result.PRDate,
            result.Site,
            result.Department,
            result.RequestedBy,
            result.ApprovedBy,
            result.Status,
            result.Remarks,
            Items = items
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Update purchase request
app.MapPut("/api/purchase-requests/{id}", async (int id, HttpContext context, AppDbContext db) =>
{
    try
    {
        var pr = await db.PurchaseRequests.FindAsync(id);
        if (pr == null)
            return Results.NotFound(new { error = "Purchase Request not found" });

        var body = await context.Request.ReadFromJsonAsync<PurchaseRequestInput>();

        pr.PRDate = body?.PRDate ?? pr.PRDate;
        pr.Site = body?.Site ?? pr.Site;
        pr.Department = body?.Department ?? pr.Department;
        pr.RequestedBy = body?.RequestedBy ?? pr.RequestedBy;
        pr.ApprovedBy = body?.ApprovedBy ?? pr.ApprovedBy;
        pr.Status = body?.Status ?? pr.Status;
        pr.Remarks = body?.Remarks ?? pr.Remarks;
        pr.UpdatedAt = DateTime.UtcNow;

        // Update items if provided
        if (body?.Items != null)
        {
            // Remove existing items
            var existingItems = await db.PurchaseRequestItems
                .Where(i => i.PurchaseRequestId == id)
                .ToListAsync();
            db.PurchaseRequestItems.RemoveRange(existingItems);

            // Add new items
            foreach (var item in body.Items)
            {
                var prItem = new PurchaseRequestItem
                {
                    PurchaseRequestId = pr.Id,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    EstimatedPrice = item.EstimatedPrice,
                    TotalPrice = item.Quantity * item.EstimatedPrice,
                    Purpose = item.Purpose ?? "",
                    Priority = item.Priority ?? "NORMAL",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.PurchaseRequestItems.Add(prItem);
            }
        }

        await db.SaveChangesAsync();
        return Results.Ok(pr);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Delete purchase request
app.MapDelete("/api/purchase-requests/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var pr = await db.PurchaseRequests.FindAsync(id);
        if (pr == null)
            return Results.NotFound(new { error = "Purchase Request not found" });

        // Delete items first
        var items = await db.PurchaseRequestItems.Where(i => i.PurchaseRequestId == id).ToListAsync();
        db.PurchaseRequestItems.RemoveRange(items);

        db.PurchaseRequests.Remove(pr);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Purchase Request deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Get PR statistics
app.MapGet("/api/purchase-requests/stats", async (AppDbContext db) =>
{
    var total = await db.PurchaseRequests.CountAsync();
    var draft = await db.PurchaseRequests.CountAsync(pr => pr.Status == "DRAFT");
    var pending = await db.PurchaseRequests.CountAsync(pr => pr.Status == "PENDING");
    var approved = await db.PurchaseRequests.CountAsync(pr => pr.Status == "APPROVED");
    var rejected = await db.PurchaseRequests.CountAsync(pr => pr.Status == "REJECTED");

    var allItems = await db.PurchaseRequestItems.ToListAsync();
    var totalAmount = allItems.Sum(i => i.TotalPrice);

    return Results.Ok(new
    {
        total,
        draft,
        pending,
        approved,
        rejected,
        totalAmount
    });
});

// Upload PR from CSV
app.MapPost("/api/purchase-requests/upload", async (HttpRequest request, AppDbContext db) =>
{
    try
    {
        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        var site = form["site"].FirstOrDefault() ?? "DEFAULT";
        var department = form["department"].FirstOrDefault() ?? "Warehouse";

        if (file == null || file.Length == 0)
            return Results.BadRequest(new { error = "No file uploaded" });

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        // Generate PR Number
        var prCount = await db.PurchaseRequests.CountAsync() + 1;
        var prNumber = $"PR-{DateTime.Now:yyyyMMdd}-{prCount:D4}";

        var pr = new PurchaseRequest
        {
            PRNumber = prNumber,
            PRDate = DateTime.Now,
            Site = site,
            Department = department,
            Status = "DRAFT",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.PurchaseRequests.Add(pr);
        await db.SaveChangesAsync();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var qty = decimal.TryParse(GetValue(values, headers, "Quantity"), out var q) ? q : 0;
                var price = decimal.TryParse(GetValue(values, headers, "EstimatedPrice"), out var p) ? p : 0;

                var item = new PurchaseRequestItem
                {
                    PurchaseRequestId = pr.Id,
                    PartNumber = GetValue(values, headers, "PartNumber"),
                    Description = GetValue(values, headers, "Description"),
                    Unit = GetValue(values, headers, "Unit"),
                    Quantity = qty,
                    EstimatedPrice = price,
                    TotalPrice = qty * price,
                    Purpose = GetValue(values, headers, "Purpose"),
                    Priority = GetValue(values, headers, "Priority") ?? "NORMAL",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.PurchaseRequestItems.Add(item);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Created PR {prNumber} with {insertedCount} items", prNumber, inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// ==================== PURCHASE ORDER API ENDPOINTS ====================

// Get all purchase orders
app.MapGet("/api/purchase-orders", async (AppDbContext db, string? site = null, string? status = null) =>
{
    var query = db.PurchaseOrders.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
        query = query.Where(po => po.Site == site);

    if (!string.IsNullOrEmpty(status) && status != "all")
        query = query.Where(po => po.Status == status);

    var orders = await query.OrderByDescending(po => po.PODate).ToListAsync();

    var orderIds = orders.Select(o => o.Id).ToList();
    var allItems = await db.PurchaseOrderItems.Where(i => orderIds.Contains(i.PurchaseOrderId)).ToListAsync();

    var result = orders.Select(po => new
    {
        po.Id, po.PONumber, po.PODate, po.Site, po.Vendor, po.VendorCode,
        po.PRNumber, po.Status, po.DeliveryDate, po.DeliveryAddress,
        po.SubTotal, po.Tax, po.Discount, po.TotalAmount, po.PaymentTerms,
        po.Remarks, po.ApprovedBy,
        Items = allItems.Where(i => i.PurchaseOrderId == po.Id).ToList()
    }).ToList();

    return Results.Ok(result);
});

app.MapGet("/api/purchase-orders/{id}", async (int id, AppDbContext db) =>
{
    var po = await db.PurchaseOrders.FindAsync(id);
    if (po == null) return Results.NotFound(new { error = "Purchase Order not found" });

    var items = await db.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id).ToListAsync();

    return Results.Ok(new
    {
        po.Id, po.PONumber, po.PODate, po.Site, po.Vendor, po.VendorCode,
        po.PRNumber, po.Status, po.DeliveryDate, po.DeliveryAddress,
        po.SubTotal, po.Tax, po.Discount, po.TotalAmount, po.PaymentTerms,
        po.Remarks, po.ApprovedBy, Items = items
    });
});

app.MapPost("/api/purchase-orders", async (HttpContext context, AppDbContext db) =>
{
    try
    {
        var body = await context.Request.ReadFromJsonAsync<PurchaseOrderInput>();
        if (body == null) return Results.BadRequest(new { error = "Invalid data" });

        var poCount = await db.PurchaseOrders.CountAsync() + 1;
        var poNumber = $"PO-{DateTime.Now:yyyyMMdd}-{poCount:D4}";

        var items = body.Items ?? new List<POItemInput>();
        var subTotal = items.Sum(i => i.Quantity * i.UnitPrice);

        var po = new PurchaseOrder
        {
            PONumber = poNumber,
            PODate = body.PODate ?? DateTime.Now,
            Site = body.Site,
            Vendor = body.Vendor,
            VendorCode = body.VendorCode ?? "",
            PRNumber = body.PRNumber ?? "",
            Status = body.Status ?? "DRAFT",
            DeliveryDate = body.DeliveryDate,
            DeliveryAddress = body.DeliveryAddress ?? "",
            SubTotal = subTotal,
            Tax = body.Tax,
            Discount = body.Discount,
            TotalAmount = subTotal + body.Tax - body.Discount,
            PaymentTerms = body.PaymentTerms ?? "",
            Remarks = body.Remarks ?? "",
            ApprovedBy = body.ApprovedBy ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.PurchaseOrders.Add(po);
        await db.SaveChangesAsync();

        foreach (var item in items)
        {
            db.PurchaseOrderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = po.Id,
                PartNumber = item.PartNumber,
                Description = item.Description,
                Unit = item.Unit,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice,
                DeliveredQty = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        return Results.Ok(po);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/purchase-orders/{id}", async (int id, HttpContext context, AppDbContext db) =>
{
    try
    {
        var po = await db.PurchaseOrders.FindAsync(id);
        if (po == null) return Results.NotFound(new { error = "Purchase Order not found" });

        var body = await context.Request.ReadFromJsonAsync<PurchaseOrderInput>();

        po.PODate = body?.PODate ?? po.PODate;
        po.Site = body?.Site ?? po.Site;
        po.Vendor = body?.Vendor ?? po.Vendor;
        po.VendorCode = body?.VendorCode ?? po.VendorCode;
        po.PRNumber = body?.PRNumber ?? po.PRNumber;
        po.Status = body?.Status ?? po.Status;
        po.DeliveryDate = body?.DeliveryDate;
        po.DeliveryAddress = body?.DeliveryAddress ?? po.DeliveryAddress;
        po.Tax = body?.Tax ?? po.Tax;
        po.Discount = body?.Discount ?? po.Discount;
        po.PaymentTerms = body?.PaymentTerms ?? po.PaymentTerms;
        po.Remarks = body?.Remarks ?? po.Remarks;
        po.ApprovedBy = body?.ApprovedBy ?? po.ApprovedBy;
        po.UpdatedAt = DateTime.UtcNow;

        if (body?.Items != null)
        {
            var existingItems = await db.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id).ToListAsync();
            db.PurchaseOrderItems.RemoveRange(existingItems);

            foreach (var item in body.Items)
            {
                db.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Unit = item.Unit,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice,
                    DeliveredQty = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        var allItems = await db.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id).ToListAsync();
        po.SubTotal = allItems.Sum(i => i.TotalPrice);
        po.TotalAmount = po.SubTotal + po.Tax - po.Discount;

        await db.SaveChangesAsync();
        return Results.Ok(po);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/purchase-orders/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var po = await db.PurchaseOrders.FindAsync(id);
        if (po == null) return Results.NotFound(new { error = "Purchase Order not found" });

        var items = await db.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id).ToListAsync();
        db.PurchaseOrderItems.RemoveRange(items);
        db.PurchaseOrders.Remove(po);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Purchase Order deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/purchase-orders/stats", async (AppDbContext db) =>
{
    var total = await db.PurchaseOrders.CountAsync();
    var draft = await db.PurchaseOrders.CountAsync(po => po.Status == "DRAFT");
    var approved = await db.PurchaseOrders.CountAsync(po => po.Status == "APPROVED");
    var completed = await db.PurchaseOrders.CountAsync(po => po.Status == "COMPLETED");
    var cancelled = await db.PurchaseOrders.CountAsync(po => po.Status == "CANCELLED");
    var totalAmount = db.PurchaseOrders.Sum(po => po.TotalAmount);

    return Results.Ok(new { total, draft, approved, completed, cancelled, totalAmount });
});

// ==================== GOOD RECEIPT API ENDPOINTS ====================

app.MapGet("/api/good-receipts", async (AppDbContext db, string? site = null, string? status = null) =>
{
    var query = db.GoodReceipts.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
        query = query.Where(gr => gr.Site == site);

    if (!string.IsNullOrEmpty(status) && status != "all")
        query = query.Where(gr => gr.Status == status);

    var receipts = await query.OrderByDescending(gr => gr.GRDate).ToListAsync();

    var receiptIds = receipts.Select(r => r.Id).ToList();
    var allItems = await db.GoodReceiptItems.Where(i => receiptIds.Contains(i.GoodReceiptId)).ToListAsync();

    var result = receipts.Select(gr => new
    {
        gr.Id, gr.GRNumber, gr.GRDate, gr.Site, gr.Vendor, gr.PONumber,
        gr.Status, gr.ReceivedBy, gr.ApprovedBy, gr.Remarks,
        Items = allItems.Where(i => i.GoodReceiptId == gr.Id).ToList()
    }).ToList();

    return Results.Ok(result);
});

app.MapGet("/api/good-receipts/{id}", async (int id, AppDbContext db) =>
{
    var gr = await db.GoodReceipts.FindAsync(id);
    if (gr == null) return Results.NotFound(new { error = "Good Receipt not found" });

    var items = await db.GoodReceiptItems.Where(i => i.GoodReceiptId == id).ToListAsync();

    return Results.Ok(new
    {
        gr.Id, gr.GRNumber, gr.GRDate, gr.Site, gr.Vendor, gr.PONumber,
        gr.Status, gr.ReceivedBy, gr.ApprovedBy, gr.Remarks, Items = items
    });
});

app.MapPost("/api/good-receipts", async (HttpContext context, AppDbContext db) =>
{
    try
    {
        var body = await context.Request.ReadFromJsonAsync<GoodReceiptInput>();
        if (body == null) return Results.BadRequest(new { error = "Invalid data" });

        var grCount = await db.GoodReceipts.CountAsync() + 1;
        var grNumber = $"GR-{DateTime.Now:yyyyMMdd}-{grCount:D4}";

        var gr = new GoodReceipt
        {
            GRNumber = grNumber,
            GRDate = body.GRDate ?? DateTime.Now,
            Site = body.Site,
            Vendor = body.Vendor,
            PONumber = body.PONumber ?? "",
            Status = body.Status ?? "RECEIVED",
            ReceivedBy = body.ReceivedBy ?? "",
            ApprovedBy = body.ApprovedBy ?? "",
            Remarks = body.Remarks ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.GoodReceipts.Add(gr);
        await db.SaveChangesAsync();

        if (body.Items != null)
        {
            foreach (var item in body.Items)
            {
                db.GoodReceiptItems.Add(new GoodReceiptItem
                {
                    GoodReceiptId = gr.Id,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Unit = item.Unit,
                    OrderedQty = item.OrderedQty,
                    ReceivedQty = item.ReceivedQty,
                    AcceptedQty = item.AcceptedQty,
                    RejectedQty = item.RejectedQty,
                    Location = item.Location ?? "",
                    Remarks = item.Remarks ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                // Update inventory stock
                var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.PartNumber == item.PartNumber && i.Location == body.Site);
                if (inventory != null)
                {
                    inventory.Stock += item.AcceptedQty;
                    inventory.UpdatedAt = DateTime.UtcNow;
                }
            }
            await db.SaveChangesAsync();
        }

        return Results.Ok(gr);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/good-receipts/{id}", async (int id, HttpContext context, AppDbContext db) =>
{
    try
    {
        var gr = await db.GoodReceipts.FindAsync(id);
        if (gr == null) return Results.NotFound(new { error = "Good Receipt not found" });

        var body = await context.Request.ReadFromJsonAsync<GoodReceiptInput>();

        gr.GRDate = body?.GRDate ?? gr.GRDate;
        gr.Site = body?.Site ?? gr.Site;
        gr.Vendor = body?.Vendor ?? gr.Vendor;
        gr.PONumber = body?.PONumber ?? gr.PONumber;
        gr.Status = body?.Status ?? gr.Status;
        gr.ReceivedBy = body?.ReceivedBy ?? gr.ReceivedBy;
        gr.ApprovedBy = body?.ApprovedBy ?? gr.ApprovedBy;
        gr.Remarks = body?.Remarks ?? gr.Remarks;
        gr.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(gr);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/good-receipts/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var gr = await db.GoodReceipts.FindAsync(id);
        if (gr == null) return Results.NotFound(new { error = "Good Receipt not found" });

        var items = await db.GoodReceiptItems.Where(i => i.GoodReceiptId == id).ToListAsync();
        db.GoodReceiptItems.RemoveRange(items);
        db.GoodReceipts.Remove(gr);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Good Receipt deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/good-receipts/stats", async (AppDbContext db) =>
{
    var total = await db.GoodReceipts.CountAsync();
    var received = await db.GoodReceipts.CountAsync(gr => gr.Status == "RECEIVED");
    var inspected = await db.GoodReceipts.CountAsync(gr => gr.Status == "INSPECTED");
    var completed = await db.GoodReceipts.CountAsync(gr => gr.Status == "COMPLETED");

    return Results.Ok(new { total, received, inspected, completed });
});

// ==================== GOOD ISSUE API ENDPOINTS ====================

app.MapGet("/api/good-issues", async (AppDbContext db, string? site = null, string? status = null) =>
{
    var query = db.GoodIssues.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
        query = query.Where(gi => gi.Site == site);

    if (!string.IsNullOrEmpty(status) && status != "all")
        query = query.Where(gi => gi.Status == status);

    var issues = await query.OrderByDescending(gi => gi.GIDate).ToListAsync();

    var issueIds = issues.Select(i => i.Id).ToList();
    var allItems = await db.GoodIssueItems.Where(i => issueIds.Contains(i.GoodIssueId)).ToListAsync();

    var result = issues.Select(gi => new
    {
        gi.Id, gi.GINumber, gi.GIDate, gi.Site, gi.Department,
        gi.RequestNumber, gi.Status, gi.IssuedBy, gi.ReceivedBy,
        gi.ApprovedBy, gi.Remarks,
        Items = allItems.Where(i => i.GoodIssueId == gi.Id).ToList()
    }).ToList();

    return Results.Ok(result);
});

app.MapGet("/api/good-issues/{id}", async (int id, AppDbContext db) =>
{
    var gi = await db.GoodIssues.FindAsync(id);
    if (gi == null) return Results.NotFound(new { error = "Good Issue not found" });

    var items = await db.GoodIssueItems.Where(i => i.GoodIssueId == id).ToListAsync();

    return Results.Ok(new
    {
        gi.Id, gi.GINumber, gi.GIDate, gi.Site, gi.Department,
        gi.RequestNumber, gi.Status, gi.IssuedBy, gi.ReceivedBy,
        gi.ApprovedBy, gi.Remarks, Items = items
    });
});

app.MapPost("/api/good-issues", async (HttpContext context, AppDbContext db) =>
{
    try
    {
        var body = await context.Request.ReadFromJsonAsync<GoodIssueInput>();
        if (body == null) return Results.BadRequest(new { error = "Invalid data" });

        var giCount = await db.GoodIssues.CountAsync() + 1;
        var giNumber = $"GI-{DateTime.Now:yyyyMMdd}-{giCount:D4}";

        var gi = new GoodIssue
        {
            GINumber = giNumber,
            GIDate = body.GIDate ?? DateTime.Now,
            Site = body.Site,
            Department = body.Department ?? "",
            RequestNumber = body.RequestNumber ?? "",
            Status = body.Status ?? "ISSUED",
            IssuedBy = body.IssuedBy ?? "",
            ReceivedBy = body.ReceivedBy ?? "",
            ApprovedBy = body.ApprovedBy ?? "",
            Remarks = body.Remarks ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.GoodIssues.Add(gi);
        await db.SaveChangesAsync();

        if (body.Items != null)
        {
            foreach (var item in body.Items)
            {
                // === VALIDASI NEGATIF ===
                if (item.RequestedQty < 0) return Results.BadRequest(new { error = $"RequestedQty tidak boleh negatif untuk part {item.PartNumber}" });
                if (item.IssuedQty < 0) return Results.BadRequest(new { error = $"IssuedQty tidak boleh negatif untuk part {item.PartNumber}" });

                // Get current stock
                var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.PartNumber == item.PartNumber && i.Location == body.Site);
                if (inventory == null) return Results.BadRequest(new { error = $"Part {item.PartNumber} tidak ditemukan di lokasi {body.Site}" });

                var stockBefore = inventory.Stock;
                var issuedQty = item.IssuedQty;

                // === VALIDASI STOCK: TIDAK BOLEH MELEBIHI STOK ===
                if (issuedQty > stockBefore)
                    return Results.BadRequest(new
                    {
                        error = $"STOCK INSUFFICIENT! Part {item.PartNumber}: Diminta {issuedQty}, Tersedia {stockBefore}. " +
                                $"Pengeluaran tidak bisa melebihi stok yang ada!"
                    });

                db.GoodIssueItems.Add(new GoodIssueItem
                {
                    GoodIssueId = gi.Id,
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Unit = item.Unit,
                    RequestedQty = item.RequestedQty,
                    IssuedQty = issuedQty,
                    StockBefore = stockBefore,
                    StockAfter = stockBefore - issuedQty,
                    Purpose = item.Purpose ?? "",
                    Remarks = item.Remarks ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                // Update inventory stock
                inventory.Stock -= issuedQty;
                inventory.UpdatedAt = DateTime.UtcNow;
            }
            await db.SaveChangesAsync();
        }

        return Results.Ok(gi);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/good-issues/{id}", async (int id, HttpContext context, AppDbContext db) =>
{
    try
    {
        var gi = await db.GoodIssues.FindAsync(id);
        if (gi == null) return Results.NotFound(new { error = "Good Issue not found" });

        var body = await context.Request.ReadFromJsonAsync<GoodIssueInput>();

        gi.GIDate = body?.GIDate ?? gi.GIDate;
        gi.Site = body?.Site ?? gi.Site;
        gi.Department = body?.Department ?? gi.Department;
        gi.RequestNumber = body?.RequestNumber ?? gi.RequestNumber;
        gi.Status = body?.Status ?? gi.Status;
        gi.IssuedBy = body?.IssuedBy ?? gi.IssuedBy;
        gi.ReceivedBy = body?.ReceivedBy ?? gi.ReceivedBy;
        gi.ApprovedBy = body?.ApprovedBy ?? gi.ApprovedBy;
        gi.Remarks = body?.Remarks ?? gi.Remarks;
        gi.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(gi);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/good-issues/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var gi = await db.GoodIssues.FindAsync(id);
        if (gi == null) return Results.NotFound(new { error = "Good Issue not found" });

        var items = await db.GoodIssueItems.Where(i => i.GoodIssueId == id).ToListAsync();

        // Restore inventory stock
        foreach (var item in items)
        {
            var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.PartNumber == item.PartNumber && i.Location == gi.Site);
            if (inventory != null)
            {
                inventory.Stock += item.IssuedQty;
                inventory.UpdatedAt = DateTime.UtcNow;
            }
        }

        db.GoodIssueItems.RemoveRange(items);
        db.GoodIssues.Remove(gi);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Good Issue deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/good-issues/stats", async (AppDbContext db) =>
{
    var total = await db.GoodIssues.CountAsync();
    var issued = await db.GoodIssues.CountAsync(gi => gi.Status == "ISSUED");
    var completed = await db.GoodIssues.CountAsync(gi => gi.Status == "COMPLETED");
    var cancelled = await db.GoodIssues.CountAsync(gi => gi.Status == "CANCELLED");

    return Results.Ok(new { total, issued, completed, cancelled });
});

// ==================== VENDOR API ENDPOINTS ====================

app.MapGet("/api/vendors", async (AppDbContext db, string? category = null, string? status = null) =>
{
    var query = db.Vendors.AsQueryable();

    if (!string.IsNullOrEmpty(category) && category != "all")
        query = query.Where(v => v.Category == category);

    if (!string.IsNullOrEmpty(status) && status != "all")
        query = query.Where(v => v.Status == status);

    var vendors = await query.OrderBy(v => v.VendorName).ToListAsync();

    return Results.Ok(vendors);
});

app.MapGet("/api/vendors/{id}", async (int id, AppDbContext db) =>
{
    var vendor = await db.Vendors.FindAsync(id);
    if (vendor == null) return Results.NotFound(new { error = "Vendor not found" });
    return Results.Ok(vendor);
});

app.MapPost("/api/vendors", async (VendorInput input, AppDbContext db) =>
{
    try
    {
        var vendorCount = await db.Vendors.CountAsync() + 1;
        var vendorCode = string.IsNullOrEmpty(input.VendorCode)
            ? $"V{DateTime.Now:yyyyMMdd}{vendorCount:D4}"
            : input.VendorCode;

        var vendor = new Vendor
        {
            VendorCode = vendorCode,
            VendorName = input.VendorName,
            VendorType = input.VendorType ?? "SUPPLIER",
            Address = input.Address ?? "",
            City = input.City ?? "",
            Province = input.Province,
            PostalCode = input.PostalCode,
            Country = input.Country ?? "Indonesia",
            Phone = input.Phone ?? "",
            Fax = input.Fax,
            Email = input.Email ?? "",
            ContactPerson = input.ContactPerson ?? "",
            ContactPhone = input.ContactPhone,
            TaxId = input.TaxId ?? "",
            NIB = input.NIB,
            BankName = input.BankName,
            BankAccountName = input.BankAccountName,
            BankAccountNumber = input.BankAccountNumber,
            BankBranch = input.BankBranch,
            PaymentTerms = input.PaymentTerms ?? "",
            Category = input.Category,
            Status = input.Status ?? "ACTIVE",
            Remarks = input.Remarks ?? "",
            TotalPurchases = 0,
            OutstandingBalance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Vendors.Add(vendor);
        await db.SaveChangesAsync();
        return Results.Ok(vendor);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/vendors/{id}", async (int id, VendorInput input, AppDbContext db) =>
{
    try
    {
        var vendor = await db.Vendors.FindAsync(id);
        if (vendor == null) return Results.NotFound(new { error = "Vendor not found" });

        vendor.VendorCode = input.VendorCode ?? vendor.VendorCode;
        vendor.VendorName = input.VendorName;
        vendor.VendorType = input.VendorType ?? vendor.VendorType;
        vendor.Address = input.Address ?? vendor.Address;
        vendor.City = input.City ?? vendor.City;
        vendor.Province = input.Province ?? vendor.Province;
        vendor.PostalCode = input.PostalCode ?? vendor.PostalCode;
        vendor.Country = input.Country ?? vendor.Country;
        vendor.Phone = input.Phone ?? vendor.Phone;
        vendor.Fax = input.Fax ?? vendor.Fax;
        vendor.Email = input.Email ?? vendor.Email;
        vendor.ContactPerson = input.ContactPerson ?? vendor.ContactPerson;
        vendor.ContactPhone = input.ContactPhone ?? vendor.ContactPhone;
        vendor.TaxId = input.TaxId ?? vendor.TaxId;
        vendor.NIB = input.NIB ?? vendor.NIB;
        vendor.BankName = input.BankName ?? vendor.BankName;
        vendor.BankAccountName = input.BankAccountName ?? vendor.BankAccountName;
        vendor.BankAccountNumber = input.BankAccountNumber ?? vendor.BankAccountNumber;
        vendor.BankBranch = input.BankBranch ?? vendor.BankBranch;
        vendor.PaymentTerms = input.PaymentTerms ?? vendor.PaymentTerms;
        vendor.Category = input.Category;
        vendor.Status = input.Status ?? vendor.Status;
        vendor.Remarks = input.Remarks ?? vendor.Remarks;
        vendor.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(vendor);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/vendors/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var vendor = await db.Vendors.FindAsync(id);
        if (vendor == null) return Results.NotFound(new { error = "Vendor not found" });

        db.Vendors.Remove(vendor);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Vendor deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/vendors/stats", async (AppDbContext db) =>
{
    var total = await db.Vendors.CountAsync();
    var active = await db.Vendors.CountAsync(v => v.Status == "ACTIVE");
    var inactive = await db.Vendors.CountAsync(v => v.Status == "INACTIVE");
    var totalPurchases = db.Vendors.Sum(v => v.TotalPurchases);

    return Results.Ok(new { total, active, inactive, totalPurchases });
});

app.MapGet("/api/vendors/categories", async (AppDbContext db) =>
{
    var categories = await db.Vendors.Select(v => v.Category).Distinct().OrderBy(c => c).ToListAsync();
    return Results.Ok(categories);
});

// ==================== FLEET OVERVIEW API ENDPOINTS ====================

app.MapGet("/api/fleet", async (AppDbContext db, string? site = null, string? status = null, string? category = null) =>
{
    var query = db.FleetVehicles.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
        query = query.Where(f => f.Site == site);

    if (!string.IsNullOrEmpty(status) && status != "all")
        query = query.Where(f => f.Status == status);

    if (!string.IsNullOrEmpty(category) && category != "all")
        query = query.Where(f => f.Category == category);

    var vehicles = await query.OrderBy(f => f.UnitNo).ToListAsync();

    return Results.Ok(vehicles);
});

app.MapGet("/api/fleet/{id}", async (int id, AppDbContext db) =>
{
    var vehicle = await db.FleetVehicles.FindAsync(id);
    if (vehicle == null) return Results.NotFound(new { error = "Vehicle not found" });
    return Results.Ok(vehicle);
});

app.MapPost("/api/fleet", async (FleetVehicleInput input, AppDbContext db) =>
{
    try
    {
        // === VALIDASI REQUIRED ===
        if (string.IsNullOrWhiteSpace(input.UnitNo)) return Results.BadRequest(new { error = "UnitNo wajib diisi" });
        if (string.IsNullOrWhiteSpace(input.UnitDescription)) return Results.BadRequest(new { error = "UnitDescription wajib diisi" });
        if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });

        // === DUPLICATE CHECK ===
        var existing = await db.FleetVehicles.FirstOrDefaultAsync(v => v.UnitNo == input.UnitNo && v.Site == input.Site);
        if (existing != null) return Results.BadRequest(new { error = $"Unit '{input.UnitNo}' sudah terdaftar di site '{input.Site}'" });

        // === VALIDASI HM: HMakhir >= HMAwal ===
        if (input.HMakhir < input.HMAwal) return Results.BadRequest(new { error = $"HMakhir ({input.HMakhir}) tidak boleh lebih kecil dari HMAwal ({input.HMAwal}). Data HM tidak valid!" });
        if (input.KMakhir < input.KMAwal) return Results.BadRequest(new { error = $"KMakhir ({input.KMakhir}) tidak boleh lebih kecil dari KMAwal ({input.KMAwal}). Data KM tidak valid!" });

        // === VALIDASI NEGATIF ===
        if (input.HMAwal < 0) return Results.BadRequest(new { error = "HMAwal tidak boleh negatif" });
        if (input.KMAwal < 0) return Results.BadRequest(new { error = "KMAwal tidak boleh negatif" });
        if (input.HMakhir < 0) return Results.BadRequest(new { error = "HMakhir tidak boleh negatif" });
        if (input.KMakhir < 0) return Results.BadRequest(new { error = "KMakhir tidak boleh negatif" });
        if (input.TotalFuel < 0) return Results.BadRequest(new { error = "TotalFuel tidak boleh negatif" });
        if (input.TotalJam < 0) return Results.BadRequest(new { error = "TotalJam tidak boleh negatif" });
        if (input.TotalKM < 0) return Results.BadRequest(new { error = "TotalKM tidak boleh negatif" });
        if (input.GrossWeight < 0) return Results.BadRequest(new { error = "GrossWeight tidak boleh negatif" });
        if (input.TareWeight < 0) return Results.BadRequest(new { error = "TareWeight tidak boleh negatif" });
        if (input.PayloadCapacity < 0) return Results.BadRequest(new { error = "PayloadCapacity tidak boleh negatif" });
        if (input.MaxPayload < 0) return Results.BadRequest(new { error = "MaxPayload tidak boleh negatif" });
        if (input.AcquisitionCost < 0) return Results.BadRequest(new { error = "AcquisitionCost tidak boleh negatif" });
        if (input.DepreciationRate < 0 || input.DepreciationRate > 100) return Results.BadRequest(new { error = "DepreciationRate harus antara 0-100%" });
        if (input.BenchmarkLitrePerKM < 0) return Results.BadRequest(new { error = "BenchmarkLitrePerKM tidak boleh negatif" });
        if (input.BenchmarkLitrePerHour < 0) return Results.BadRequest(new { error = "BenchmarkLitrePerHour tidak boleh negatif" });
        if (input.FuelTankCapacity < 0) return Results.BadRequest(new { error = "FuelTankCapacity tidak boleh negatif" });
        if (input.TyreCostPerUnit < 0) return Results.BadRequest(new { error = "TyreCostPerUnit tidak boleh negatif" });

        // === VALIDASI WEIGHT SPEC ===
        if (input.GrossWeight > 0 && input.TareWeight > 0 && input.GrossWeight <= input.TareWeight)
            return Results.BadRequest(new { error = $"GrossWeight ({input.GrossWeight}) harus lebih besar dari TareWeight ({input.TareWeight})" });

        // Calculate totals
        var totalJam = input.HMakhir - input.HMAwal;
        var totalKM = input.KMakhir - input.KMAwal;
        var fuelRatio = totalJam > 0 ? (input.TotalFuel / totalJam) : 0;

        // Calculate days since installation (or default to 30 days)
        var days = 30m;
        if (input.PemasanganDate.HasValue)
        {
            days = Math.Max(1, (decimal)(DateTime.Now - input.PemasanganDate.Value).TotalDays);
        }
        var avgHMHari = totalJam / days;
        var avgKMHari = totalKM / days;

        var vehicle = new FleetVehicle
        {
            UnitNo = input.UnitNo,
            UnitDescription = input.UnitDescription,
            Site = input.Site,
            MerkType = input.MerkType ?? "",
            Category = input.Category ?? "",
            LicensePlate = input.LicensePlate,
            ChassisNumber = input.ChassisNumber,
            EngineNumber = input.EngineNumber,
            VehicleType = input.VehicleType ?? "OWNED",
            FuelType = input.FuelType,
            GrossWeight = input.GrossWeight,
            TareWeight = input.TareWeight,
            PayloadCapacity = input.PayloadCapacity,
            MaxPayload = input.MaxPayload,
            FuelTankCapacity = input.FuelTankCapacity,
            AvgFuelConsumption = input.AvgFuelConsumption,
            BenchmarkLitrePerKM = input.BenchmarkLitrePerKM,
            BenchmarkLitrePerHour = input.BenchmarkLitrePerHour,
            FuelCardNumber = input.FuelCardNumber,
            TyreSize = input.TyreSize,
            TyreQuantity = input.TyreQuantity ?? 0,
            TyreCostPerUnit = input.TyreCostPerUnit,
            AvgTyreLifeKM = input.AvgTyreLifeKM,
            PemasanganDate = input.PemasanganDate,
            HMAwal = input.HMAwal,
            KMAwal = input.KMAwal,
            HMakhir = input.HMakhir,
            KMakhir = input.KMakhir,
            TotalJam = totalJam,
            TotalKM = totalKM,
            FuelRatio = fuelRatio,
            AvgHMHari = avgHMHari,
            AvgKMHari = avgKMHari,
            HMUsage = input.HMUsage,
            TotalFuel = input.TotalFuel,
            CostCenter = input.CostCenter,
            RouteCode = input.RouteCode,
            AssignedDriverId = input.AssignedDriverId,
            AcquisitionCost = input.AcquisitionCost,
            DepreciationRate = input.DepreciationRate,
            AccumulatedDepreciation = input.AccumulatedDepreciation,
            BookValue = input.BookValue,
            UsefulLifeYear = input.UsefulLifeYear,
            InsuranceExpiry = input.InsuranceExpiry,
            TaxExpiry = input.TaxExpiry,
            KIRExpiry = input.KIRExpiry,
            STNKExpiry = input.STNKExpiry,
            Status = input.Status ?? "ACTIVE",
            Remarks = input.Remarks ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.FleetVehicles.Add(vehicle);
        await db.SaveChangesAsync();
        return Results.Ok(vehicle);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/fleet/{id}", async (int id, FleetVehicleInput input, AppDbContext db) =>
{
    try
    {
        var vehicle = await db.FleetVehicles.FindAsync(id);
        if (vehicle == null) return Results.NotFound(new { error = "Vehicle not found" });

        // === VALIDASI REQUIRED FIELDS ===
        if (string.IsNullOrWhiteSpace(input.UnitNo)) return Results.BadRequest(new { error = "UnitNo wajib diisi" });
        if (string.IsNullOrWhiteSpace(input.UnitDescription)) return Results.BadRequest(new { error = "UnitDescription wajib diisi" });
        if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });

        // === VALIDASI HM: HMakhir >= HMAwal ===
        if (input.HMakhir < input.HMAwal) return Results.BadRequest(new { error = $"HMakhir ({input.HMakhir}) tidak boleh lebih kecil dari HMAwal ({input.HMAwal})" });
        if (input.KMakhir < input.KMAwal) return Results.BadRequest(new { error = $"KMakhir ({input.KMakhir}) tidak boleh lebih kecil dari KMAwal ({input.KMAwal})" });

        // === VALIDASI TIDAK BOLEH NEGATIF ===
        if (input.HMAwal < 0) return Results.BadRequest(new { error = "HMAwal tidak boleh negatif" });
        if (input.KMAwal < 0) return Results.BadRequest(new { error = "KMAwal tidak boleh negatif" });
        if (input.HMakhir < 0) return Results.BadRequest(new { error = "HMakhir tidak boleh negatif" });
        if (input.KMakhir < 0) return Results.BadRequest(new { error = "KMakhir tidak boleh negatif" });
        if (input.HMUsage < 0) return Results.BadRequest(new { error = "HMUsage tidak boleh negatif" });
        if (input.TotalFuel < 0) return Results.BadRequest(new { error = "TotalFuel tidak boleh negatif" });
        if ((input.GrossWeight ?? 0) < 0) return Results.BadRequest(new { error = "GrossWeight tidak boleh negatif" });
        if ((input.TareWeight ?? 0) < 0) return Results.BadRequest(new { error = "TareWeight tidak boleh negatif" });
        if ((input.PayloadCapacity ?? 0) < 0) return Results.BadRequest(new { error = "PayloadCapacity tidak boleh negatif" });
        if ((input.MaxPayload ?? 0) < 0) return Results.BadRequest(new { error = "MaxPayload tidak boleh negatif" });
        if ((input.FuelTankCapacity ?? 0) < 0) return Results.BadRequest(new { error = "FuelTankCapacity tidak boleh negatif" });
        if ((input.BenchmarkLitrePerKM ?? 0) < 0) return Results.BadRequest(new { error = "BenchmarkLitrePerKM tidak boleh negatif" });
        if ((input.BenchmarkLitrePerHour ?? 0) < 0) return Results.BadRequest(new { error = "BenchmarkLitrePerHour tidak boleh negatif" });
        if ((input.AcquisitionCost ?? 0) < 0) return Results.BadRequest(new { error = "AcquisitionCost tidak boleh negatif" });
        if ((input.DepreciationRate ?? 0) < 0) return Results.BadRequest(new { error = "DepreciationRate tidak boleh negatif" });
        if (input.TyreQuantity < 0) return Results.BadRequest(new { error = "TyreQuantity tidak boleh negatif" });
        if ((input.TyreCostPerUnit ?? 0) < 0) return Results.BadRequest(new { error = "TyreCostPerUnit tidak boleh negatif" });

        // === VALIDASI WEIGHT SPEC ===
        if (input.GrossWeight > 0 && input.TareWeight > 0 && input.GrossWeight <= input.TareWeight)
            return Results.BadRequest(new { error = "GrossWeight harus lebih besar dari TareWeight" });

        // Calculate totals
        var totalJam = input.HMakhir - input.HMAwal;
        var totalKM = input.KMakhir - input.KMAwal;
        var fuelRatio = totalJam > 0 ? (input.TotalFuel / totalJam) : 0;

        var days = 30m;
        if (input.PemasanganDate.HasValue)
        {
            days = Math.Max(1, (decimal)(DateTime.Now - input.PemasanganDate.Value).TotalDays);
        }
        var avgHMHari = totalJam / days;
        var avgKMHari = totalKM / days;

        vehicle.UnitNo = input.UnitNo;
        vehicle.UnitDescription = input.UnitDescription;
        vehicle.Site = input.Site;
        vehicle.MerkType = input.MerkType ?? vehicle.MerkType;
        vehicle.Category = input.Category ?? vehicle.Category;
        vehicle.LicensePlate = input.LicensePlate;
        vehicle.VehicleType = input.VehicleType ?? vehicle.VehicleType;
        vehicle.GrossWeight = input.GrossWeight;
        vehicle.TareWeight = input.TareWeight;
        vehicle.PayloadCapacity = input.PayloadCapacity;
        vehicle.MaxPayload = input.MaxPayload;
        vehicle.BenchmarkLitrePerKM = input.BenchmarkLitrePerKM;
        vehicle.BenchmarkLitrePerHour = input.BenchmarkLitrePerHour;
        vehicle.FuelTankCapacity = input.FuelTankCapacity;
        vehicle.AcquisitionCost = input.AcquisitionCost;
        vehicle.DepreciationRate = input.DepreciationRate;
        vehicle.CostCenter = input.CostCenter;
        vehicle.AssignedDriverId = input.AssignedDriverId;
        vehicle.InsuranceExpiry = input.InsuranceExpiry;
        vehicle.TaxExpiry = input.TaxExpiry;
        vehicle.KIRExpiry = input.KIRExpiry;
        vehicle.STNKExpiry = input.STNKExpiry;
        vehicle.PemasanganDate = input.PemasanganDate;
        vehicle.HMAwal = input.HMAwal;
        vehicle.KMAwal = input.KMAwal;
        vehicle.HMakhir = input.HMakhir;
        vehicle.KMakhir = input.KMakhir;
        vehicle.TotalJam = totalJam;
        vehicle.TotalKM = totalKM;
        vehicle.FuelRatio = fuelRatio;
        vehicle.AvgHMHari = avgHMHari;
        vehicle.AvgKMHari = avgKMHari;
        vehicle.HMUsage = input.HMUsage;
        vehicle.TotalFuel = input.TotalFuel;
        vehicle.Status = input.Status ?? vehicle.Status;
        vehicle.Remarks = input.Remarks ?? vehicle.Remarks;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(vehicle);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/api/fleet/{id}", async (int id, AppDbContext db) =>
{
    try
    {
        var vehicle = await db.FleetVehicles.FindAsync(id);
        if (vehicle == null) return Results.NotFound(new { error = "Vehicle not found" });

        db.FleetVehicles.Remove(vehicle);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Vehicle deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/fleet/stats", async (AppDbContext db, string? site = null) =>
{
    var query = db.FleetVehicles.AsQueryable();

    if (!string.IsNullOrEmpty(site) && site.ToLower() != "all")
        query = query.Where(f => f.Site == site);

    var vehicles = await query.ToListAsync();

    var totalUnit = vehicles.Count;
    var totalHM = vehicles.Sum(v => v.TotalJam);
    var totalKM = vehicles.Sum(v => v.TotalKM);
    var totalFuel = vehicles.Sum(v => v.TotalFuel);
    var avgHMHari = totalUnit > 0 ? vehicles.Average(v => v.AvgHMHari) : 0;
    var avgKMHari = totalUnit > 0 ? vehicles.Average(v => v.AvgKMHari) : 0;
    var fuelRatio = totalHM > 0 ? (totalFuel / totalHM) : 0;

    return Results.Ok(new
    {
        totalUnit,
        totalHM,
        totalKM,
        totalFuel,
        avgHMHari,
        avgKMHari,
        fuelRatio
    });
});

app.MapGet("/api/fleet/summary", async (AppDbContext db) =>
{
    var vehicles = await db.FleetVehicles.ToListAsync();

    var summary = vehicles
        .GroupBy(v => v.Site)
        .Select(g => new
        {
            Site = g.Key,
            TotalUnit = g.Count(),
            TotalHM = g.Sum(v => v.TotalJam),
            TotalKM = g.Sum(v => v.TotalKM),
            TotalFuel = g.Sum(v => v.TotalFuel),
            AvgHMHari = g.Average(v => v.AvgHMHari),
            AvgKMHari = g.Average(v => v.AvgKMHari),
            FuelRatio = g.Sum(v => v.TotalJam) > 0 ? (g.Sum(v => v.TotalFuel) / g.Sum(v => v.TotalJam)) : 0
        })
        .OrderBy(s => s.Site)
        .ToList();

    // Overall summary
    var overall = new
    {
        Site = "OVERALL",
        TotalUnit = vehicles.Count,
        TotalHM = vehicles.Sum(v => v.TotalJam),
        TotalKM = vehicles.Sum(v => v.TotalKM),
        TotalFuel = vehicles.Sum(v => v.TotalFuel),
        AvgHMHari = vehicles.Count > 0 ? vehicles.Average(v => v.AvgHMHari) : 0,
        AvgKMHari = vehicles.Count > 0 ? vehicles.Average(v => v.AvgKMHari) : 0,
        FuelRatio = vehicles.Sum(v => v.TotalJam) > 0 ? (vehicles.Sum(v => v.TotalFuel) / vehicles.Sum(v => v.TotalJam)) : 0
    };

    return Results.Ok(new { summary, overall });
});

app.MapGet("/api/fleet/categories", async (AppDbContext db) =>
{
    var categories = await db.FleetVehicles.Select(v => v.Category).Distinct().Where(c => !string.IsNullOrEmpty(c)).OrderBy(c => c).ToListAsync();
    return Results.Ok(categories);
});

app.MapPost("/api/fleet/upload", async (HttpRequest request, AppDbContext db) =>
{
    try
    {
        var form = await request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
            return Results.BadRequest(new { error = "No file uploaded" });

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var hmAwal = decimal.TryParse(GetValue(values, headers, "HMAwal"), out var hm1) ? hm1 : 0;
                var hmAkhir = decimal.TryParse(GetValue(values, headers, "HMakhir"), out var hm2) ? hm2 : 0;
                var kmAwal = decimal.TryParse(GetValue(values, headers, "KMAwal"), out var km1) ? km1 : 0;
                var kmAkhir = decimal.TryParse(GetValue(values, headers, "KMakhir"), out var km2) ? km2 : 0;
                var totalFuel = decimal.TryParse(GetValue(values, headers, "TotalFuel"), out var tf) ? tf : 0;

                var totalJam = hmAkhir - hmAwal;
                var totalKM = kmAkhir - kmAwal;
                var fuelRatio = totalJam > 0 ? (totalFuel / totalJam) : 0;
                var avgHMHari = totalJam / 30;
                var avgKMHari = totalKM / 30;

                var vehicle = new FleetVehicle
                {
                    UnitNo = GetValue(values, headers, "UnitNo"),
                    UnitDescription = GetValue(values, headers, "UnitDescription"),
                    Site = GetValue(values, headers, "Site"),
                    MerkType = GetValue(values, headers, "MerkType"),
                    Category = GetValue(values, headers, "Category"),
                    HMAwal = hmAwal,
                    KMAwal = kmAwal,
                    HMakhir = hmAkhir,
                    KMakhir = kmAkhir,
                    TotalJam = totalJam,
                    TotalKM = totalKM,
                    FuelRatio = fuelRatio,
                    AvgHMHari = avgHMHari,
                    AvgKMHari = avgKMHari,
                    TotalFuel = totalFuel,
                    Status = "ACTIVE",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.FleetVehicles.Add(vehicle);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Uploaded {insertedCount} vehicles", inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// ==================== R&M - WORK ORDER ====================

// List Work Orders
app.MapGet("/api/workorders", async (AppDbContext db, string? site, string? status, string? unitNo) =>
{
    var query = db.WorkOrders.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(w => w.Site == site);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(w => w.Status == status);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(w => w.UnitNo.Contains(unitNo));

    var orders = await query.OrderByDescending(w => w.WODate).ToListAsync();
    return Results.Ok(orders);
});

// Get Work Order by ID
app.MapGet("/api/workorders/{id}", async (int id, AppDbContext db) =>
{
    var wo = await db.WorkOrders.FindAsync(id);
    return wo == null ? Results.NotFound() : Results.Ok(wo);
});

// Create Work Order
app.MapPost("/api/workorders", async (WorkOrderInput input, AppDbContext db) =>
{
    var wo = new WorkOrder
    {
        WONumber = input.WONumber ?? $"WO-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
        WODate = input.WODate ?? DateTime.UtcNow,
        Site = input.Site,
        UnitNo = input.UnitNo,
        MerkType = input.MerkType,
        Category = input.Category,
        WOType = input.WOType,
        Priority = input.Priority,
        Problem = input.Problem,
        Cause = input.Cause,
        ActionTaken = input.ActionTaken,
        Status = input.Status,
        ScheduledDate = input.ScheduledDate,
        StartDate = input.StartDate,
        EndDate = input.EndDate,
        EstimatedCost = input.EstimatedCost,
        ActualCost = input.ActualCost,
        AssignedTo = input.AssignedTo,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.WorkOrders.Add(wo);
    await db.SaveChangesAsync();
    return Results.Ok(wo);
});

// Update Work Order
app.MapPut("/api/workorders/{id}", async (int id, WorkOrderInput input, AppDbContext db) =>
{
    var wo = await db.WorkOrders.FindAsync(id);
    if (wo == null) return Results.NotFound();

    wo.Site = input.Site;
    wo.UnitNo = input.UnitNo;
    wo.MerkType = input.MerkType;
    wo.Category = input.Category;
    wo.WOType = input.WOType;
    wo.Priority = input.Priority;
    wo.Problem = input.Problem;
    wo.Cause = input.Cause;
    wo.ActionTaken = input.ActionTaken;
    wo.Status = input.Status;
    wo.ScheduledDate = input.ScheduledDate;
    wo.StartDate = input.StartDate;
    wo.EndDate = input.EndDate;
    wo.EstimatedCost = input.EstimatedCost;
    wo.ActualCost = input.ActualCost;
    wo.AssignedTo = input.AssignedTo;
    wo.Remarks = input.Remarks;
    wo.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(wo);
});

// Delete Work Order
app.MapDelete("/api/workorders/{id}", async (int id, AppDbContext db) =>
{
    var wo = await db.WorkOrders.FindAsync(id);
    if (wo == null) return Results.NotFound();

    db.WorkOrders.Remove(wo);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Work Order deleted" });
});

// ==================== R&M - PREVENTIVE MAINTENANCE ====================

// List PM
app.MapGet("/api/pm", async (AppDbContext db, string? site, string? status, string? unitNo) =>
{
    var query = db.PreventiveMaintenances.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(p => p.Site == site);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(p => p.Status == status);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(p => p.UnitNo.Contains(unitNo));

    var pms = await query.OrderByDescending(p => p.PMDate).ToListAsync();
    return Results.Ok(pms);
});

// Get PM by ID
app.MapGet("/api/pm/{id}", async (int id, AppDbContext db) =>
{
    var pm = await db.PreventiveMaintenances.FindAsync(id);
    return pm == null ? Results.NotFound() : Results.Ok(pm);
});

// Create PM
app.MapPost("/api/pm", async (PMInput input, AppDbContext db) =>
{
    var pm = new PreventiveMaintenance
    {
        PMNumber = input.PMNumber ?? $"PM-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
        PMDate = input.PMDate ?? DateTime.UtcNow,
        Site = input.Site,
        UnitNo = input.UnitNo,
        MerkType = input.MerkType,
        PMType = input.PMType,
        Description = input.Description,
        Status = input.Status,
        ScheduledDate = input.ScheduledDate,
        StartDate = input.StartDate,
        EndDate = input.EndDate,
        HMValue = input.HMValue,
        NextHMValue = input.NextHMValue,
        AssignedTo = input.AssignedTo,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.PreventiveMaintenances.Add(pm);
    await db.SaveChangesAsync();
    return Results.Ok(pm);
});

// Update PM
app.MapPut("/api/pm/{id}", async (int id, PMInput input, AppDbContext db) =>
{
    var pm = await db.PreventiveMaintenances.FindAsync(id);
    if (pm == null) return Results.NotFound();

    pm.Site = input.Site;
    pm.UnitNo = input.UnitNo;
    pm.MerkType = input.MerkType;
    pm.PMType = input.PMType;
    pm.Description = input.Description;
    pm.Status = input.Status;
    pm.ScheduledDate = input.ScheduledDate;
    pm.StartDate = input.StartDate;
    pm.EndDate = input.EndDate;
    pm.HMValue = input.HMValue;
    pm.NextHMValue = input.NextHMValue;
    pm.AssignedTo = input.AssignedTo;
    pm.Remarks = input.Remarks;
    pm.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(pm);
});

// Delete PM
app.MapDelete("/api/pm/{id}", async (int id, AppDbContext db) =>
{
    var pm = await db.PreventiveMaintenances.FindAsync(id);
    if (pm == null) return Results.NotFound();

    db.PreventiveMaintenances.Remove(pm);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "PM deleted" });
});

// ==================== R&M - CORRECTIVE MAINTENANCE ====================

// List Corrective
app.MapGet("/api/corrective", async (AppDbContext db, string? site, string? status, string? unitNo) =>
{
    var query = db.CorrectiveMaintenances.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(c => c.Site == site);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(c => c.Status == status);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(c => c.UnitNo.Contains(unitNo));

    var cms = await query.OrderByDescending(c => c.CMDate).ToListAsync();
    return Results.Ok(cms);
});

// Get Corrective by ID
app.MapGet("/api/corrective/{id}", async (int id, AppDbContext db) =>
{
    var cm = await db.CorrectiveMaintenances.FindAsync(id);
    return cm == null ? Results.NotFound() : Results.Ok(cm);
});

// Create Corrective
app.MapPost("/api/corrective", async (CMInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.UnitNo)) return Results.BadRequest(new { error = "UnitNo wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.CMType)) return Results.BadRequest(new { error = "CMType wajib diisi" });

    // === VALIDASI NEGATIF ===
    if ((input.DowntimeHours ?? 0) < 0) return Results.BadRequest(new { error = "DowntimeHours tidak boleh negatif" });
    if ((input.RepairCost ?? 0) < 0) return Results.BadRequest(new { error = "RepairCost tidak boleh negatif" });
    if ((input.PartsCost ?? 0) < 0) return Results.BadRequest(new { error = "PartsCost tidak boleh negatif" });
    if ((input.LaborCost ?? 0) < 0) return Results.BadRequest(new { error = "LaborCost tidak boleh negatif" });

    // === VALIDASI BREAKDOWN: BreakdownEnd >= BreakdownStart ===
    if (input.BreakdownEnd.HasValue && input.BreakdownStart.HasValue && input.BreakdownEnd < input.BreakdownStart)
        return Results.BadRequest(new { error = $"BreakdownEnd ({input.BreakdownEnd}) tidak boleh lebih kecil dari BreakdownStart ({input.BreakdownStart})" });

    var cm = new CorrectiveMaintenance
    {
        CMNumber = input.CMNumber ?? $"CM-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
        CMDate = input.CMDate ?? DateTime.UtcNow,
        Site = input.Site,
        UnitNo = input.UnitNo,
        MerkType = input.MerkType,
        Category = input.Category,
        CMType = input.CMType,
        Priority = input.Priority,
        Problem = input.Problem,
        RootCause = input.RootCause,
        Solution = input.Solution,
        Status = input.Status,
        BreakdownStart = input.BreakdownStart,
        BreakdownEnd = input.BreakdownEnd,
        DowntimeHours = input.DowntimeHours,
        RepairCost = input.RepairCost,
        PartsCost = input.PartsCost,
        LaborCost = input.LaborCost,
        ReportedBy = input.ReportedBy,
        AssignedTo = input.AssignedTo,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.CorrectiveMaintenances.Add(cm);
    await db.SaveChangesAsync();
    return Results.Ok(cm);
});

// Update Corrective
app.MapPut("/api/corrective/{id}", async (int id, CMInput input, AppDbContext db) =>
{
    var cm = await db.CorrectiveMaintenances.FindAsync(id);
    if (cm == null) return Results.NotFound();

    cm.Site = input.Site;
    cm.UnitNo = input.UnitNo;
    cm.MerkType = input.MerkType;
    cm.Category = input.Category;
    cm.CMType = input.CMType;
    cm.Priority = input.Priority;
    cm.Problem = input.Problem;
    cm.RootCause = input.RootCause;
    cm.Solution = input.Solution;
    cm.Status = input.Status;
    cm.BreakdownStart = input.BreakdownStart;
    cm.BreakdownEnd = input.BreakdownEnd;
    cm.DowntimeHours = input.DowntimeHours;
    cm.RepairCost = input.RepairCost;
    cm.PartsCost = input.PartsCost;
    cm.LaborCost = input.LaborCost;
    cm.ReportedBy = input.ReportedBy;
    cm.AssignedTo = input.AssignedTo;
    cm.Remarks = input.Remarks;
    cm.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(cm);
});

// Delete Corrective
app.MapDelete("/api/corrective/{id}", async (int id, AppDbContext db) =>
{
    var cm = await db.CorrectiveMaintenances.FindAsync(id);
    if (cm == null) return Results.NotFound();

    db.CorrectiveMaintenances.Remove(cm);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Corrective Maintenance deleted" });
});

// ==================== R&M - SUMMARY ====================

app.MapGet("/api/rm/summary", async (AppDbContext db) =>
{
    var summary = new RMSummary
    {
        TotalWorkOrders = await db.WorkOrders.CountAsync(),
        OpenWorkOrders = await db.WorkOrders.CountAsync(w => w.Status == "OPEN"),
        InProgressWorkOrders = await db.WorkOrders.CountAsync(w => w.Status == "IN_PROGRESS"),
        CompletedWorkOrders = await db.WorkOrders.CountAsync(w => w.Status == "COMPLETED"),
        TotalPM = await db.PreventiveMaintenances.CountAsync(),
        ScheduledPM = await db.PreventiveMaintenances.CountAsync(p => p.Status == "SCHEDULED"),
        CompletedPM = await db.PreventiveMaintenances.CountAsync(p => p.Status == "COMPLETED"),
        OverduePM = await db.PreventiveMaintenances.CountAsync(p => p.Status == "OVERDUE"),
        TotalCorrective = await db.CorrectiveMaintenances.CountAsync(),
        BreakdownCount = await db.CorrectiveMaintenances.CountAsync(c => c.CMType == "BREAKDOWN"),
        TotalDowntimeHours = await db.CorrectiveMaintenances.SumAsync(c => c.DowntimeHours ?? 0),
        TotalRepairCost = await db.CorrectiveMaintenances.SumAsync(c => (c.RepairCost ?? 0) + (c.PartsCost ?? 0) + (c.LaborCost ?? 0))
    };
    return Results.Ok(summary);
});

// ==================== HR - EMPLOYEE ====================

// List Employees
app.MapGet("/api/employees", async (AppDbContext db, string? site, string? department, string? status, string? search) =>
{
    var query = db.Employees.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(e => e.Site == site);
    if (!string.IsNullOrEmpty(department) && department != "all") query = query.Where(e => e.Department == department);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(e => e.Status == status);
    if (!string.IsNullOrEmpty(search)) query = query.Where(e => e.FullName.Contains(search) || e.EmployeeCode.Contains(search));

    var employees = await query.OrderBy(e => e.FullName).ToListAsync();
    return Results.Ok(employees);
});

// Get Employee by ID
app.MapGet("/api/employees/{id}", async (int id, AppDbContext db) =>
{
    var emp = await db.Employees.FindAsync(id);
    return emp == null ? Results.NotFound() : Results.Ok(emp);
});

// Create Employee
app.MapPost("/api/employees", async (EmployeeInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.FullName)) return Results.BadRequest(new { error = "FullName wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });

    // === VALIDASI DUPLIKAT EmployeeCode ===
    if (!string.IsNullOrWhiteSpace(input.EmployeeCode))
    {
        var existing = await db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == input.EmployeeCode);
        if (existing != null) return Results.BadRequest(new { error = $"EmployeeCode '{input.EmployeeCode}' sudah ada" });
    }

    // === VALIDASI KEDATANGAN: ResignDate >= JoinDate ===
    if (input.ResignDate.HasValue && input.JoinDate.HasValue && input.ResignDate < input.JoinDate)
        return Results.BadRequest(new { error = $"ResignDate ({input.ResignDate:d}) tidak boleh lebih kecil dari JoinDate ({input.JoinDate:d})" });

    var emp = new Employee
    {
        EmployeeCode = input.EmployeeCode ?? $"EMP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
        FullName = input.FullName,
        NickName = input.NickName,
        PIN = input.PIN,
        RFID = input.RFID,
        Gender = input.Gender,
        BirthDate = input.BirthDate,
        BirthPlace = input.BirthPlace,
        Religion = input.Religion,
        MaritalStatus = input.MaritalStatus,
        Address = input.Address,
        Phone = input.Phone,
        Email = input.Email,
        EmergencyContact = input.EmergencyContact,
        EmergencyPhone = input.EmergencyPhone,
        Site = input.Site,
        Department = input.Department,
        Position = input.Position,
        Level = input.Level,
        EmployeeType = input.EmployeeType,
        Status = input.Status,
        JoinDate = input.JoinDate,
        ResignDate = input.ResignDate,
        PhotoUrl = input.PhotoUrl,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Employees.Add(emp);
    await db.SaveChangesAsync();
    return Results.Ok(emp);
});

// Update Employee
app.MapPut("/api/employees/{id}", async (int id, EmployeeInput input, AppDbContext db) =>
{
    var emp = await db.Employees.FindAsync(id);
    if (emp == null) return Results.NotFound();

    emp.FullName = input.FullName;
    emp.NickName = input.NickName;
    emp.PIN = input.PIN;
    emp.RFID = input.RFID;
    emp.Gender = input.Gender;
    emp.BirthDate = input.BirthDate;
    emp.BirthPlace = input.BirthPlace;
    emp.Religion = input.Religion;
    emp.MaritalStatus = input.MaritalStatus;
    emp.Address = input.Address;
    emp.Phone = input.Phone;
    emp.Email = input.Email;
    emp.EmergencyContact = input.EmergencyContact;
    emp.EmergencyPhone = input.EmergencyPhone;
    emp.Site = input.Site;
    emp.Department = input.Department;
    emp.Position = input.Position;
    emp.Level = input.Level;
    emp.EmployeeType = input.EmployeeType;
    emp.Status = input.Status;
    emp.JoinDate = input.JoinDate;
    emp.ResignDate = input.ResignDate;
    emp.PhotoUrl = input.PhotoUrl;
    emp.Remarks = input.Remarks;
    emp.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(emp);
});

// Delete Employee
app.MapDelete("/api/employees/{id}", async (int id, AppDbContext db) =>
{
    var emp = await db.Employees.FindAsync(id);
    if (emp == null) return Results.NotFound();

    db.Employees.Remove(emp);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Employee deleted" });
});

// Get Employee Departments
app.MapGet("/api/employees/departments", async (AppDbContext db) =>
{
    var departments = await db.Employees.Select(e => e.Department).Distinct().ToListAsync();
    return Results.Ok(departments);
});

// ==================== HR - ATTENDANCE ====================

// List Attendance
app.MapGet("/api/attendance", async (AppDbContext db, string? site, string? department, string? date, string? status) =>
{
    var query = db.Attendances.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(a => a.Site == site);
    if (!string.IsNullOrEmpty(department) && department != "all") query = query.Where(a => a.Department == department);
    if (!string.IsNullOrEmpty(date)) query = query.Where(a => a.AttendanceDate.Date == DateTime.Parse(date).Date);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(a => a.Status == status);

    var attendance = await query.OrderByDescending(a => a.AttendanceDate).ThenBy(a => a.EmployeeName).ToListAsync();
    return Results.Ok(attendance);
});

// Get Attendance by ID
app.MapGet("/api/attendance/{id}", async (int id, AppDbContext db) =>
{
    var att = await db.Attendances.FindAsync(id);
    return att == null ? Results.NotFound() : Results.Ok(att);
});

// Create Attendance
app.MapPost("/api/attendance", async (AttendanceInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.EmployeeCode)) return Results.BadRequest(new { error = "EmployeeCode wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });

    // === VALIDASI WAKTU: CheckOut >= CheckIn ===
    if (input.CheckOut.HasValue && input.CheckIn.HasValue && input.CheckOut <= input.CheckIn)
        return Results.BadRequest(new { error = "CheckOut harus lebih besar dari CheckIn" });

    // === VALIDASI NEGATIF ===
    if ((input.WorkingHours ?? 0) < 0) return Results.BadRequest(new { error = "WorkingHours tidak boleh negatif" });
    if ((input.OvertimeHours ?? 0) < 0) return Results.BadRequest(new { error = "OvertimeHours tidak boleh negatif" });

    // === VALIDASI DUPLIKAT (EmployeeCode + AttendanceDate) ===
    var attDate = input.AttendanceDate ?? DateTime.UtcNow;
    var existingAtt = await db.Attendances.FirstOrDefaultAsync(a =>
        a.EmployeeCode == input.EmployeeCode &&
        a.AttendanceDate.Date == attDate.Date);
    if (existingAtt != null) return Results.BadRequest(new { error = $"Attendance untuk EmployeeCode '{input.EmployeeCode}' di tanggal {attDate:d} sudah ada" });

    // Calculate working hours if check in/out provided
    decimal? workingHours = null;
    if (input.CheckIn.HasValue && input.CheckOut.HasValue)
    {
        workingHours = (decimal)(input.CheckOut.Value - input.CheckIn.Value).TotalHours;
    }

    var att = new Attendance
    {
        EmployeeCode = input.EmployeeCode ?? "",
        EmployeeName = input.EmployeeName ?? "",
        Site = input.Site,
        Department = input.Department,
        AttendanceDate = input.AttendanceDate ?? DateTime.UtcNow,
        CheckIn = input.CheckIn,
        CheckOut = input.CheckOut,
        Shift = input.Shift,
        Status = input.Status,
        WorkingHours = workingHours ?? input.WorkingHours,
        OvertimeHours = input.OvertimeHours,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Attendances.Add(att);
    await db.SaveChangesAsync();
    return Results.Ok(att);
});

// Update Attendance
app.MapPut("/api/attendance/{id}", async (int id, AttendanceInput input, AppDbContext db) =>
{
    var att = await db.Attendances.FindAsync(id);
    if (att == null) return Results.NotFound();

    att.EmployeeCode = input.EmployeeCode ?? "";
    att.EmployeeName = input.EmployeeName ?? "";
    att.Site = input.Site;
    att.Department = input.Department;
    att.AttendanceDate = input.AttendanceDate ?? DateTime.UtcNow;
    att.CheckIn = input.CheckIn;
    att.CheckOut = input.CheckOut;
    att.Shift = input.Shift;
    att.Status = input.Status;

    if (input.CheckIn.HasValue && input.CheckOut.HasValue)
        att.WorkingHours = (decimal)(input.CheckOut.Value - input.CheckIn.Value).TotalHours;
    else
        att.WorkingHours = input.WorkingHours;

    att.OvertimeHours = input.OvertimeHours;
    att.Remarks = input.Remarks;
    att.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(att);
});

// Delete Attendance
app.MapDelete("/api/attendance/{id}", async (int id, AppDbContext db) =>
{
    var att = await db.Attendances.FindAsync(id);
    if (att == null) return Results.NotFound();

    db.Attendances.Remove(att);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Attendance deleted" });
});

// Bulk Attendance Upload
app.MapPost("/api/attendance/upload", async (HttpContext context, AppDbContext db) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var file = form.Files.GetFile("file");
        if (file == null) return Results.BadRequest(new { error = "No file uploaded" });

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2) return Results.BadRequest(new { error = "File is empty or invalid" });

        var headers = lines[0].Split(';');
        int insertedCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            if (values.Length < headers.Length) continue;

            try
            {
                var att = new Attendance
                {
                    EmployeeCode = GetValue(values, headers, "EmployeeCode"),
                    EmployeeName = GetValue(values, headers, "EmployeeName"),
                    Site = GetValue(values, headers, "Site"),
                    Department = GetValue(values, headers, "Department"),
                    AttendanceDate = DateTime.TryParse(GetValue(values, headers, "AttendanceDate"), out var dt) ? dt : DateTime.UtcNow,
                    CheckIn = DateTime.TryParse(GetValue(values, headers, "CheckIn"), out var ci) ? ci : null,
                    CheckOut = DateTime.TryParse(GetValue(values, headers, "CheckOut"), out var co) ? co : null,
                    Shift = GetValue(values, headers, "Shift"),
                    Status = GetValue(values, headers, "Status") == "" ? "PRESENT" : GetValue(values, headers, "Status"),
                    WorkingHours = decimal.TryParse(GetValue(values, headers, "WorkingHours"), out var wh) ? wh : 0,
                    OvertimeHours = decimal.TryParse(GetValue(values, headers, "OvertimeHours"), out var ot) ? ot : 0,
                    Remarks = GetValue(values, headers, "Remarks"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Attendances.Add(att);
                insertedCount++;
            }
            catch { }
        }

        await db.SaveChangesAsync();
        return Results.Ok(new { message = $"Uploaded {insertedCount} attendance records", inserted = insertedCount });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Error: {ex.Message}" });
    }
});

// ==================== HR - PAYROLL ====================

// List Payroll
app.MapGet("/api/payroll", async (AppDbContext db, string? site, string? department, string? period, string? status) =>
{
    var query = db.Payrolls.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(p => p.Site == site);
    if (!string.IsNullOrEmpty(department) && department != "all") query = query.Where(p => p.Department == department);
    if (!string.IsNullOrEmpty(period) && period != "all") query = query.Where(p => p.PeriodYear + "-" + p.PeriodMonth == period);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(p => p.Status == status);

    var payroll = await query.OrderByDescending(p => p.PeriodYear).ThenByDescending(p => p.PeriodMonth).ThenBy(p => p.EmployeeName).ToListAsync();
    return Results.Ok(payroll);
});

// Get Payroll by ID
app.MapGet("/api/payroll/{id}", async (int id, AppDbContext db) =>
{
    var p = await db.Payrolls.FindAsync(id);
    return p == null ? Results.NotFound() : Results.Ok(p);
});

// Create Payroll
app.MapPost("/api/payroll", async (PayrollInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.EmployeeCode)) return Results.BadRequest(new { error = "EmployeeCode wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.PeriodMonth)) return Results.BadRequest(new { error = "PeriodMonth wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.PeriodYear)) return Results.BadRequest(new { error = "PeriodYear wajib diisi" });

    // === VALIDASI PERIOD FORMAT ===
    if (!int.TryParse(input.PeriodMonth, out var monthNum) || monthNum < 1 || monthNum > 12)
        return Results.BadRequest(new { error = "PeriodMonth harus angka 01-12" });

    // === VALIDASI NEGATIF ===
    if (input.BasicSalary < 0) return Results.BadRequest(new { error = "BasicSalary tidak boleh negatif" });
    if (input.Allowance < 0) return Results.BadRequest(new { error = "Allowance tidak boleh negatif" });
    if (input.Overtime < 0) return Results.BadRequest(new { error = "Overtime tidak boleh negatif" });
    if (input.Bonus < 0) return Results.BadRequest(new { error = "Bonus tidak boleh negatif" });
    if (input.AbsenceDeduction < 0) return Results.BadRequest(new { error = "AbsenceDeduction tidak boleh negatif" });
    if (input.TaxDeduction < 0) return Results.BadRequest(new { error = "TaxDeduction tidak boleh negatif" });
    if (input.InsuranceDeduction < 0) return Results.BadRequest(new { error = "InsuranceDeduction tidak boleh negatif" });
    if (input.OtherDeduction < 0) return Results.BadRequest(new { error = "OtherDeduction tidak boleh negatif" });

    var totalEarning = input.BasicSalary + input.Allowance + input.Overtime + input.Bonus;
    var totalDeduction = input.AbsenceDeduction + input.TaxDeduction + input.InsuranceDeduction + input.OtherDeduction;
    var netSalary = totalEarning - totalDeduction;

    var p = new Payroll
    {
        PayrollNumber = input.PayrollNumber ?? $"PAY-{DateTime.Now:yyyyMM}-{new Random().Next(1000, 9999)}",
        PayrollDate = input.PayrollDate ?? DateTime.UtcNow,
        PeriodMonth = input.PeriodMonth,
        PeriodYear = input.PeriodYear,
        EmployeeCode = input.EmployeeCode,
        EmployeeName = input.EmployeeName ?? "",
        Site = input.Site,
        Department = input.Department,
        Position = input.Position,
        BasicSalary = input.BasicSalary,
        Allowance = input.Allowance,
        Overtime = input.Overtime,
        Bonus = input.Bonus,
        TotalEarning = totalEarning,
        AbsenceDeduction = input.AbsenceDeduction,
        TaxDeduction = input.TaxDeduction,
        InsuranceDeduction = input.InsuranceDeduction,
        OtherDeduction = input.OtherDeduction,
        TotalDeduction = totalDeduction,
        NetSalary = netSalary,
        Status = input.Status,
        Remarks = input.Remarks,
        PaidDate = input.PaidDate,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Payrolls.Add(p);
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

// Update Payroll
app.MapPut("/api/payroll/{id}", async (int id, PayrollInput input, AppDbContext db) =>
{
    var p = await db.Payrolls.FindAsync(id);
    if (p == null) return Results.NotFound();

    var totalEarning = input.BasicSalary + input.Allowance + input.Overtime + input.Bonus;
    var totalDeduction = input.AbsenceDeduction + input.TaxDeduction + input.InsuranceDeduction + input.OtherDeduction;
    var netSalary = totalEarning - totalDeduction;

    p.EmployeeCode = input.EmployeeCode;
    p.EmployeeName = input.EmployeeName ?? "";
    p.Site = input.Site;
    p.Department = input.Department;
    p.Position = input.Position;
    p.BasicSalary = input.BasicSalary;
    p.Allowance = input.Allowance;
    p.Overtime = input.Overtime;
    p.Bonus = input.Bonus;
    p.TotalEarning = totalEarning;
    p.AbsenceDeduction = input.AbsenceDeduction;
    p.TaxDeduction = input.TaxDeduction;
    p.InsuranceDeduction = input.InsuranceDeduction;
    p.OtherDeduction = input.OtherDeduction;
    p.TotalDeduction = totalDeduction;
    p.NetSalary = netSalary;
    p.Status = input.Status;
    p.Remarks = input.Remarks;
    p.PaidDate = input.PaidDate;
    p.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(p);
});

// Delete Payroll
app.MapDelete("/api/payroll/{id}", async (int id, AppDbContext db) =>
{
    var p = await db.Payrolls.FindAsync(id);
    if (p == null) return Results.NotFound();

    db.Payrolls.Remove(p);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Payroll deleted" });
});

// Get Available Periods
app.MapGet("/api/payroll/periods", async (AppDbContext db) =>
{
    var periods = await db.Payrolls
        .Select(p => p.PeriodYear + "-" + p.PeriodMonth)
        .Distinct()
        .OrderByDescending(p => p)
        .ToListAsync();
    return Results.Ok(periods);
});

// ==================== HR - SUMMARY ====================

app.MapGet("/api/hr/summary", async (AppDbContext db) =>
{
    var today = DateTime.UtcNow.Date;

    var summary = new HRSummary
    {
        TotalEmployees = await db.Employees.CountAsync(),
        ActiveEmployees = await db.Employees.CountAsync(e => e.Status == "ACTIVE"),
        InactiveEmployees = await db.Employees.CountAsync(e => e.Status != "ACTIVE"),
        TotalAttendance = await db.Attendances.CountAsync(),
        PresentToday = await db.Attendances.CountAsync(a => a.AttendanceDate.Date == today && a.Status == "PRESENT"),
        AbsentToday = await db.Attendances.CountAsync(a => a.AttendanceDate.Date == today && a.Status == "ABSENT"),
        LateToday = await db.Attendances.CountAsync(a => a.AttendanceDate.Date == today && a.Status == "LATE"),
        TotalPayroll = await db.Payrolls.Where(p => p.Status == "PAID").SumAsync(p => p.NetSalary),
        PendingPayroll = await db.Payrolls.CountAsync(p => p.Status != "PAID")
    };
    return Results.Ok(summary);
});

// ==================== AUTO-CREATE DATABASE TABLES ====================

// Auto-create database tables if not exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await db.Database.EnsureCreatedAsync();

        // Fix: Change timestamp with time zone to timestamp without time zone
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                ALTER TABLE fuel_receipts ALTER COLUMN ""Tanggal"" TYPE timestamp;
                ALTER TABLE fuel_usages ALTER COLUMN ""Tanggal"" TYPE timestamp;
                ALTER TABLE fuel_usages DROP COLUMN IF EXISTS ""LiterAwal"";
                ALTER TABLE fuel_usages DROP COLUMN IF EXISTS ""LiterAkhir"";
                ALTER TABLE fuel_usages ADD COLUMN IF NOT EXISTS ""HM"" numeric(18,2);
                ALTER TABLE fuel_usages ADD COLUMN IF NOT EXISTS ""KM"" numeric(18,2);
                ALTER TABLE fuel_usages ALTER COLUMN ""HM"" SET DEFAULT 0;
                ALTER TABLE fuel_usages ALTER COLUMN ""KM"" SET DEFAULT 0;
            ");
            Console.WriteLine("DateTime columns fixed!");
            Console.WriteLine("Old columns dropped, HM and KM columns added!");
        }
        catch
        {
            // Columns may already be correct
        }

        // Create Tyre tables if not exist
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS ""tyres_po"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""No"" INTEGER NOT NULL,
                    ""Tanggal"" timestamp NOT NULL,
                    ""Site"" VARCHAR(100) NOT NULL,
                    ""Vendor"" VARCHAR(200),
                    ""NoPO"" VARCHAR(50),
                    ""MerkType"" VARCHAR(100),
                    ""Size"" VARCHAR(50),
                    ""Qty"" numeric(18,2),
                    ""UnitPrice"" numeric(18,2),
                    ""TotalPrice"" numeric(18,2),
                    ""Status"" VARCHAR(50),
                    ""CreatedAt"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    ""UpdatedAt"" timestamp DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS ""tyres_problems"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""No"" INTEGER NOT NULL,
                    ""Tanggal"" timestamp NOT NULL,
                    ""Site"" VARCHAR(100) NOT NULL,
                    ""UnitNo"" VARCHAR(50),
                    ""NoSeriTyre"" VARCHAR(100),
                    ""ProblemDescription"" VARCHAR(500),
                    ""ActionTaken"" VARCHAR(500),
                    ""Cost"" numeric(18,2),
                    ""CreatedAt"" timestamp DEFAULT CURRENT_TIMESTAMP,
                    ""UpdatedAt"" timestamp DEFAULT CURRENT_TIMESTAMP
                );
            ");
            Console.WriteLine("Tyre tables created!");
        }
        catch
        {
            // Tables may already exist
        }

        Console.WriteLine("Database tables created successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating database tables: {ex.Message}");
    }
}

// ==================== FINANCE - CHART OF ACCOUNTS ====================

app.MapGet("/api/coa", async (AppDbContext db, string? type, bool? active) =>
{
    var query = db.ChartOfAccounts.AsQueryable();
    if (!string.IsNullOrEmpty(type)) query = query.Where(c => c.AccountType == type);
    if (active.HasValue) query = query.Where(c => c.IsActive == active.Value);

    var accounts = await query.OrderBy(c => c.AccountCode).ToListAsync();
    return Results.Ok(accounts);
});

app.MapGet("/api/coa/{code}", async (string code, AppDbContext db) =>
{
    var account = await db.ChartOfAccounts.FirstOrDefaultAsync(c => c.AccountCode == code);
    return account == null ? Results.NotFound() : Results.Ok(account);
});

app.MapPost("/api/coa", async (ChartOfAccount input, AppDbContext db) =>
{
    var existing = await db.ChartOfAccounts.FirstOrDefaultAsync(c => c.AccountCode == input.AccountCode);
    if (existing != null) return Results.BadRequest(new { error = "Account code already exists" });

    input.CurrentBalance = input.OpeningBalance;
    db.ChartOfAccounts.Add(input);
    await db.SaveChangesAsync();
    return Results.Ok(input);
});

app.MapPut("/api/coa/{id}", async (int id, ChartOfAccount input, AppDbContext db) =>
{
    var account = await db.ChartOfAccounts.FindAsync(id);
    if (account == null) return Results.NotFound();

    account.AccountName = input.AccountName;
    account.AccountType = input.AccountType;
    account.ParentAccountCode = input.ParentAccountCode;
    account.NormalBalance = input.NormalBalance;
    account.CostCenterRequired = input.CostCenterRequired;
    account.TaxCode = input.TaxCode;
    account.IsActive = input.IsActive;
    account.Remarks = input.Remarks;
    account.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(account);
});

app.MapDelete("/api/coa/{id}", async (int id, AppDbContext db) =>
{
    var account = await db.ChartOfAccounts.FindAsync(id);
    if (account == null) return Results.NotFound();

    db.ChartOfAccounts.Remove(account);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Account deleted" });
});

// Initialize Default Chart of Accounts
app.MapPost("/api/coa/init", async (AppDbContext db) =>
{
    var existing = await db.ChartOfAccounts.AnyAsync();
    if (existing) return Results.Ok(new { message = "COA already initialized" });

    var defaultAccounts = new List<ChartOfAccount>
    {
        // ASSET
        new() { AccountCode = "1000", AccountName = "Kas & Bank", AccountType = "ASSET", NormalBalance = "DEBIT" },
        new() { AccountCode = "1100", AccountName = "Piutang Usaha", AccountType = "ASSET", NormalBalance = "DEBIT" },
        new() { AccountCode = "1200", AccountName = "Persediaan", AccountType = "ASSET", NormalBalance = "DEBIT" },
        new() { AccountCode = "1300", AccountName = "Aset Tetap", AccountType = "ASSET", NormalBalance = "DEBIT" },
        new() { AccountCode = "1400", AccountName = "Akumulasi Penyusutan", AccountType = "ASSET", NormalBalance = "CREDIT" },
        // LIABILITY
        new() { AccountCode = "2000", AccountName = "Hutang Usaha", AccountType = "LIABILITY", NormalBalance = "CREDIT" },
        new() { AccountCode = "2100", AccountName = "Hutang Pajak", AccountType = "LIABILITY", NormalBalance = "CREDIT" },
        new() { AccountCode = "2200", AccountName = "Hutang Gaji", AccountType = "LIABILITY", NormalBalance = "CREDIT" },
        // EQUITY
        new() { AccountCode = "3000", AccountName = "Modal Saham", AccountType = "EQUITY", NormalBalance = "CREDIT" },
        new() { AccountCode = "3100", AccountName = "Laba Ditahan", AccountType = "EQUITY", NormalBalance = "CREDIT" },
        // REVENUE
        new() { AccountCode = "4000", AccountName = "Pendapatan Angkutan", AccountType = "REVENUE", NormalBalance = "CREDIT" },
        new() { AccountCode = "4100", AccountName = "Pendapatan Lainnya", AccountType = "REVENUE", NormalBalance = "CREDIT" },
        // EXPENSE
        new() { AccountCode = "5000", AccountName = "Bahan Bakar & Lubricant", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
        new() { AccountCode = "5100", AccountName = "Biaya Pemeliharaan", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
        new() { AccountCode = "5200", AccountName = "Biaya Gaji & Upah", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
        new() { AccountCode = "5300", AccountName = "Biaya Depresiasi", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
        new() { AccountCode = "5400", AccountName = "Biaya Overhead", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
        new() { AccountCode = "5500", AccountName = "Harga Pokok Penjualan", AccountType = "EXPENSE", NormalBalance = "DEBIT" },
    };

    db.ChartOfAccounts.AddRange(defaultAccounts);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = $"Created {defaultAccounts.Count} default accounts" });
});

// ==================== FINANCE - BUDGET ====================

app.MapGet("/api/budget", async (AppDbContext db, string? site, string? period, string? division, string? status) =>
{
    var query = db.Budgets.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(b => b.Site == site);
    if (!string.IsNullOrEmpty(period) && period != "all") query = query.Where(b => b.PeriodYear + "-" + b.PeriodMonth == period);
    if (!string.IsNullOrEmpty(division) && division != "all") query = query.Where(b => b.Division == division);
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(b => b.Status == status);

    var budgets = await query.OrderByDescending(b => b.PeriodYear).ThenByDescending(b => b.PeriodMonth).ThenBy(b => b.AccountCode).ToListAsync();
    return Results.Ok(budgets);
});

app.MapGet("/api/budget/{id}", async (int id, AppDbContext db) =>
{
    var budget = await db.Budgets.FindAsync(id);
    return budget == null ? Results.NotFound() : Results.Ok(budget);
});

app.MapPost("/api/budget", async (BudgetInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.AccountCode)) return Results.BadRequest(new { error = "AccountCode wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.PeriodMonth)) return Results.BadRequest(new { error = "PeriodMonth wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.PeriodYear)) return Results.BadRequest(new { error = "PeriodYear wajib diisi" });

    // === VALIDASI NEGATIF ===
    if (input.PlannedAmount < 0) return Results.BadRequest(new { error = "PlannedAmount tidak boleh negatif" });

    // === VALIDASI DUPLIKAT (AccountCode + Period + Site) ===
    var existingBudget = await db.Budgets.FirstOrDefaultAsync(b =>
        b.AccountCode == input.AccountCode &&
        b.PeriodYear == input.PeriodYear &&
        b.PeriodMonth == input.PeriodMonth &&
        b.Site == input.Site);
    if (existingBudget != null) return Results.BadRequest(new { error = $"Budget untuk AccountCode '{input.AccountCode}' di period {input.PeriodYear}-{input.PeriodMonth} site '{input.Site}' sudah ada" });

    var budget = new Budget
    {
        BudgetNumber = input.BudgetNumber ?? $"BUD-{input.PeriodYear}{input.PeriodMonth}-{new Random().Next(1000, 9999)}",
        PeriodMonth = input.PeriodMonth,
        PeriodYear = input.PeriodYear,
        Site = input.Site,
        Department = input.Department,
        Division = input.Division,
        AccountCode = input.AccountCode,
        AccountName = input.AccountName ?? "",
        PlannedAmount = input.PlannedAmount,
        ActualAmount = 0,
        CommittedAmount = 0,
        AvailableBudget = input.PlannedAmount,
        UtilizationPercent = 0,
        Status = "DRAFT",
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Budgets.Add(budget);
    await db.SaveChangesAsync();
    return Results.Ok(budget);
});

app.MapPut("/api/budget/{id}", async (int id, BudgetInput input, AppDbContext db) =>
{
    var budget = await db.Budgets.FindAsync(id);
    if (budget == null) return Results.NotFound();

    budget.PlannedAmount = input.PlannedAmount;
    budget.Site = input.Site;
    budget.Department = input.Department;
    budget.Division = input.Division;
    budget.AccountCode = input.AccountCode;
    budget.AccountName = input.AccountName ?? "";
    budget.Remarks = input.Remarks;
    budget.AvailableBudget = budget.PlannedAmount - budget.ActualAmount - budget.CommittedAmount;
    budget.UtilizationPercent = budget.PlannedAmount > 0 ? (budget.ActualAmount / budget.PlannedAmount) * 100 : 0;
    budget.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(budget);
});

app.MapPut("/api/budget/{id}/approve", async (int id, AppDbContext db) =>
{
    var budget = await db.Budgets.FindAsync(id);
    if (budget == null) return Results.NotFound();

    budget.Status = "APPROVED";
    budget.ApprovedDate = DateTime.UtcNow;
    budget.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(budget);
});

app.MapDelete("/api/budget/{id}", async (int id, AppDbContext db) =>
{
    var budget = await db.Budgets.FindAsync(id);
    if (budget == null) return Results.NotFound();

    db.Budgets.Remove(budget);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Budget deleted" });
});

// Update Budget Actual from Journal Entries
app.MapPost("/api/budget/calculate-actual/{periodYear}/{periodMonth}", async (string periodYear, string periodMonth, AppDbContext db) =>
{
    var budgets = await db.Budgets.Where(b => b.PeriodYear == periodYear && b.PeriodMonth == periodMonth && b.Status == "APPROVED").ToListAsync();

    foreach (var budget in budgets)
    {
        // Get actual from journal entries
        var journalLines = await db.JournalLines
            .Include(j => j.JournalEntry)
            .Where(j => j.AccountCode == budget.AccountCode
                && j.JournalEntry!.PeriodYear == periodYear
                && j.JournalEntry.PeriodMonth == periodMonth
                && j.JournalEntry.Status == "POSTED")
            .ToListAsync();

        var totalActual = journalLines.Where(j => j.DC == "D").Sum(j => j.Amount);
        budget.ActualAmount = totalActual;
        budget.AvailableBudget = budget.PlannedAmount - budget.ActualAmount - budget.CommittedAmount;
        budget.UtilizationPercent = budget.PlannedAmount > 0 ? Math.Round((budget.ActualAmount / budget.PlannedAmount) * 100, 2) : 0;
        budget.UpdatedAt = DateTime.UtcNow;
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { message = $"Updated {budgets.Count} budgets" });
});

// Budget Summary with Alerts
app.MapGet("/api/budget/summary", async (AppDbContext db, string? period) =>
{
    var query = db.Budgets.AsQueryable();
    if (!string.IsNullOrEmpty(period) && period != "all")
        query = query.Where(b => b.PeriodYear + "-" + b.PeriodMonth == period);

    var budgets = await query.ToListAsync();

    var summary = new BudgetSummary
    {
        TotalBudgets = budgets.Count,
        ActiveBudgets = budgets.Count(b => b.Status == "APPROVED"),
        TotalPlanned = budgets.Sum(b => b.PlannedAmount),
        TotalActual = budgets.Sum(b => b.ActualAmount),
        TotalCommitted = budgets.Sum(b => b.CommittedAmount),
        OverallUtilization = budgets.Sum(b => b.PlannedAmount) > 0
            ? Math.Round((budgets.Sum(b => b.ActualAmount) / budgets.Sum(b => b.PlannedAmount)) * 100, 2) : 0
    };

    // Generate alerts for budgets exceeding thresholds
    foreach (var b in budgets.Where(b => b.UtilizationPercent >= 75))
    {
        var alertLevel = b.UtilizationPercent >= 100 ? "EXCEEDED" : b.UtilizationPercent >= 90 ? "CRITICAL" : "WARNING";
        summary.Alerts.Add(new BudgetAlert
        {
            BudgetNumber = b.BudgetNumber,
            AccountCode = b.AccountCode,
            AccountName = b.AccountName,
            PlannedAmount = b.PlannedAmount,
            ActualAmount = b.ActualAmount,
            UtilizationPercent = b.UtilizationPercent,
            AlertLevel = alertLevel
        });
    }

    return Results.Ok(summary);
});

// ==================== FINANCE - JOURNAL ENTRIES (GL) ====================

app.MapGet("/api/gl", async (AppDbContext db, string? site, string? period, string? status, string? source) =>
{
    var query = db.JournalEntries.AsQueryable();
    if (!string.IsNullOrEmpty(status) && status != "all") query = query.Where(j => j.Status == status);
    if (!string.IsNullOrEmpty(source) && source != "all") query = query.Where(j => j.SourceModule == source);
    if (!string.IsNullOrEmpty(period) && period != "all")
    {
        var parts = period.Split('-');
        if (parts.Length == 2)
        {
            var py = parts[0]; var pm = parts[1];
            query = query.Where(j => j.PeriodYear == py && j.PeriodMonth == pm);
        }
    }

    var entries = await query
        .OrderByDescending(j => j.EntryDate)
        .ThenByDescending(j => j.EntryNumber)
        .ToListAsync();

    return Results.Ok(entries);
});

app.MapGet("/api/gl/{id}", async (int id, AppDbContext db) =>
{
    var entry = await db.JournalEntries.FirstOrDefaultAsync(j => j.Id == id);
    if (entry == null) return Results.NotFound();

    var lines = await db.JournalLines.Where(l => l.JournalEntryId == id).ToListAsync();
    return Results.Ok(new { entry, lines });
});

app.MapGet("/api/gl/{id}/lines", async (int id, AppDbContext db) =>
{
    var lines = await db.JournalLines.Where(l => l.JournalEntryId == id).ToListAsync();
    return Results.Ok(lines);
});

app.MapPost("/api/gl", async (JournalEntryInput input, AppDbContext db) =>
{
    if (input.Lines == null || input.Lines.Count < 2)
        return Results.BadRequest(new { error = "Journal must have at least 2 lines" });

    // Validate debits = credits
    var totalDebit = input.Lines.Where(l => l.DC == "D").Sum(l => l.Amount);
    var totalCredit = input.Lines.Where(l => l.DC == "C").Sum(l => l.Amount);

    if (Math.Abs(totalDebit - totalCredit) > 0.01m)
        return Results.BadRequest(new { error = $"Debit ({totalDebit}) must equal Credit ({totalCredit})" });

    var entryDate = input.EntryDate ?? DateTime.UtcNow;
    var entry = new JournalEntry
    {
        EntryNumber = input.EntryNumber ?? $"JE-{entryDate:yyyyMMdd}-{new Random().Next(1000, 9999)}",
        EntryDate = entryDate,
        PeriodMonth = entryDate.ToString("MM"),
        PeriodYear = entryDate.ToString("yyyy"),
        EntryType = input.EntryType,
        SourceModule = input.SourceModule ?? "MANUAL",
        SourceId = input.SourceId,
        Description = input.Description,
        Status = "DRAFT",
        TotalDebit = totalDebit,
        TotalCredit = totalCredit,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Add lines
    foreach (var lineInput in input.Lines)
    {
        var line = new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = lineInput.AccountCode,
            AccountName = lineInput.AccountName ?? "",
            DC = lineInput.DC,
            Amount = lineInput.Amount,
            CostCenter = lineInput.CostCenter,
            Site = lineInput.Site,
            Description = lineInput.Description,
            CreatedAt = DateTime.UtcNow
        };
        db.JournalLines.Add(line);
    }

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

app.MapPut("/api/gl/{id}/post", async (int id, AppDbContext db) =>
{
    var entry = await db.JournalEntries.FindAsync(id);
    if (entry == null) return Results.NotFound();

    if (entry.Status == "POSTED")
        return Results.BadRequest(new { error = "Entry already posted" });

    // Re-validate
    var lines = await db.JournalLines.Where(l => l.JournalEntryId == id).ToListAsync();
    var totalDebit = lines.Where(l => l.DC == "D").Sum(l => l.Amount);
    var totalCredit = lines.Where(l => l.DC == "C").Sum(l => l.Amount);

    if (Math.Abs(totalDebit - totalCredit) > 0.01m)
        return Results.BadRequest(new { error = "Cannot post: Debit != Credit" });

    entry.Status = "POSTED";
    entry.TotalDebit = totalDebit;
    entry.TotalCredit = totalCredit;
    entry.PostedAt = DateTime.UtcNow;
    entry.UpdatedAt = DateTime.UtcNow;

    // Update account balances
    foreach (var line in lines)
    {
        var account = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == line.AccountCode);
        if (account != null)
        {
            if (line.DC == "D")
                account.CurrentBalance += line.Amount;
            else
                account.CurrentBalance -= line.Amount;
            account.UpdatedAt = DateTime.UtcNow;
        }
    }

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

app.MapPut("/api/gl/{id}/cancel", async (int id, AppDbContext db) =>
{
    var entry = await db.JournalEntries.FindAsync(id);
    if (entry == null) return Results.NotFound();

    if (entry.Status == "POSTED")
    {
        // Reverse account balances
        var lines = await db.JournalLines.Where(l => l.JournalEntryId == id).ToListAsync();
        foreach (var line in lines)
        {
            var account = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == line.AccountCode);
            if (account != null)
            {
                if (line.DC == "D")
                    account.CurrentBalance -= line.Amount;
                else
                    account.CurrentBalance += line.Amount;
            }
        }
    }

    entry.Status = "CANCELLED";
    entry.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

app.MapDelete("/api/gl/{id}", async (int id, AppDbContext db) =>
{
    var entry = await db.JournalEntries.FindAsync(id);
    if (entry == null) return Results.NotFound();

    if (entry.Status == "POSTED")
        return Results.BadRequest(new { error = "Cannot delete posted entry" });

    var lines = await db.JournalLines.Where(l => l.JournalEntryId == id).ToListAsync();
    db.JournalLines.RemoveRange(lines);
    db.JournalEntries.Remove(entry);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Journal entry deleted" });
});

// GL Summary
app.MapGet("/api/gl/summary", async (AppDbContext db) =>
{
    var entries = await db.JournalEntries.Where(j => j.Status == "POSTED").ToListAsync();
    var lines = await db.JournalLines.ToListAsync();

    var postedEntryIds = entries.Select(e => e.Id).ToHashSet();
    var postedLines = lines.Where(l => postedEntryIds.Contains(l.JournalEntryId)).ToList();

    var summary = new GLSummary
    {
        TotalEntries = entries.Count,
        PostedEntries = entries.Count(e => e.Status == "POSTED"),
        TotalDebit = postedLines.Where(l => l.DC == "D").Sum(l => l.Amount),
        TotalCredit = postedLines.Where(l => l.DC == "C").Sum(l => l.Amount)
    };

    // Top accounts by balance
    var accounts = await db.ChartOfAccounts.Where(a => a.IsActive && a.CurrentBalance != 0)
        .OrderByDescending(a => Math.Abs(a.CurrentBalance))
        .Take(10)
        .ToListAsync();

    summary.TopAccounts = accounts.Select(a => new AccountBalance
    {
        AccountCode = a.AccountCode,
        AccountName = a.AccountName,
        AccountType = a.AccountType,
        Balance = a.CurrentBalance
    }).ToList();

    return Results.Ok(summary);
});

// Get Account Ledger
app.MapGet("/api/gl/account/{accountCode}", async (string accountCode, AppDbContext db) =>
{
    var account = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == accountCode);
    if (account == null) return Results.NotFound();

    var lines = await db.JournalLines
        .Include(j => j.JournalEntry)
        .Where(j => j.AccountCode == accountCode && j.JournalEntry!.Status == "POSTED")
        .OrderBy(j => j.JournalEntry!.EntryDate)
        .ThenBy(j => j.Id)
        .ToListAsync();

    return Results.Ok(new
    {
        Account = account,
        Lines = lines,
        TotalDebit = lines.Where(l => l.DC == "D").Sum(l => l.Amount),
        TotalCredit = lines.Where(l => l.DC == "C").Sum(l => l.Amount),
        Balance = account.OpeningBalance + lines.Where(l => l.DC == "D").Sum(l => l.Amount) - lines.Where(l => l.DC == "C").Sum(l => l.Amount)
    });
});

// ==================== FINANCE - AUTO POSTING FROM OTHER MODULES ====================

// Auto-post from Purchase Order (when PO is completed/received)
app.MapPost("/api/gl/auto/po/{poId}", async (int poId, AppDbContext db) =>
{
    var po = await db.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == poId);
    if (po == null) return Results.NotFound();

    var poItems = await db.PurchaseOrderItems.Where(i => i.PurchaseOrderId == poId).ToListAsync();

    // Check if already posted
    var existing = await db.JournalEntries.AnyAsync(j => j.SourceModule == "PO" && j.SourceId == poId.ToString());
    if (existing) return Results.BadRequest(new { error = "PO already posted" });

    var entryDate = po.PODate;
    var entry = new JournalEntry
    {
        EntryNumber = $"JE-PO-{entryDate:yyyyMMdd}-{po.Id}",
        EntryDate = entryDate,
        PeriodMonth = entryDate.ToString("MM"),
        PeriodYear = entryDate.ToString("yyyy"),
        EntryType = "AUTO",
        SourceModule = "PO",
        SourceId = poId.ToString(),
        Description = $"Purchase Order {po.PONumber}",
        Status = "POSTED",
        TotalDebit = po.TotalAmount,
        TotalCredit = po.TotalAmount,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Inventory/Purchase (for each item)
    foreach (var item in poItems)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "1200", // Persediaan
            AccountName = "Persediaan",
            DC = "D",
            Amount = item.TotalPrice,
            Site = po.Site,
            Description = item.Description,
            CreatedAt = DateTime.UtcNow
        });
    }

    // Credit: Accounts Payable
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "2000", // Hutang Usaha
        AccountName = "Hutang Usaha - " + po.Vendor,
        DC = "C",
        Amount = po.SubTotal,
        Site = po.Site,
        Description = $"PO {po.PONumber} - {po.Vendor}",
        CreatedAt = DateTime.UtcNow
    });

    // Tax (if any)
    if (po.Tax > 0)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "2100", // Hutang Pajak
            AccountName = "Hutang PPN",
            DC = "C",
            Amount = po.Tax,
            Site = po.Site,
            Description = $"PPN PO {po.PONumber}",
            CreatedAt = DateTime.UtcNow
        });
    }

    await db.SaveChangesAsync();

    // Update Vendor Outstanding Balance
    var vendor = await db.Vendors.FirstOrDefaultAsync(v => v.VendorName == po.Vendor);
    if (vendor != null)
    {
        vendor.OutstandingBalance += po.TotalAmount;
        vendor.TotalPurchases += po.TotalAmount;
    }

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

// Auto-post from Payroll
app.MapPost("/api/gl/auto/payroll/{payrollId}", async (int payrollId, AppDbContext db) =>
{
    var payroll = await db.Payrolls.FindAsync(payrollId);
    if (payroll == null) return Results.NotFound();

    var existing = await db.JournalEntries.AnyAsync(j => j.SourceModule == "PAYROLL" && j.SourceId == payrollId.ToString());
    if (existing) return Results.BadRequest(new { error = "Payroll already posted" });

    var entryDate = payroll.PayrollDate;
    var entry = new JournalEntry
    {
        EntryNumber = $"JE-PAY-{entryDate:yyyyMM}-{payroll.Id}",
        EntryDate = entryDate,
        PeriodMonth = payroll.PeriodMonth,
        PeriodYear = payroll.PeriodYear,
        EntryType = "AUTO",
        SourceModule = "PAYROLL",
        SourceId = payrollId.ToString(),
        Description = $"Payroll {payroll.PayrollNumber} - {payroll.EmployeeName}",
        Status = "POSTED",
        TotalDebit = payroll.NetSalary + payroll.TaxDeduction + payroll.InsuranceDeduction,
        TotalCredit = payroll.NetSalary + payroll.TaxDeduction + payroll.InsuranceDeduction,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Salary Expense
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "5200", // Biaya Gaji & Upah
        AccountName = "Biaya Gaji & Upah",
        DC = "D",
        Amount = payroll.BasicSalary + payroll.Allowance,
        Site = payroll.Site,
        Description = payroll.EmployeeName,
        CreatedAt = DateTime.UtcNow
    });

    // Debit: Tax Expense (company portion)
    if (payroll.TaxDeduction > 0)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "5200",
            AccountName = "Biaya Pajak Karyawan",
            DC = "D",
            Amount = payroll.TaxDeduction,
            Site = payroll.Site,
            Description = $"PPh 21 {payroll.EmployeeName}",
            CreatedAt = DateTime.UtcNow
        });
    }

    // Credit: Cash/Bank
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "1000", // Kas & Bank
        AccountName = "Kas & Bank",
        DC = "C",
        Amount = payroll.NetSalary,
        Site = payroll.Site,
        Description = $"Gaji {payroll.EmployeeName}",
        CreatedAt = DateTime.UtcNow
    });

    // Credit: Tax Payable
    if (payroll.TaxDeduction > 0)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "2100",
            AccountName = "Hutang PPh 21",
            DC = "C",
            Amount = payroll.TaxDeduction,
            Site = payroll.Site,
            Description = $"PPh 21 {payroll.EmployeeName}",
            CreatedAt = DateTime.UtcNow
        });
    }

    // Credit: Salary Payable
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "2200", // Hutang Gaji
        AccountName = "Hutang Gaji",
        DC = "C",
        Amount = payroll.NetSalary,
        Site = payroll.Site,
        Description = payroll.EmployeeName,
        CreatedAt = DateTime.UtcNow
    });

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

// Auto-post from Fuel Receipt
app.MapPost("/api/gl/auto/fuel/{fuelId}", async (int fuelId, AppDbContext db) =>
{
    var fuel = await db.FuelReceipts.FindAsync(fuelId);
    if (fuel == null) return Results.NotFound();

    var existing = await db.JournalEntries.AnyAsync(j => j.SourceModule == "FUEL" && j.SourceId == fuelId.ToString());
    if (existing) return Results.BadRequest(new { error = "Fuel receipt already posted" });

    var entry = new JournalEntry
    {
        EntryNumber = $"JE-FUEL-{fuel.Tanggal:yyyyMMdd}-{fuel.Id}",
        EntryDate = fuel.Tanggal,
        PeriodMonth = fuel.Tanggal.ToString("MM"),
        PeriodYear = fuel.Tanggal.ToString("yyyy"),
        EntryType = "AUTO",
        SourceModule = "FUEL",
        SourceId = fuelId.ToString(),
        Description = $"Fuel Receipt {fuel.NoTiket} - {fuel.Vendor}",
        Status = "POSTED",
        TotalDebit = fuel.TotalHarga,
        TotalCredit = fuel.TotalHarga,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Fuel Expense
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "5000", // Bahan Bakar
        AccountName = "Bahan Bakar & Lubricant",
        DC = "D",
        Amount = fuel.TotalHarga,
        Site = fuel.Site,
        Description = $"{fuel.Liter} L {fuel.JenisBBM}",
        CreatedAt = DateTime.UtcNow
    });

    // Credit: Cash/Accounts Payable
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "2000",
        AccountName = $"Hutang Usaha - {fuel.Vendor}",
        DC = "C",
        Amount = fuel.TotalHarga,
        Site = fuel.Site,
        Description = fuel.NoTiket,
        CreatedAt = DateTime.UtcNow
    });

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

// Auto-post from Maintenance
app.MapPost("/api/gl/auto/maintenance/{cmId}", async (int cmId, AppDbContext db) =>
{
    var cm = await db.CorrectiveMaintenances.FindAsync(cmId);
    if (cm == null) return Results.NotFound();

    var existing = await db.JournalEntries.AnyAsync(j => j.SourceModule == "MAINTENANCE" && j.SourceId == cmId.ToString());
    if (existing) return Results.BadRequest(new { error = "Maintenance already posted" });

    var totalCost = (cm.RepairCost ?? 0) + (cm.PartsCost ?? 0) + (cm.LaborCost ?? 0);
    if (totalCost == 0) return Results.BadRequest(new { error = "No cost to post" });

    var entry = new JournalEntry
    {
        EntryNumber = $"JE-MAINT-{cm.CMDate:yyyyMMdd}-{cm.Id}",
        EntryDate = cm.CMDate,
        PeriodMonth = cm.CMDate.ToString("MM"),
        PeriodYear = cm.CMDate.ToString("yyyy"),
        EntryType = "AUTO",
        SourceModule = "MAINTENANCE",
        SourceId = cmId.ToString(),
        Description = $"Corrective Maintenance {cm.CMNumber} - {cm.UnitNo}",
        Status = "POSTED",
        TotalDebit = totalCost,
        TotalCredit = totalCost,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Maintenance Expense
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "5100", // Biaya Pemeliharaan
        AccountName = "Biaya Pemeliharaan",
        DC = "D",
        Amount = totalCost,
        Site = cm.Site,
        Description = $"{cm.CMNumber} - {cm.UnitNo}",
        CreatedAt = DateTime.UtcNow
    });

    // Credit: Cash/Spareparts Inventory
    if (cm.PartsCost > 0)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "1200",
            AccountName = "Persediaan Sparepart",
            DC = "C",
            Amount = cm.PartsCost ?? 0,
            Site = cm.Site,
            Description = "Sparepart usage",
            CreatedAt = DateTime.UtcNow
        });
    }

    // Credit: Cash/Bank for cash payments
    var remaining = totalCost - (cm.PartsCost ?? 0);
    if (remaining > 0)
    {
        db.JournalLines.Add(new JournalLine
        {
            JournalEntryId = entry.Id,
            AccountCode = "1000",
            AccountName = "Kas & Bank",
            DC = "C",
            Amount = remaining,
            Site = cm.Site,
            Description = "Payment",
            CreatedAt = DateTime.UtcNow
        });
    }

    await db.SaveChangesAsync();
    return Results.Ok(entry);
});

// ==================== FINANCE - COST PER TON ANALYSIS ====================

app.MapGet("/api/cost-per-ton", async (AppDbContext db, string? site, string? period) =>
{
    var query = db.ProductionData.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(p => p.Site == site);
    if (!string.IsNullOrEmpty(period) && period != "all")
        query = query.Where(p => p.PeriodYear + "-" + p.PeriodMonth == period);

    var data = await query.OrderByDescending(p => p.PeriodYear).ThenByDescending(p => p.PeriodMonth).ToListAsync();

    // Calculate cost per ton if not already set
    foreach (var d in data)
    {
        d.TotalCost = d.FuelCost + d.MaintenanceCost + d.LaborCost + d.OtherCost;
        d.CostPerTon = d.TotalTonase > 0 ? Math.Round(d.TotalCost / d.TotalTonase, 4) : 0;
    }

    return Results.Ok(data);
});

app.MapGet("/api/cost-per-ton/{id}", async (int id, AppDbContext db) =>
{
    var data = await db.ProductionData.FindAsync(id);
    if (data == null) return Results.NotFound();

    data.TotalCost = data.FuelCost + data.MaintenanceCost + data.LaborCost + data.OtherCost;
    data.CostPerTon = data.TotalTonase > 0 ? Math.Round(data.TotalCost / data.TotalTonase, 4) : 0;

    return Results.Ok(data);
});

app.MapPost("/api/cost-per-ton", async (ProductionDataInput input, AppDbContext db) =>
{
    var totalCost = input.FuelCost + input.MaintenanceCost + input.LaborCost + input.OtherCost;
    var costPerTon = input.TotalTonase > 0 ? Math.Round(totalCost / input.TotalTonase, 4) : 0;

    var data = new ProductionData
    {
        PeriodMonth = input.PeriodMonth,
        PeriodYear = input.PeriodYear,
        Site = input.Site,
        Division = input.Division,
        TotalTonase = input.TotalTonase,
        TotalOperatingHours = input.TotalOperatingHours,
        TotalOverburden = input.TotalOverburden,
        HaulingDistance = input.HaulingDistance,
        TotalCost = totalCost,
        CostPerTon = costPerTon,
        FuelCost = input.FuelCost,
        MaintenanceCost = input.MaintenanceCost,
        LaborCost = input.LaborCost,
        OtherCost = input.OtherCost,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.ProductionData.Add(data);
    await db.SaveChangesAsync();
    return Results.Ok(data);
});

app.MapPut("/api/cost-per-ton/{id}", async (int id, ProductionDataInput input, AppDbContext db) =>
{
    var data = await db.ProductionData.FindAsync(id);
    if (data == null) return Results.NotFound();

    data.PeriodMonth = input.PeriodMonth;
    data.PeriodYear = input.PeriodYear;
    data.Site = input.Site;
    data.Division = input.Division;
    data.TotalTonase = input.TotalTonase;
    data.TotalOperatingHours = input.TotalOperatingHours;
    data.TotalOverburden = input.TotalOverburden;
    data.HaulingDistance = input.HaulingDistance;
    data.FuelCost = input.FuelCost;
    data.MaintenanceCost = input.MaintenanceCost;
    data.LaborCost = input.LaborCost;
    data.OtherCost = input.OtherCost;
    data.TotalCost = input.FuelCost + input.MaintenanceCost + input.LaborCost + input.OtherCost;
    data.CostPerTon = input.TotalTonase > 0 ? Math.Round(data.TotalCost / input.TotalTonase, 4) : 0;
    data.Remarks = input.Remarks;
    data.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(data);
});

app.MapDelete("/api/cost-per-ton/{id}", async (int id, AppDbContext db) =>
{
    var data = await db.ProductionData.FindAsync(id);
    if (data == null) return Results.NotFound();

    db.ProductionData.Remove(data);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Production data deleted" });
});

// Auto-calculate Cost Per Ton from actual expenses
app.MapPost("/api/cost-per-ton/calculate/{periodYear}/{periodMonth}", async (string periodYear, string periodMonth, string site, AppDbContext db) =>
{
    // Get fuel cost from journal
    var fuelLines = await db.JournalLines
        .Include(j => j.JournalEntry)
        .Where(j => j.AccountCode == "5000"
            && j.JournalEntry!.PeriodYear == periodYear
            && j.JournalEntry.PeriodMonth == periodMonth
            && j.JournalEntry.Status == "POSTED"
            && (string.IsNullOrEmpty(site) || site == "all" || j.Site == site))
        .ToListAsync();

    var fuelCost = fuelLines.Sum(l => l.Amount);

    // Get maintenance cost
    var maintLines = await db.JournalLines
        .Include(j => j.JournalEntry)
        .Where(j => j.AccountCode == "5100"
            && j.JournalEntry!.PeriodYear == periodYear
            && j.JournalEntry.PeriodMonth == periodMonth
            && j.JournalEntry.Status == "POSTED"
            && (string.IsNullOrEmpty(site) || site == "all" || j.Site == site))
        .ToListAsync();

    var maintCost = maintLines.Sum(l => l.Amount);

    // Get labor cost (salary expense)
    var laborLines = await db.JournalLines
        .Include(j => j.JournalEntry)
        .Where(j => j.AccountCode == "5200"
            && j.JournalEntry!.PeriodYear == periodYear
            && j.JournalEntry.PeriodMonth == periodMonth
            && j.JournalEntry.Status == "POSTED"
            && (string.IsNullOrEmpty(site) || site == "all" || j.Site == site))
        .ToListAsync();

    var laborCost = laborLines.Sum(l => l.Amount);

    return Results.Ok(new
    {
        Period = $"{periodYear}-{periodMonth}",
        Site = site ?? "ALL",
        FuelCost = fuelCost,
        MaintenanceCost = maintCost,
        LaborCost = laborCost,
        OtherCost = 0m,
        TotalCost = fuelCost + maintCost + laborCost
    });
});

// Cost Per Ton Summary
app.MapGet("/api/cost-per-ton/summary", async (AppDbContext db, string? site) =>
{
    var query = db.ProductionData.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(p => p.Site == site);

    var data = await query.ToListAsync();

    var summary = data.Select(d => new CostPerTonSummary
    {
        Period = $"{d.PeriodYear}-{d.PeriodMonth}",
        Site = d.Site,
        TotalTonase = d.TotalTonase,
        TotalCost = d.TotalCost,
        CostPerTon = d.CostPerTon,
        FuelCost = d.FuelCost,
        MaintenanceCost = d.MaintenanceCost,
        LaborCost = d.LaborCost,
        OtherCost = d.OtherCost,
        FuelCostPerTon = d.TotalTonase > 0 ? Math.Round(d.FuelCost / d.TotalTonase, 4) : 0,
        MaintCostPerTon = d.TotalTonase > 0 ? Math.Round(d.MaintenanceCost / d.TotalTonase, 4) : 0
    }).ToList();

    return Results.Ok(summary);
});

// ==================== HAULING OPERATIONS - HAUL TRIP / RITASE ====================

// HaulTrip CRUD
app.MapGet("/api/haul-trips", async (AppDbContext db, string? site, string? date, string? unitNo, string? status) =>
{
    var query = db.HaulTrips.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(t => t.Site == site);
    if (!string.IsNullOrEmpty(date)) query = query.Where(t => t.TripDate.Date == DateTime.Parse(date).Date);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(t => t.UnitNo == unitNo);
    if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);

    var trips = await query.OrderByDescending(t => t.TripDate).ThenByDescending(t => t.TripNumber).ToListAsync();
    return Results.Ok(trips);
});

app.MapGet("/api/haul-trips/{id}", async (int id, AppDbContext db) =>
{
    var trip = await db.HaulTrips.FindAsync(id);
    return trip == null ? Results.NotFound() : Results.Ok(trip);
});

app.MapPost("/api/haul-trips", async (HaulTripInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.UnitNo)) return Results.BadRequest(new { error = "UnitNo wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.DriverId)) return Results.BadRequest(new { error = "DriverId wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.RouteCode)) return Results.BadRequest(new { error = "RouteCode wajib diisi" });

    // === VALIDASI NEGATIF ===
    if ((input.GrossWeight ?? 0) < 0) return Results.BadRequest(new { error = "GrossWeight tidak boleh negatif" });
    if ((input.TareWeight ?? 0) < 0) return Results.BadRequest(new { error = "TareWeight tidak boleh negatif" });
    if ((input.PayloadTon ?? 0) < 0) return Results.BadRequest(new { error = "PayloadTon tidak boleh negatif" });
    if ((input.DistanceKM ?? 0) < 0) return Results.BadRequest(new { error = "DistanceKM tidak boleh negatif" });
    if ((input.FuelConsumed ?? 0) < 0) return Results.BadRequest(new { error = "FuelConsumed tidak boleh negatif" });
    if ((input.RatePerTon ?? 0) < 0) return Results.BadRequest(new { error = "RatePerTon tidak boleh negatif" });
    if ((input.HMStart ?? 0) < 0) return Results.BadRequest(new { error = "HMStart tidak boleh negatif" });
    if ((input.HMEnd ?? 0) < 0) return Results.BadRequest(new { error = "HMEnd tidak boleh negatif" });
    if ((input.CycleTimeMinutes ?? 0) < 0) return Results.BadRequest(new { error = "CycleTimeMinutes tidak boleh negatif" });

    // === VALIDASI BERAT: GrossWeight harus > TareWeight ===
    if (input.GrossWeight > 0 && input.TareWeight > 0 && input.GrossWeight <= input.TareWeight)
        return Results.BadRequest(new { error = "GrossWeight harus lebih besar dari TareWeight" });

    // === VALIDASI HM: HMEnd >= HMStart ===
    if (input.HMEnd.HasValue && input.HMStart.HasValue && input.HMEnd < input.HMStart)
        return Results.BadRequest(new { error = $"HMEnd ({input.HMEnd}) tidak boleh lebih kecil dari HMStart ({input.HMStart})" });

    // === VALIDASI WAKTU: EndTime >= StartTime ===
    if (input.EndTime.HasValue && input.StartTime.HasValue && input.EndTime <= input.StartTime)
        return Results.BadRequest(new { error = "EndTime harus lebih besar dari StartTime" });

    // Auto-generate trip number
    var tripDate = input.TripDate ?? DateTime.UtcNow;
    var tripCount = await db.HaulTrips.CountAsync(t => t.TripDate.Date == tripDate.Date) + 1;
    var tripNumber = input.TripNumber ?? $"TR{tripDate:yyyyMMdd}{tripCount:D4}";

    // === VALIDASI DUPLIKAT TripNumber ===
    var existingTrip = await db.HaulTrips.FirstOrDefaultAsync(t => t.TripNumber == tripNumber);
    if (existingTrip != null) return Results.BadRequest(new { error = $"TripNumber '{tripNumber}' sudah ada" });

    // Auto-calculate net weight and fuel ratios
    var grossWeight = input.GrossWeight ?? 0;
    var tareWeight = input.TareWeight ?? 0;
    var netWeight = grossWeight - tareWeight;
    var payloadTon = input.PayloadTon ?? (netWeight > 0 ? netWeight / 1000 : 0);
    var distanceKM = input.DistanceKM ?? 0;
    var fuelConsumed = input.FuelConsumed ?? 0;
    var ratePerTon = input.RatePerTon ?? 0;
    var tripRevenue = payloadTon * ratePerTon;

    // Calculate fuel ratios
    var lperKM = distanceKM > 0 ? fuelConsumed / distanceKM : 0;
    var tonKM = payloadTon * distanceKM;
    var lperTonKM = tonKM > 0 ? fuelConsumed / tonKM : 0;

    // Calculate cycle time
    var cycleMinutes = 0m;
    if (input.StartTime.HasValue && input.EndTime.HasValue)
    {
        cycleMinutes = (decimal)(input.EndTime.Value - input.StartTime.Value).TotalMinutes;
    }

    var trip = new HaulTrip
    {
        TripNumber = tripNumber,
        TripDate = tripDate,
        Site = input.Site,
        UnitNo = input.UnitNo,
        DriverId = input.DriverId,
        DriverName = input.DriverName,
        RouteCode = input.RouteCode,
        RouteName = input.RouteName,
        Shift = input.Shift ?? "SHIFT_1",
        MaterialType = input.MaterialType,
        OriginPit = input.OriginPit,
        DestinationStockpile = input.DestinationStockpile,
        WBTicketIn = input.WBTicketIn,
        WBTicketOut = input.WBTicketOut,
        GrossWeight = input.GrossWeight,
        TareWeight = input.TareWeight,
        NetWeight = netWeight,
        PayloadTon = payloadTon,
        StartTime = input.StartTime,
        EndTime = input.EndTime,
        CycleTimeMinutes = cycleMinutes > 0 ? cycleMinutes : input.CycleTimeMinutes,
        LoadingTimeMinutes = input.LoadingTimeMinutes,
        HaulingTimeMinutes = input.HaulingTimeMinutes,
        DumpingTimeMinutes = input.DumpingTimeMinutes,
        ReturningTimeMinutes = input.ReturningTimeMinutes,
        DistanceKM = distanceKM,
        FuelConsumed = fuelConsumed,
        FuelRatioLperKM = lperKM,
        FuelRatioLperTonKM = lperTonKM,
        HMStart = input.HMStart,
        HMEnd = input.HMEnd,
        RatePerTon = ratePerTon,
        TripRevenue = tripRevenue,
        TripCost = input.TripCost ?? 0,
        Status = "COMPLETED",
        Remarks = input.Remarks,
        IsWBValidated = false,
        IsRevenuePosted = false,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.HaulTrips.Add(trip);
    await db.SaveChangesAsync();
    return Results.Ok(trip);
});

app.MapPut("/api/haul-trips/{id}", async (int id, HaulTripInput input, AppDbContext db) =>
{
    var trip = await db.HaulTrips.FindAsync(id);
    if (trip == null) return Results.NotFound();

    var netWeight = (input.GrossWeight ?? 0) - (input.TareWeight ?? 0);
    var payloadTon = input.PayloadTon ?? (netWeight > 0 ? netWeight / 1000 : trip.PayloadTon);
    var distanceKM = input.DistanceKM ?? trip.DistanceKM ?? 0;
    var fuelConsumed = input.FuelConsumed ?? trip.FuelConsumed ?? 0;
    var ratePerTon = input.RatePerTon ?? trip.RatePerTon ?? 0;
    var tonKM = payloadTon * distanceKM;
    var lperTonKM = tonKM > 0 ? fuelConsumed / tonKM : 0;

    trip.TripDate = input.TripDate ?? trip.TripDate;
    trip.Site = input.Site;
    trip.UnitNo = input.UnitNo;
    trip.DriverId = input.DriverId;
    trip.DriverName = input.DriverName;
    trip.RouteCode = input.RouteCode;
    trip.RouteName = input.RouteName;
    trip.Shift = input.Shift ?? trip.Shift;
    trip.MaterialType = input.MaterialType;
    trip.OriginPit = input.OriginPit;
    trip.DestinationStockpile = input.DestinationStockpile;
    trip.WBTicketIn = input.WBTicketIn;
    trip.WBTicketOut = input.WBTicketOut;
    trip.GrossWeight = input.GrossWeight;
    trip.TareWeight = input.TareWeight;
    trip.NetWeight = netWeight;
    trip.PayloadTon = payloadTon;
    trip.DistanceKM = distanceKM;
    trip.FuelConsumed = fuelConsumed;
    trip.FuelRatioLperKM = distanceKM > 0 ? fuelConsumed / distanceKM : 0;
    trip.FuelRatioLperTonKM = lperTonKM;
    trip.RatePerTon = ratePerTon;
    trip.TripRevenue = payloadTon * ratePerTon;
    trip.TripCost = input.TripCost ?? trip.TripCost;
    trip.Remarks = input.Remarks;
    trip.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(trip);
});

app.MapDelete("/api/haul-trips/{id}", async (int id, AppDbContext db) =>
{
    var trip = await db.HaulTrips.FindAsync(id);
    if (trip == null) return Results.NotFound();
    db.HaulTrips.Remove(trip);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Trip deleted" });
});

// HaulTrip Summary
app.MapGet("/api/haul-trips/summary", async (AppDbContext db, string? site, string? month, string? year) =>
{
    var query = db.HaulTrips.Where(t => t.Status == "COMPLETED").AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(t => t.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(t => t.TripDate.ToString("MM") == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(t => t.TripDate.ToString("yyyy") == year);

    var trips = await query.ToListAsync();

    var summary = new HaulTripSummary
    {
        Site = site ?? "ALL",
        Period = $"{year ?? "ALL"}-{month ?? "ALL"}",
        TotalTrips = trips.Count,
        TotalTon = trips.Sum(t => t.PayloadTon ?? 0),
        TotalKM = trips.Sum(t => t.DistanceKM ?? 0),
        TotalHours = trips.Sum(t => (t.CycleTimeMinutes ?? 0) / 60),
        TotalFuel = trips.Sum(t => t.FuelConsumed ?? 0),
        TotalRevenue = trips.Sum(t => t.TripRevenue ?? 0),
        TotalCost = trips.Sum(t => t.TripCost ?? 0),
        AvgCycleTimeMinutes = trips.Count > 0 ? trips.Average(t => t.CycleTimeMinutes ?? 0) : 0,
        AvgPayloadTon = trips.Count > 0 ? trips.Average(t => t.PayloadTon ?? 0) : 0,
        AvgFuelPerTrip = trips.Count > 0 ? trips.Average(t => t.FuelConsumed ?? 0) : 0
    };
    summary.GrossProfit = summary.TotalRevenue - summary.TotalCost;
    summary.ProfitMargin = summary.TotalRevenue > 0 ? Math.Round(summary.GrossProfit / summary.TotalRevenue * 100, 2) : 0;

    return Results.Ok(summary);
});

// Auto-post Revenue to GL when trip is completed
app.MapPost("/api/haul-trips/{id}/post-revenue", async (int id, AppDbContext db) =>
{
    var trip = await db.HaulTrips.FindAsync(id);
    if (trip == null) return Results.NotFound();
    if (trip.IsRevenuePosted) return Results.BadRequest(new { error = "Revenue already posted" });
    if (trip.TripRevenue == null || trip.TripRevenue <= 0) return Results.BadRequest(new { error = "No revenue to post" });

    // Get revenue account (default: 4000 - Pendapatan Angkutan)
    var revenueAccount = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == "4000");
    var arAccount = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == "1101"); // Piutang
    var routeCostAccount = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == "5103"); // Biaya Angkutan per Trip

    // Create journal entry for revenue recognition
    var entryDate = trip.TripDate;
    var entry = new JournalEntry
    {
        EntryNumber = $"JE-HAUL-{entryDate:yyyyMMdd}-{trip.Id}",
        EntryDate = entryDate,
        PeriodMonth = entryDate.ToString("MM"),
        PeriodYear = entryDate.ToString("yyyy"),
        EntryType = "AUTO",
        SourceModule = "HAUL",
        SourceId = trip.Id.ToString(),
        Description = $"Revenue Trip {trip.TripNumber} - Unit {trip.UnitNo} - {trip.PayloadTon} ton",
        Status = "POSTED",
        TotalDebit = trip.TripRevenue ?? 0,
        TotalCredit = trip.TripRevenue ?? 0,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Piutang (AR) - debit balance
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "1101",
        AccountName = arAccount?.AccountName ?? "Piutang Usaha",
        DC = "D",
        Amount = trip.TripRevenue ?? 0,
        CostCenter = trip.UnitNo,
        Site = trip.Site,
        Description = $"Piutang Angkutan - Trip {trip.TripNumber}",
        CreatedAt = DateTime.UtcNow
    });

    // Credit: Pendapatan Angkutan - credit balance
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "4000",
        AccountName = revenueAccount?.AccountName ?? "Pendapatan Angkutan",
        DC = "C",
        Amount = trip.TripRevenue ?? 0,
        CostCenter = trip.UnitNo,
        Site = trip.Site,
        Description = $"Revenue Trip {trip.TripNumber}",
        CreatedAt = DateTime.UtcNow
    });

    trip.IsRevenuePosted = true;
    trip.Status = "VALIDATED";
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Revenue posted to GL", entryId = entry.Id, revenue = trip.TripRevenue });
});

// ==================== ROUTE MASTER ====================
app.MapGet("/api/routes", async (AppDbContext db, string? site, string? status) =>
{
    var query = db.RouteMasters.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(r => r.Site == site);
    if (!string.IsNullOrEmpty(status)) query = query.Where(r => r.Status == status);
    return Results.Ok(await query.OrderBy(r => r.RouteCode).ToListAsync());
});

app.MapGet("/api/routes/{id}", async (int id, AppDbContext db) =>
{
    var route = await db.RouteMasters.FindAsync(id);
    return route == null ? Results.NotFound() : Results.Ok(route);
});

app.MapPost("/api/routes", async (RouteMasterInput input, AppDbContext db) =>
{
    var existing = await db.RouteMasters.FirstOrDefaultAsync(r => r.RouteCode == input.RouteCode);
    if (existing != null) return Results.BadRequest(new { error = "Route code already exists" });

    var route = new RouteMaster
    {
        RouteCode = input.RouteCode!,
        RouteName = input.RouteName,
        Site = input.Site,
        OriginPit = input.OriginPit,
        Destination = input.Destination,
        DistanceKM = input.DistanceKM ?? 0,
        EstimatedCycleTime = input.EstimatedCycleTime ?? 0,
        GradePercent = input.GradePercent ?? 0,
        RoadType = input.RoadType,
        EstimatedFuelPerTrip = input.EstimatedFuelPerTrip,
        CostPerKM = input.CostPerKM,
        CostPerTonKM = input.CostPerTonKM,
        RatePerTon = input.RatePerTon,
        Remarks = input.Remarks,
        Status = "ACTIVE",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.RouteMasters.Add(route);
    await db.SaveChangesAsync();
    return Results.Ok(route);
});

app.MapPut("/api/routes/{id}", async (int id, RouteMasterInput input, AppDbContext db) =>
{
    var route = await db.RouteMasters.FindAsync(id);
    if (route == null) return Results.NotFound();

    route.RouteName = input.RouteName;
    route.Site = input.Site;
    route.OriginPit = input.OriginPit;
    route.Destination = input.Destination;
    route.DistanceKM = input.DistanceKM ?? route.DistanceKM;
    route.EstimatedCycleTime = input.EstimatedCycleTime ?? route.EstimatedCycleTime;
    route.GradePercent = input.GradePercent ?? route.GradePercent;
    route.RoadType = input.RoadType;
    route.EstimatedFuelPerTrip = input.EstimatedFuelPerTrip ?? route.EstimatedFuelPerTrip;
    route.CostPerKM = input.CostPerKM ?? route.CostPerKM;
    route.CostPerTonKM = input.CostPerTonKM ?? route.CostPerTonKM;
    route.RatePerTon = input.RatePerTon ?? route.RatePerTon;
    route.Remarks = input.Remarks;
    route.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(route);
});

app.MapDelete("/api/routes/{id}", async (int id, AppDbContext db) =>
{
    var route = await db.RouteMasters.FindAsync(id);
    if (route == null) return Results.NotFound();
    db.RouteMasters.Remove(route);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Route deleted" });
});

// Route efficiency summary
app.MapGet("/api/routes/efficiency", async (AppDbContext db, string? site, string? month, string? year) =>
{
    var query = db.HaulTrips.Where(t => t.Status == "COMPLETED").AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(t => t.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(t => t.TripDate.ToString("MM") == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(t => t.TripDate.ToString("yyyy") == year);

    var trips = await query.ToListAsync();
    var routes = await db.RouteMasters.ToListAsync();

    var summaries = trips.GroupBy(t => t.RouteCode).Select(g =>
    {
        var route = routes.FirstOrDefault(r => r.RouteCode == g.Key);
        var routeTrips = g.ToList();
        var totalTon = routeTrips.Sum(t => t.PayloadTon ?? 0);
        var totalTonKM = routeTrips.Sum(t => (t.PayloadTon ?? 0) * (t.DistanceKM ?? 0));
        var totalFuel = routeTrips.Sum(t => t.FuelConsumed ?? 0);
        var lperTonKM = totalTonKM > 0 ? totalFuel / totalTonKM : 0;
        var routeRevenue = routeTrips.Sum(t => t.TripRevenue ?? 0);

        return new RouteEfficiencySummary
        {
            RouteCode = g.Key,
            RouteName = route?.RouteName ?? g.Key,
            Site = route?.Site ?? "UNKNOWN",
            TotalTrips = routeTrips.Count,
            TotalTon = totalTon,
            TotalTonKM = totalTonKM,
            AvgPayloadTon = routeTrips.Count > 0 ? routeTrips.Average(t => t.PayloadTon ?? 0) : 0,
            AvgCycleTimeMinutes = routeTrips.Count > 0 ? routeTrips.Average(t => t.CycleTimeMinutes ?? 0) : 0,
            AvgFuelPerTrip = routeTrips.Count > 0 ? routeTrips.Average(t => t.FuelConsumed ?? 0) : 0,
            LitrePerTonKM = lperTonKM,
            ActualCostPerTonKM = totalTonKM > 0 ? routeTrips.Sum(t => t.TripCost ?? 0) / totalTonKM : 0,
            RouteRatePerTon = route?.RatePerTon ?? 0,
            RouteRevenue = routeRevenue,
            RouteProfit = routeRevenue - routeTrips.Sum(t => t.TripCost ?? 0)
        };
    }).ToList();

    return Results.Ok(summaries);
});

// ==================== WEIGHBRIDGE TICKETS ====================
app.MapGet("/api/weighbridge", async (AppDbContext db, string? site, string? date, string? status, string? unitNo) =>
{
    var query = db.WeighbridgeTickets.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(w => w.Site == site);
    if (!string.IsNullOrEmpty(date)) query = query.Where(w => w.TicketDate.Date == DateTime.Parse(date).Date);
    if (!string.IsNullOrEmpty(status)) query = query.Where(w => w.Status == status);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(w => w.UnitNo == unitNo);

    return Results.Ok(await query.OrderByDescending(w => w.TicketDate).ThenByDescending(w => w.TicketNumber).ToListAsync());
});

app.MapGet("/api/weighbridge/{id}", async (int id, AppDbContext db) =>
{
    var ticket = await db.WeighbridgeTickets.FindAsync(id);
    return ticket == null ? Results.NotFound() : Results.Ok(ticket);
});

app.MapPost("/api/weighbridge", async (WeighbridgeTicketInput input, AppDbContext db) =>
{
    // === VALIDASI REQUIRED FIELDS ===
    if (string.IsNullOrWhiteSpace(input.UnitNo)) return Results.BadRequest(new { error = "UnitNo wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.Site)) return Results.BadRequest(new { error = "Site wajib diisi" });
    if (string.IsNullOrWhiteSpace(input.TicketType)) return Results.BadRequest(new { error = "TicketType wajib diisi" });

    // === VALIDASI NEGATIF ===
    if ((input.FirstWeight ?? 0) < 0) return Results.BadRequest(new { error = "FirstWeight tidak boleh negatif" });
    if ((input.SecondWeight ?? 0) < 0) return Results.BadRequest(new { error = "SecondWeight tidak boleh negatif" });
    if ((input.NetWeight ?? 0) < 0) return Results.BadRequest(new { error = "NetWeight tidak boleh negatif" });
    if ((input.TareCompensation ?? 0) < 0) return Results.BadRequest(new { error = "TareCompensation tidak boleh negatif" });
    if ((input.AxleLoad ?? 0) < 0) return Results.BadRequest(new { error = "AxleLoad tidak boleh negatif" });

    // Auto-generate ticket number
    var ticketDate = input.TicketDate ?? DateTime.UtcNow;
    var ticketNumber = input.TicketNumber ?? $"WB-{ticketDate:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

    // === VALIDASI DUPLIKAT TicketNumber ===
    var existingTicket = await db.WeighbridgeTickets.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
    if (existingTicket != null) return Results.BadRequest(new { error = $"TicketNumber '{ticketNumber}' sudah ada" });

    var netWeight = input.NetWeight ?? 0m;
    if (input.FirstWeight > 0 && input.SecondWeight > 0)
    {
        if (input.TicketType == "IN")
            netWeight = input.SecondWeight ?? 0; // Second weigh (loaded) - First (empty)
        else
            netWeight = input.FirstWeight ?? 0; // First weigh (loaded) - Second (empty)
    }

    var finalNetWeight = netWeight - (input.TareCompensation ?? 0);

    var ticket = new WeighbridgeTicket
    {
        TicketNumber = ticketNumber,
        TicketDate = ticketDate,
        Site = input.Site,
        TicketType = input.TicketType,
        UnitNo = input.UnitNo,
        DriverName = input.DriverName,
        DriverBadge = input.DriverBadge,
        FirstWeight = input.FirstWeight,
        SecondWeight = input.SecondWeight,
        NetWeight = netWeight,
        MaterialType = input.MaterialType,
        OriginPit = input.OriginPit,
        DestinationStockpile = input.DestinationStockpile,
        TareCompensation = input.TareCompensation,
        FinalNetWeight = finalNetWeight,
        VehicleType = input.VehicleType,
        AxleLoad = input.AxleLoad,
        WeighbridgeOperator = input.WeighbridgeOperator,
        FirstWeighTime = input.FirstWeighTime,
        SecondWeighTime = input.SecondWeighTime,
        TripNumber = input.TripNumber,
        IsLinked = false,
        Status = "ACTIVE",
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.WeighbridgeTickets.Add(ticket);
    await db.SaveChangesAsync();
    return Results.Ok(ticket);
});

app.MapPost("/api/weighbridge/{id}/link-trip", async (int id, AppDbContext db, string tripNumber) =>
{
    var ticket = await db.WeighbridgeTickets.FindAsync(id);
    if (ticket == null) return Results.NotFound();

    var trip = await db.HaulTrips.FirstOrDefaultAsync(t => t.TripNumber == tripNumber);
    if (trip == null) return Results.NotFound();

    // Validate: update trip with weighbridge data
    if (ticket.TicketType == "IN")
    {
        trip.WBTicketIn = ticket.TicketNumber;
        trip.TareWeight = ticket.FinalNetWeight ?? ticket.NetWeight;
    }
    else // OUT
    {
        trip.WBTicketOut = ticket.TicketNumber;
        trip.GrossWeight = ticket.FinalNetWeight ?? ticket.NetWeight;
        // Auto-calculate net weight
        if (trip.TareWeight > 0 && trip.GrossWeight > 0)
        {
            trip.NetWeight = trip.GrossWeight - trip.TareWeight;
            trip.PayloadTon = Math.Max(0m, (trip.NetWeight ?? 0) / 1000m);
            // Recalculate revenue
            if (trip.RatePerTon > 0)
                trip.TripRevenue = trip.PayloadTon * trip.RatePerTon;
        }
        trip.IsWBValidated = true;
    }

    ticket.IsLinked = true;
    ticket.TripNumber = tripNumber;

    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Linked to trip", tripNumber });
});

// Weighbridge reconciliation - compare WB tickets vs trip inputs
app.MapGet("/api/weighbridge/reconciliation", async (AppDbContext db, string? site, string? month, string? year) =>
{
    var query = db.WeighbridgeTickets.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(w => w.Site == site);

    var tickets = await query.ToListAsync();
    var trips = await db.HaulTrips.ToListAsync();

    var linkedTickets = tickets.Where(t => t.IsLinked).ToList();
    var unlinkedTickets = tickets.Where(t => !t.IsLinked && t.TicketType == "OUT").ToList();

    var linkedTon = linkedTickets.Sum(t => (t.FinalNetWeight ?? t.NetWeight ?? 0) / 1000);
    var tripInputTon = trips.Sum(t => t.PayloadTon ?? 0);
    var varianceTon = linkedTon - tripInputTon;
    var variancePercent = linkedTon > 0 ? Math.Round(varianceTon / linkedTon * 100, 2) : 0;

    var reconciliation = new WeighbridgeReconciliation
    {
        TotalTickets = tickets.Count,
        LinkedTrips = linkedTickets.Count,
        UnlinkedTickets = unlinkedTickets.Count,
        LinkedTon = linkedTon,
        UnlinkedTon = unlinkedTickets.Sum(t => (t.FinalNetWeight ?? t.NetWeight ?? 0) / 1000),
        VarianceTon = varianceTon,
        VariancePercent = variancePercent,
        UnlinkedList = unlinkedTickets
    };

    return Results.Ok(reconciliation);
});

// ==================== FUEL ANALYSIS ====================
app.MapGet("/api/fuel-analysis", async (AppDbContext db, string? site, string? month, string? year, string? unitNo) =>
{
    var query = db.FuelAnalyses.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(f => f.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(f => f.PeriodMonth == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(f => f.PeriodYear == year);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(f => f.UnitNo == unitNo);
    return Results.Ok(await query.OrderByDescending(f => f.PeriodYear).ThenByDescending(f => f.PeriodMonth).ToListAsync());
});

// Auto-calculate fuel analysis from actual trip data
app.MapPost("/api/fuel-analysis/calculate", async (FuelAnalysisInput input, AppDbContext db) =>
{
    var periodStart = new DateTime(int.Parse(input.PeriodYear), int.Parse(input.PeriodMonth), 1);
    var periodEnd = periodStart.AddMonths(1);

    var trips = await db.HaulTrips
        .Where(t => t.Site == input.Site &&
                    t.TripDate >= periodStart && t.TripDate < periodEnd &&
                    t.Status == "COMPLETED" &&
                    (string.IsNullOrEmpty(input.UnitNo) || t.UnitNo == input.UnitNo) &&
                    (string.IsNullOrEmpty(input.RouteCode) || t.RouteCode == input.RouteCode))
        .ToListAsync();

    var fuelUsages = await db.FuelUsages
        .Where(f => f.Site == input.Site &&
                    f.Tanggal >= periodStart && f.Tanggal < periodEnd &&
                    (string.IsNullOrEmpty(input.UnitNo) || f.UnitNo == input.UnitNo))
        .ToListAsync();

    // Group by unit
    var unitGroups = trips.GroupBy(t => t.UnitNo);
    var results = new List<FuelAnalysis>();

    foreach (var group in unitGroups)
    {
        var unitTrips = group.ToList();
        var unitFuelUsages = fuelUsages.Where(f => f.UnitNo == group.Key).ToList();

        var totalFuelLitres = unitFuelUsages.Sum(f => f.Pemakaian) + unitTrips.Sum(t => t.FuelConsumed ?? 0);
        var totalKM = unitTrips.Sum(t => t.DistanceKM ?? 0);
        var totalTon = unitTrips.Sum(t => t.PayloadTon ?? 0);
        var totalTonKM = unitTrips.Sum(t => (t.PayloadTon ?? 0) * (t.DistanceKM ?? 0));
        var totalTrips = unitTrips.Count;
        var totalHours = unitTrips.Sum(t => (t.CycleTimeMinutes ?? 0) / 60);

        // Get fuel price
        var fuelPrice = await db.FuelReceipts
            .Where(fr => fr.Site == input.Site)
            .OrderByDescending(fr => fr.Tanggal)
            .Select(fr => fr.HargaPerLiter)
            .FirstOrDefaultAsync();

        var benchmark = input.BenchmarkLitrePerKM ?? 0;
        var actualLperKM = totalKM > 0 ? totalFuelLitres / totalKM : 0;
        var variance = benchmark > 0 ? Math.Round((actualLperKM - benchmark) / benchmark * 100, 2) : 0;

        var analysis = new FuelAnalysis
        {
            PeriodMonth = input.PeriodMonth,
            PeriodYear = input.PeriodYear,
            Site = input.Site,
            UnitNo = group.Key,
            RouteCode = input.RouteCode,
            TotalFuelLitres = totalFuelLitres,
            TotalKM = totalKM,
            TotalTonKM = totalTonKM,
            TotalTon = totalTon,
            TotalTrips = totalTrips,
            TotalHours = totalHours,
            LitrePerKM = totalKM > 0 ? Math.Round(totalFuelLitres / totalKM, 4) : 0,
            LitrePerTon = totalTon > 0 ? Math.Round(totalFuelLitres / totalTon, 4) : 0,
            LitrePerTonKM = totalTonKM > 0 ? Math.Round(totalFuelLitres / totalTonKM, 4) : 0,
            LitrePerHour = totalHours > 0 ? Math.Round(totalFuelLitres / totalHours, 4) : 0,
            FuelCost = totalFuelLitres * fuelPrice,
            FuelCostPerTon = totalTon > 0 ? Math.Round(totalFuelLitres * fuelPrice / totalTon, 4) : 0,
            BenchmarkLitrePerKM = benchmark,
            VariancePercent = variance,
            CreatedAt = DateTime.UtcNow
        };

        // Remove existing analysis for same period/unit
        var existing = await db.FuelAnalyses
            .FirstOrDefaultAsync(f => f.PeriodMonth == input.PeriodMonth &&
                                      f.PeriodYear == input.PeriodYear &&
                                      f.Site == input.Site &&
                                      f.UnitNo == group.Key);
        if (existing != null)
        {
            existing.TotalFuelLitres = analysis.TotalFuelLitres;
            existing.TotalKM = analysis.TotalKM;
            existing.TotalTonKM = analysis.TotalTonKM;
            existing.TotalTon = analysis.TotalTon;
            existing.TotalTrips = analysis.TotalTrips;
            existing.TotalHours = analysis.TotalHours;
            existing.LitrePerKM = analysis.LitrePerKM;
            existing.LitrePerTon = analysis.LitrePerTon;
            existing.LitrePerTonKM = analysis.LitrePerTonKM;
            existing.LitrePerHour = analysis.LitrePerHour;
            existing.FuelCost = analysis.FuelCost;
            existing.FuelCostPerTon = analysis.FuelCostPerTon;
            existing.VariancePercent = analysis.VariancePercent;
        }
        else
        {
            db.FuelAnalyses.Add(analysis);
        }

        results.Add(analysis);
    }

    await db.SaveChangesAsync();
    return Results.Ok(results);
});

// Fuel efficiency summary
app.MapGet("/api/fuel-analysis/summary", async (AppDbContext db, string? site, string? month, string? year) =>
{
    var query = db.FuelAnalyses.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(f => f.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(f => f.PeriodMonth == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(f => f.PeriodYear == year);

    var data = await query.ToListAsync();

    var summary = new FuelEfficiencySummary
    {
        Site = site ?? "ALL",
        TotalUnits = data.Count,
        TotalFuelLitres = data.Sum(f => f.TotalFuelLitres),
        TotalKM = data.Sum(f => f.TotalKM),
        TotalTon = data.Sum(f => f.TotalTon),
        TotalTonKM = data.Sum(f => f.TotalTonKM),
        TotalTrips = data.Sum(f => f.TotalTrips),
        AvgLitrePerKM = data.Count > 0 ? Math.Round(data.Average(f => f.LitrePerKM), 4) : 0,
        AvgLitrePerTon = data.Count > 0 ? Math.Round(data.Average(f => f.LitrePerTon), 4) : 0,
        AvgLitrePerTonKM = data.Count > 0 ? Math.Round(data.Average(f => f.LitrePerTonKM), 4) : 0,
        AvgLitrePerHour = data.Count > 0 ? Math.Round(data.Average(f => f.LitrePerHour), 4) : 0,
        TotalFuelCost = data.Sum(f => f.FuelCost),
        FuelCostPerTon = data.Sum(f => f.TotalTon) > 0
            ? Math.Round(data.Sum(f => f.FuelCost) / data.Sum(f => f.TotalTon), 4) : 0,
        UnderPerformers = data.Count(f => f.VariancePercent > 10),
        OverPerformers = data.Count(f => f.VariancePercent < -10)
    };

    return Results.Ok(summary);
});

// ==================== UNIT COST TRACKING ====================
app.MapGet("/api/unit-cost", async (AppDbContext db, string? site, string? month, string? year, string? unitNo) =>
{
    var query = db.UnitCostTrackings.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(u => u.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(u => u.PeriodMonth == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(u => u.PeriodYear == year);
    if (!string.IsNullOrEmpty(unitNo)) query = query.Where(u => u.UnitNo == unitNo);
    return Results.Ok(await query.OrderByDescending(u => u.PeriodYear).ThenByDescending(u => u.PeriodMonth).ToListAsync());
});

// Auto-calculate unit cost from actual data
app.MapPost("/api/unit-cost/calculate", async (UnitCostTrackingInput input, AppDbContext db) =>
{
    var periodStart = new DateTime(int.Parse(input.PeriodYear), int.Parse(input.PeriodMonth), 1);
    var periodEnd = periodStart.AddMonths(1);

    var vehicle = await db.FleetVehicles.FirstOrDefaultAsync(v => v.UnitNo == input.UnitNo && v.Site == input.Site);
    var costCenter = input.CostCenter ?? vehicle?.CostCenter;

    // Get trip data
    var trips = await db.HaulTrips
        .Where(t => t.Site == input.Site && t.UnitNo == input.UnitNo &&
                    t.TripDate >= periodStart && t.TripDate < periodEnd && t.Status == "COMPLETED")
        .ToListAsync();

    // Get fuel cost from FuelAnalysis
    var fuelAnalysis = await db.FuelAnalyses
        .FirstOrDefaultAsync(f => f.Site == input.Site && f.UnitNo == input.UnitNo &&
                                  f.PeriodMonth == input.PeriodMonth && f.PeriodYear == input.PeriodYear);
    var fuelCost = fuelAnalysis?.FuelCost ?? 0;

    // Get maintenance cost from CorrectiveMaintenance
    var maintRecords = await db.CorrectiveMaintenances
        .Where(c => c.Site == input.Site && c.UnitNo == input.UnitNo &&
                    c.CMDate >= periodStart && c.CMDate < periodEnd && c.Status == "COMPLETED")
        .ToListAsync();
    var maintenanceCost = maintRecords.Sum(c => (c.RepairCost ?? 0) + (c.PartsCost ?? 0) + (c.LaborCost ?? 0));

    // Get driver cost from Payroll (approximate)
    var driverPayroll = await db.Payrolls
        .Where(p => p.Site == input.Site &&
                    p.PeriodMonth == input.PeriodMonth && p.PeriodYear == input.PeriodYear)
        .ToListAsync();
    var driverCost = driverPayroll.Sum(p => p.NetSalary); // Simplified - should link by driver

    // Calculate depreciation
    var depreciation = vehicle?.DepreciationRate > 0
        ? (vehicle.AcquisitionCost ?? 0) * (vehicle.DepreciationRate / 100) / 12
        : 0;

    // Calculate total cost
    var totalCost = fuelCost + maintenanceCost + driverCost + depreciation;
    var totalTrips = trips.Count;
    var totalTon = trips.Sum(t => t.PayloadTon ?? 0);
    var totalKM = trips.Sum(t => t.DistanceKM ?? 0);
    var totalHours = trips.Sum(t => (t.CycleTimeMinutes ?? 0) / 60m);

    var tracking = new UnitCostTracking
    {
        PeriodMonth = input.PeriodMonth,
        PeriodYear = input.PeriodYear,
        Site = input.Site,
        UnitNo = input.UnitNo,
        CostCenter = costCenter,
        Category = vehicle?.Category,
        FuelCost = fuelCost,
        MaintenanceCost = maintenanceCost,
        DriverCost = driverCost,
        DepreciationCost = depreciation ?? 0,
        TyreCost = 0,
        OtherCost = 0,
        TotalCost = totalCost ?? 0,
        TotalTrips = totalTrips,
        TotalTon = totalTon,
        TotalKM = totalKM,
        TotalHours = totalHours,
        CostPerTrip = totalTrips > 0 ? Math.Round((totalCost ?? 0) / totalTrips, 4) : 0m,
        CostPerTon = totalTon > 0 ? Math.Round((totalCost ?? 0) / totalTon, 4) : 0m,
        CostPerKM = totalKM > 0 ? Math.Round((totalCost ?? 0) / totalKM, 4) : 0m,
        CostPerHour = totalHours > 0 ? Math.Round((totalCost ?? 0) / totalHours, 4) : 0m,
        CreatedAt = DateTime.UtcNow
    };

    // Update or create
    var existing = await db.UnitCostTrackings
        .FirstOrDefaultAsync(u => u.PeriodMonth == input.PeriodMonth &&
                                  u.PeriodYear == input.PeriodYear &&
                                  u.Site == input.Site && u.UnitNo == input.UnitNo);
    if (existing != null)
    {
        existing.FuelCost = tracking.FuelCost;
        existing.MaintenanceCost = tracking.MaintenanceCost;
        existing.DriverCost = tracking.DriverCost;
        existing.DepreciationCost = tracking.DepreciationCost;
        existing.TotalCost = tracking.TotalCost;
        existing.TotalTrips = tracking.TotalTrips;
        existing.TotalTon = tracking.TotalTon;
        existing.TotalKM = tracking.TotalKM;
        existing.TotalHours = tracking.TotalHours;
        existing.CostPerTrip = tracking.CostPerTrip;
        existing.CostPerTon = tracking.CostPerTon;
        existing.CostPerKM = tracking.CostPerKM;
        existing.CostPerHour = tracking.CostPerHour;
    }
    else
    {
        db.UnitCostTrackings.Add(tracking);
    }

    await db.SaveChangesAsync();
    return Results.Ok(tracking);
});

// ==================== DRIVER PRODUCTIVITY ====================
app.MapGet("/api/driver-productivity", async (AppDbContext db, string? site, string? month, string? year, string? driverId) =>
{
    var query = db.DriverProductivities.AsQueryable();
    if (!string.IsNullOrEmpty(site) && site != "all") query = query.Where(d => d.Site == site);
    if (!string.IsNullOrEmpty(month)) query = query.Where(d => d.PeriodMonth == month);
    if (!string.IsNullOrEmpty(year)) query = query.Where(d => d.PeriodYear == year);
    if (!string.IsNullOrEmpty(driverId)) query = query.Where(d => d.DriverId == driverId);
    return Results.Ok(await query.OrderByDescending(d => d.PeriodYear).ThenByDescending(d => d.PeriodMonth).ToListAsync());
});

// Auto-calculate driver productivity
app.MapPost("/api/driver-productivity/calculate", async (DriverProductivityInput input, AppDbContext db) =>
{
    var periodStart = new DateTime(int.Parse(input.PeriodYear), int.Parse(input.PeriodMonth), 1);
    var periodEnd = periodStart.AddMonths(1);

    var trips = await db.HaulTrips
        .Where(t => t.Site == input.Site && t.UnitNo == input.UnitNo &&
                    t.TripDate >= periodStart && t.TripDate < periodEnd && t.Status == "COMPLETED" &&
                    (!string.IsNullOrEmpty(input.DriverId) ? t.DriverId == input.DriverId : true))
        .ToListAsync();

    var attendance = await db.Attendances
        .Where(a => a.Site == input.Site &&
                    a.AttendanceDate >= periodStart && a.AttendanceDate < periodEnd &&
                    a.EmployeeCode == input.DriverId)
        .ToListAsync();

    var driver = await db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == input.DriverId);
    var fuelAnalysis = await db.FuelAnalyses
        .FirstOrDefaultAsync(f => f.Site == input.Site && f.UnitNo == input.UnitNo &&
                                  f.PeriodMonth == input.PeriodMonth && f.PeriodYear == input.PeriodYear);

    var totalTrips = trips.Count;
    var targetTrips = input.TargetTrips ?? 20; // Default target: 20 trips/month
    var achievement = targetTrips > 0 ? Math.Round((decimal)totalTrips / targetTrips * 100, 2) : 0;

    // Ritase allowance (per trip incentive)
    var ritaseAllowance = totalTrips * 50000; // Rp 50,000 per ritase

    // Incentive for achievement
    var incentive = achievement >= 100 ? totalTrips * 25000 : achievement >= 80 ? totalTrips * 15000 : 0;

    // Deductions (late, absence)
    var lateCount = attendance.Count(a => a.Status == "LATE");
    var deduction = lateCount * 50000; // Rp 50,000 per late

    var productivity = new DriverProductivity
    {
        PeriodMonth = input.PeriodMonth,
        PeriodYear = input.PeriodYear,
        Site = input.Site,
        DriverId = input.DriverId,
        DriverName = driver?.FullName ?? input.DriverName ?? "",
        UnitNo = input.UnitNo,
        TotalTrips = totalTrips,
        TargetTrips = targetTrips,
        AchievementPercent = achievement,
        TotalTon = trips.Sum(t => t.PayloadTon ?? 0),
        TotalKM = trips.Sum(t => t.DistanceKM ?? 0),
        TotalHours = trips.Sum(t => (t.CycleTimeMinutes ?? 0) / 60),
        TotalFuelLitres = fuelAnalysis?.TotalFuelLitres ?? 0,
        FuelEfficiency = fuelAnalysis?.LitrePerKM ?? 0,
        WorkingDays = attendance.Count(a => a.Status == "PRESENT"),
        LateCount = lateCount,
        AccidentCount = 0,
        ViolationCount = 0,
        RitaseAllowance = ritaseAllowance,
        IncentiveAmount = incentive,
        DeductionAmount = deduction,
        TotalPayable = ritaseAllowance + incentive - deduction,
        CreatedAt = DateTime.UtcNow
    };

    var existing = await db.DriverProductivities
        .FirstOrDefaultAsync(d => d.PeriodMonth == input.PeriodMonth &&
                                  d.PeriodYear == input.PeriodYear &&
                                  d.Site == input.Site && d.DriverId == input.DriverId);
    if (existing != null)
    {
        existing.TotalTrips = productivity.TotalTrips;
        existing.TargetTrips = productivity.TargetTrips;
        existing.AchievementPercent = productivity.AchievementPercent;
        existing.TotalTon = productivity.TotalTon;
        existing.TotalKM = productivity.TotalKM;
        existing.TotalHours = productivity.TotalHours;
        existing.TotalFuelLitres = productivity.TotalFuelLitres;
        existing.FuelEfficiency = productivity.FuelEfficiency;
        existing.WorkingDays = productivity.WorkingDays;
        existing.LateCount = productivity.LateCount;
        existing.RitaseAllowance = productivity.RitaseAllowance;
        existing.IncentiveAmount = productivity.IncentiveAmount;
        existing.DeductionAmount = productivity.DeductionAmount;
        existing.TotalPayable = productivity.TotalPayable;
    }
    else
    {
        db.DriverProductivities.Add(productivity);
    }

    await db.SaveChangesAsync();
    return Results.Ok(productivity);
});

// ==================== AUTO-POST HAULING REVENUE (BATCH) ====================
// Post all unposted trips for a period to GL
app.MapPost("/api/haul-trips/post-revenue-batch", async (AppDbContext db, string site, string month, string year) =>
{
    var periodStart = new DateTime(int.Parse(year), int.Parse(month), 1);
    var periodEnd = periodStart.AddMonths(1);

    var unpostedTrips = await db.HaulTrips
        .Where(t => t.Site == site &&
                    t.TripDate >= periodStart && t.TripDate < periodEnd &&
                    !t.IsRevenuePosted &&
                    t.TripRevenue > 0 && t.Status == "COMPLETED")
        .ToListAsync();

    if (!unpostedTrips.Any()) return Results.Ok(new { message = "No unposted trips found", count = 0 });

    var revenueAccount = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == "4000");
    var arAccount = await db.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == "1101");

    var entryDate = periodEnd.AddDays(-1);
    var entry = new JournalEntry
    {
        EntryNumber = $"JE-HAUL-BATCH-{year}{month}-{site}",
        EntryDate = entryDate,
        PeriodMonth = month,
        PeriodYear = year,
        EntryType = "AUTO",
        SourceModule = "HAUL",
        Description = $"Batch Revenue Posting - {site} - Period {month}/{year} - {unpostedTrips.Count} trips",
        Status = "POSTED",
        TotalDebit = unpostedTrips.Sum(t => t.TripRevenue) ?? 0,
        TotalCredit = unpostedTrips.Sum(t => t.TripRevenue) ?? 0,
        PostedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync();

    // Debit: Piutang
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "1101",
        AccountName = arAccount?.AccountName ?? "Piutang Usaha",
        DC = "D",
        Amount = unpostedTrips.Sum(t => t.TripRevenue ?? 0),
        CostCenter = site,
        Site = site,
        Description = $"Batch AR - {unpostedTrips.Count} trips {month}/{year}",
        CreatedAt = DateTime.UtcNow
    });

    // Credit: Pendapatan
    db.JournalLines.Add(new JournalLine
    {
        JournalEntryId = entry.Id,
        AccountCode = "4000",
        AccountName = revenueAccount?.AccountName ?? "Pendapatan Angkutan",
        DC = "C",
        Amount = unpostedTrips.Sum(t => t.TripRevenue ?? 0),
        CostCenter = site,
        Site = site,
        Description = $"Batch Revenue - {unpostedTrips.Count} trips {month}/{year}",
        CreatedAt = DateTime.UtcNow
    });

    // Mark all trips as posted
    foreach (var trip in unpostedTrips)
    {
        trip.IsRevenuePosted = true;
        trip.Status = "VALIDATED";
    }

    await db.SaveChangesAsync();
    return Results.Ok(new
    {
        message = $"Posted {unpostedTrips.Count} trips to GL",
        entryId = entry.Id,
        totalRevenue = entry.TotalDebit
    });
});

// =====================================================
// P1 MASTER DATA API ENDPOINTS
// =====================================================

// ============ SITE MASTER ============
app.MapGet("/api/sites/master", async (AppDbContext db, string? status, string? siteType) =>
{
    var query = db.Sites.AsQueryable();
    if (!string.IsNullOrEmpty(status))
        query = query.Where(s => s.Status == status);
    if (!string.IsNullOrEmpty(siteType))
        query = query.Where(s => s.SiteType == siteType);
    var sites = await query.OrderBy(s => s.SiteCode).ToListAsync();
    return Results.Ok(sites);
});

app.MapGet("/api/sites/master/{id}", async (int id, AppDbContext db) =>
{
    var site = await db.Sites.FindAsync(id);
    if (site == null) return Results.NotFound(new { error = "Site not found" });
    return Results.Ok(site);
});

app.MapPost("/api/sites/master", async (SiteInput input, AppDbContext db) =>
{
    var existing = await db.Sites.FirstOrDefaultAsync(s => s.SiteCode == input.SiteCode);
    if (existing != null)
        return Results.BadRequest(new { error = "SiteCode already exists" });

    var site = new Site
    {
        SiteCode = input.SiteCode,
        SiteName = input.SiteName,
        Region = input.Region ?? "",
        Address = input.Address ?? "",
        City = input.City ?? "",
        Province = input.Province ?? "",
        SiteType = input.SiteType,
        Currency = input.Currency,
        TimeZone = input.TimeZone,
        Status = "ACTIVE",
        ContactPerson = input.ContactPerson,
        Phone = input.Phone,
        Email = input.Email,
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    db.Sites.Add(site);
    await db.SaveChangesAsync();

    // Audit log
    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "CREATE", Module = "MASTER", RecordId = site.SiteCode, Description = $"Site created: {site.SiteName}" });
    await db.SaveChangesAsync();

    return Results.Created($"/api/sites/master/{site.Id}", site);
});

app.MapPut("/api/sites/master/{id}", async (int id, SiteInput input, AppDbContext db) =>
{
    var site = await db.Sites.FindAsync(id);
    if (site == null) return Results.NotFound(new { error = "Site not found" });

    site.SiteName = input.SiteName;
    site.Region = input.Region ?? site.Region;
    site.Address = input.Address ?? site.Address;
    site.City = input.City ?? site.City;
    site.Province = input.Province ?? site.Province;
    site.SiteType = input.SiteType;
    site.Currency = input.Currency;
    site.TimeZone = input.TimeZone;
    site.ContactPerson = input.ContactPerson;
    site.Phone = input.Phone;
    site.Email = input.Email;
    site.Remarks = input.Remarks;
    site.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "UPDATE", Module = "MASTER", RecordId = site.SiteCode, Description = $"Site updated: {site.SiteName}" });
    await db.SaveChangesAsync();

    return Results.Ok(site);
});

app.MapDelete("/api/sites/master/{id}", async (int id, AppDbContext db) =>
{
    var site = await db.Sites.FindAsync(id);
    if (site == null) return Results.NotFound(new { error = "Site not found" });
    site.Status = "INACTIVE";
    site.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Site deactivated" });
});

// ============ COST CENTER MASTER ============
app.MapGet("/api/cost-centers/master", async (AppDbContext db, string? siteCode, string? status, string? type) =>
{
    var query = db.CostCenters.AsQueryable();
    if (!string.IsNullOrEmpty(siteCode))
        query = query.Where(c => c.SiteCode == siteCode);
    if (!string.IsNullOrEmpty(status))
        query = query.Where(c => c.Status == status);
    if (!string.IsNullOrEmpty(type))
        query = query.Where(c => c.Type == type);
    var cc = await query.OrderBy(c => c.CostCenterCode).ToListAsync();
    return Results.Ok(cc);
});

app.MapGet("/api/cost-centers/master/{id}", async (int id, AppDbContext db) =>
{
    var cc = await db.CostCenters.FindAsync(id);
    if (cc == null) return Results.NotFound(new { error = "Cost Center not found" });
    return Results.Ok(cc);
});

app.MapPost("/api/cost-centers/master", async (CostCenterInput input, AppDbContext db) =>
{
    var existing = await db.CostCenters.FirstOrDefaultAsync(c => c.CostCenterCode == input.CostCenterCode);
    if (existing != null)
        return Results.BadRequest(new { error = "CostCenterCode already exists" });

    var cc = new CostCenter
    {
        CostCenterCode = input.CostCenterCode,
        CostCenterName = input.CostCenterName,
        SiteCode = input.SiteCode ?? "",
        DepartmentCode = input.DepartmentCode,
        Type = input.Type,
        ParentCode = input.ParentCode,
        Level = input.Level,
        AllocatedBudget = input.AllocatedBudget ?? 0,
        UsedBudget = input.UsedBudget ?? 0,
        CommittedBudget = input.CommittedBudget ?? 0,
        AvailableBudget = (input.AllocatedBudget ?? 0) - (input.UsedBudget ?? 0) - (input.CommittedBudget ?? 0),
        Status = "ACTIVE",
        Remarks = input.Remarks,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    db.CostCenters.Add(cc);
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "CREATE", Module = "MASTER", RecordId = cc.CostCenterCode, Description = $"Cost Center created: {cc.CostCenterName}" });
    await db.SaveChangesAsync();

    return Results.Created($"/api/cost-centers/master/{cc.Id}", cc);
});

app.MapPut("/api/cost-centers/master/{id}", async (int id, CostCenterInput input, AppDbContext db) =>
{
    var cc = await db.CostCenters.FindAsync(id);
    if (cc == null) return Results.NotFound(new { error = "Cost Center not found" });

    cc.CostCenterName = input.CostCenterName;
    cc.SiteCode = input.SiteCode ?? cc.SiteCode;
    cc.DepartmentCode = input.DepartmentCode;
    cc.Type = input.Type;
    cc.ParentCode = input.ParentCode;
    cc.Level = input.Level;
    if (input.AllocatedBudget.HasValue) cc.AllocatedBudget = input.AllocatedBudget;
    if (input.UsedBudget.HasValue) cc.UsedBudget = input.UsedBudget;
    if (input.CommittedBudget.HasValue) cc.CommittedBudget = input.CommittedBudget;
    cc.AvailableBudget = cc.AllocatedBudget - cc.UsedBudget - cc.CommittedBudget;
    cc.Remarks = input.Remarks;
    cc.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "UPDATE", Module = "MASTER", RecordId = cc.CostCenterCode, Description = $"Cost Center updated: {cc.CostCenterName}" });
    await db.SaveChangesAsync();

    return Results.Ok(cc);
});

app.MapDelete("/api/cost-centers/master/{id}", async (int id, AppDbContext db) =>
{
    var cc = await db.CostCenters.FindAsync(id);
    if (cc == null) return Results.NotFound(new { error = "Cost Center not found" });
    cc.Status = "INACTIVE";
    cc.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Cost Center deactivated" });
});

// ============ USER MANAGEMENT ============
app.MapGet("/api/users", async (AppDbContext db, string? role, string? siteCode, string? status) =>
{
    var query = db.Users.AsQueryable();
    if (!string.IsNullOrEmpty(role))
        query = query.Where(u => u.Role == role);
    if (!string.IsNullOrEmpty(siteCode))
        query = query.Where(u => u.SiteCode == siteCode);
    if (!string.IsNullOrEmpty(status))
        query = query.Where(u => u.IsActive == (status == "ACTIVE"));
    var users = await query.OrderBy(u => u.Username).ToListAsync();
    // Hide passwords
    var safe = users.Select(u => new { u.Id, u.Username, u.FullName, u.Email, u.Role, u.SiteCode, u.Department, u.Position, u.EmployeeCode, u.IsActive, u.LastLoginAt, u.CreatedAt });
    return Results.Ok(safe);
});

app.MapGet("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var u = await db.Users.FindAsync(id);
    if (u == null) return Results.NotFound(new { error = "User not found" });
    return Results.Ok(new { u.Id, u.Username, u.FullName, u.Email, u.Role, u.SiteCode, u.Department, u.Position, u.EmployeeCode, u.IsActive, u.LastLoginAt, u.CreatedAt });
});

app.MapPost("/api/users", async (UserInput input, AppDbContext db) =>
{
    var existing = await db.Users.FirstOrDefaultAsync(u => u.Username == input.Username);
    if (existing != null)
        return Results.BadRequest(new { error = "Username already exists" });

    // Simple hash (in production use proper bcrypt via BCrypt.Net)
    var hash = BCrypt.Net.BCrypt.HashPassword(input.Password);

    var user = new User
    {
        Username = input.Username,
        PasswordHash = hash,
        FullName = input.FullName,
        Email = input.Email,
        Role = input.Role,
        SiteCode = input.SiteCode,
        Department = input.Department,
        Position = input.Position,
        EmployeeCode = input.EmployeeCode,
        IsActive = input.IsActive,
        CreatedBy = "system",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "CREATE", Module = "MASTER", RecordId = user.Username, Description = $"User created: {user.FullName} ({user.Role})" });
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Username, user.FullName, user.Role, user.IsActive });
});

app.MapPut("/api/users/{id}", async (int id, UserInput input, AppDbContext db) =>
{
    var u = await db.Users.FindAsync(id);
    if (u == null) return Results.NotFound(new { error = "User not found" });

    u.FullName = input.FullName;
    u.Email = input.Email;
    u.Role = input.Role;
    u.SiteCode = input.SiteCode;
    u.Department = input.Department;
    u.Position = input.Position;
    u.EmployeeCode = input.EmployeeCode;
    u.IsActive = input.IsActive;
    if (!string.IsNullOrEmpty(input.Password))
        u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password);
    u.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = "system", Action = "UPDATE", Module = "MASTER", RecordId = u.Username, Description = $"User updated: {u.FullName}" });
    await db.SaveChangesAsync();

    return Results.Ok(new { u.Id, u.Username, u.FullName, u.Role, u.IsActive });
});

app.MapDelete("/api/users/{id}", async (int id, AppDbContext db) =>
{
    var u = await db.Users.FindAsync(id);
    if (u == null) return Results.NotFound(new { error = "User not found" });
    u.IsActive = false;
    u.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "User deactivated" });
});

// Simple Login (no JWT - just session token for now)
app.MapPost("/api/auth/login", async (LoginInput input, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == input.Username && u.IsActive);
    if (user == null)
        return Results.Unauthorized();

    if (!BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
        return Results.Unauthorized();

    // Generate simple token
    var token = Guid.NewGuid().ToString("N");
    var session = new UserSession
    {
        Username = user.Username,
        SessionToken = token,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(1)
    };
    db.UserSessions.Add(session);

    user.LastLoginAt = DateTime.UtcNow;
    user.LastLoginIP = "system";
    user.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    db.AuditLogs.Add(new AuditLog { Username = user.Username, Action = "LOGIN", Module = "AUTH", Description = "User logged in" });
    await db.SaveChangesAsync();

    return Results.Ok(new LoginResponse
    {
        Success = true,
        Token = token,
        Username = user.Username,
        FullName = user.FullName,
        Role = user.Role,
        SiteCode = user.SiteCode,
        Message = "Login successful"
    });
});

// Reset admin password (one-time seed fix)
app.MapPost("/api/auth/seed-admin", async (AppDbContext db) =>
{
    var admin = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
    if (admin == null)
    {
        admin = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "System Administrator",
            Email = "admin@lfn.com",
            Role = "ADMIN",
            IsActive = true,
            CreatedBy = "system"
        };
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Admin user created with password: admin123" });
    }
    else
    {
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        admin.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Admin password reset to: admin123" });
    }
});

app.MapPost("/api/auth/logout", async (LoginInput input, AppDbContext db) =>
{
    var sessions = await db.UserSessions.Where(s => s.Username == input.Username && !s.IsRevoked).ToListAsync();
    foreach (var s in sessions) s.IsRevoked = true;
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Logged out" });
});

// Audit Logs
app.MapGet("/api/audit-logs", async (AppDbContext db, string? username, string? module, string? action, int limit = 100) =>
{
    var query = db.AuditLogs.AsQueryable();
    if (!string.IsNullOrEmpty(username))
        query = query.Where(a => a.Username == username);
    if (!string.IsNullOrEmpty(module))
        query = query.Where(a => a.Module == module);
    if (!string.IsNullOrEmpty(action))
        query = query.Where(a => a.Action == action);
    var logs = await query.OrderByDescending(a => a.CreatedAt).Take(limit).ToListAsync();
    return Results.Ok(logs);
});

// =====================================================
// P2 MASTER DATA API ENDPOINTS
// =====================================================

// ============ DEPARTMENT MASTER ============
app.MapGet("/api/departments/master", async (AppDbContext db, string? status, string? level, string? siteCode) =>
{
    var q = db.Departments.AsQueryable();
    if (!string.IsNullOrEmpty(status)) q = q.Where(d => d.Status == status);
    if (!string.IsNullOrEmpty(level)) q = q.Where(d => d.Level == level);
    if (!string.IsNullOrEmpty(siteCode)) q = q.Where(d => d.SiteCode == siteCode);
    var data = await q.OrderBy(d => d.Level).ThenBy(d => d.DeptCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/departments/master", async (AppDbContext db, DepartmentInput input) =>
{
    var exists = await db.Departments.AnyAsync(d => d.DeptCode == input.DeptCode);
    if (exists) return Results.BadRequest(new { error = "DeptCode already exists" });
    var dept = new Department
    {
        DeptCode = input.DeptCode, DeptName = input.DeptName, ParentCode = input.ParentCode,
        Level = input.Level, CostCenter = input.CostCenter, SiteCode = input.SiteCode,
        HeadName = input.HeadName, HeadTitle = input.HeadTitle,
        Status = "ACTIVE", Remarks = input.Remarks
    };
    db.Departments.Add(dept);
    await db.SaveChangesAsync();
    return Results.Created($"/api/departments/master/{dept.Id}", dept);
});

app.MapGet("/api/departments/master/{id}", async (AppDbContext db, int id) =>
{
    var dept = await db.Departments.FindAsync(id);
    return dept == null ? Results.NotFound() : Results.Ok(dept);
});

app.MapPut("/api/departments/master/{id}", async (AppDbContext db, int id, DepartmentInput input) =>
{
    var dept = await db.Departments.FindAsync(id);
    if (dept == null) return Results.NotFound();
    dept.DeptName = input.DeptName; dept.ParentCode = input.ParentCode; dept.Level = input.Level;
    dept.CostCenter = input.CostCenter; dept.SiteCode = input.SiteCode;
    dept.HeadName = input.HeadName; dept.HeadTitle = input.HeadTitle; dept.Remarks = input.Remarks;
    dept.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(dept);
});

app.MapDelete("/api/departments/master/{id}", async (AppDbContext db, int id) =>
{
    var dept = await db.Departments.FindAsync(id);
    if (dept == null) return Results.NotFound();
    db.Departments.Remove(dept);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ UOM MASTER ============
app.MapGet("/api/uom/master", async (AppDbContext db, string? uomType) =>
{
    var q = db.UnitOfMeasures.AsQueryable();
    if (!string.IsNullOrEmpty(uomType)) q = q.Where(u => u.UomType == uomType);
    var data = await q.OrderBy(u => u.UomType).ThenBy(u => u.UomCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/uom/master", async (AppDbContext db, UnitOfMeasureInput input) =>
{
    var exists = await db.UnitOfMeasures.AnyAsync(u => u.UomCode == input.UomCode);
    if (exists) return Results.BadRequest(new { error = "UomCode already exists" });
    var uom = new UnitOfMeasure
    {
        UomCode = input.UomCode, UomName = input.UomName, UomType = input.UomType,
        BaseUomCode = input.BaseUomCode, ConversionFactor = input.ConversionFactor, Remarks = input.Remarks
    };
    db.UnitOfMeasures.Add(uom);
    await db.SaveChangesAsync();
    return Results.Created($"/api/uom/master/{uom.Id}", uom);
});

app.MapGet("/api/uom/master/{id}", async (AppDbContext db, int id) =>
{
    var uom = await db.UnitOfMeasures.FindAsync(id);
    return uom == null ? Results.NotFound() : Results.Ok(uom);
});

app.MapPut("/api/uom/master/{id}", async (AppDbContext db, int id, UnitOfMeasureInput input) =>
{
    var uom = await db.UnitOfMeasures.FindAsync(id);
    if (uom == null) return Results.NotFound();
    uom.UomName = input.UomName; uom.UomType = input.UomType;
    uom.BaseUomCode = input.BaseUomCode; uom.ConversionFactor = input.ConversionFactor; uom.Remarks = input.Remarks;
    uom.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(uom);
});

app.MapDelete("/api/uom/master/{id}", async (AppDbContext db, int id) =>
{
    var uom = await db.UnitOfMeasures.FindAsync(id);
    if (uom == null) return Results.NotFound();
    db.UnitOfMeasures.Remove(uom);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ MATERIAL MASTER ============
app.MapGet("/api/materials/master", async (AppDbContext db, string? group, string? materialType, string? status) =>
{
    var q = db.Materials.AsQueryable();
    if (!string.IsNullOrEmpty(group)) q = q.Where(m => m.MaterialGroup == group);
    if (!string.IsNullOrEmpty(materialType)) q = q.Where(m => m.MaterialType == materialType);
    if (!string.IsNullOrEmpty(status)) q = q.Where(m => m.Status == status);
    var data = await q.OrderBy(m => m.MaterialGroup).ThenBy(m => m.MaterialCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/materials/master", async (AppDbContext db, MaterialInput input) =>
{
    var exists = await db.Materials.AnyAsync(m => m.MaterialCode == input.MaterialCode);
    if (exists) return Results.BadRequest(new { error = "MaterialCode already exists" });
    var mat = new Material
    {
        MaterialCode = input.MaterialCode, MaterialName = input.MaterialName,
        MaterialGroup = input.MaterialGroup, MaterialType = input.MaterialType,
        UomCode = input.UomCode, Brand = input.Brand, Spec = input.Spec,
        UnitPrice = input.UnitPrice, Currency = input.Currency ?? "IDR",
        SiteCode = input.SiteCode, Remarks = input.Remarks
    };
    db.Materials.Add(mat);
    await db.SaveChangesAsync();
    return Results.Created($"/api/materials/master/{mat.Id}", mat);
});

app.MapGet("/api/materials/master/{id}", async (AppDbContext db, int id) =>
{
    var mat = await db.Materials.FindAsync(id);
    return mat == null ? Results.NotFound() : Results.Ok(mat);
});

app.MapPut("/api/materials/master/{id}", async (AppDbContext db, int id, MaterialInput input) =>
{
    var mat = await db.Materials.FindAsync(id);
    if (mat == null) return Results.NotFound();
    mat.MaterialName = input.MaterialName; mat.MaterialGroup = input.MaterialGroup;
    mat.MaterialType = input.MaterialType; mat.UomCode = input.UomCode;
    mat.Brand = input.Brand; mat.Spec = input.Spec;
    mat.UnitPrice = input.UnitPrice; mat.Currency = input.Currency ?? "IDR";
    mat.SiteCode = input.SiteCode; mat.Remarks = input.Remarks;
    mat.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(mat);
});

app.MapDelete("/api/materials/master/{id}", async (AppDbContext db, int id) =>
{
    var mat = await db.Materials.FindAsync(id);
    if (mat == null) return Results.NotFound();
    db.Materials.Remove(mat);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ APPROVAL WORKFLOW ============
app.MapGet("/api/workflows/master", async (AppDbContext db, string? moduleType, string? status) =>
{
    var q = db.ApprovalWorkflows.AsQueryable();
    if (!string.IsNullOrEmpty(moduleType)) q = q.Where(w => w.ModuleType == moduleType);
    if (!string.IsNullOrEmpty(status)) q = q.Where(w => w.Status == status);
    var data = await q.OrderBy(w => w.ModuleType).ThenBy(w => w.ApprovalOrder).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/workflows/master", async (AppDbContext db, ApprovalWorkflowInput input) =>
{
    var wf = new ApprovalWorkflow
    {
        WorkflowName = input.WorkflowName, ModuleType = input.ModuleType,
        SiteCode = input.SiteCode, CostCenter = input.CostCenter,
        ApprovalOrder = input.ApprovalOrder, ApproverRole = input.ApproverRole,
        ApproverName = input.ApproverName, ApprovalLevel = input.ApprovalLevel,
        MinAmount = input.MinAmount, MaxAmount = input.MaxAmount
    };
    db.ApprovalWorkflows.Add(wf);
    await db.SaveChangesAsync();
    return Results.Created($"/api/workflows/master/{wf.Id}", wf);
});

app.MapGet("/api/workflows/master/{id}", async (AppDbContext db, int id) =>
{
    var wf = await db.ApprovalWorkflows.FindAsync(id);
    return wf == null ? Results.NotFound() : Results.Ok(wf);
});

app.MapPut("/api/workflows/master/{id}", async (AppDbContext db, int id, ApprovalWorkflowInput input) =>
{
    var wf = await db.ApprovalWorkflows.FindAsync(id);
    if (wf == null) return Results.NotFound();
    wf.WorkflowName = input.WorkflowName; wf.ModuleType = input.ModuleType;
    wf.SiteCode = input.SiteCode; wf.CostCenter = input.CostCenter;
    wf.ApprovalOrder = input.ApprovalOrder; wf.ApproverRole = input.ApproverRole;
    wf.ApproverName = input.ApproverName; wf.ApprovalLevel = input.ApprovalLevel;
    wf.MinAmount = input.MinAmount; wf.MaxAmount = input.MaxAmount;
    wf.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(wf);
});

app.MapDelete("/api/workflows/master/{id}", async (AppDbContext db, int id) =>
{
    var wf = await db.ApprovalWorkflows.FindAsync(id);
    if (wf == null) return Results.NotFound();
    db.ApprovalWorkflows.Remove(wf);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// =====================================================
// P3 MASTER DATA API ENDPOINTS
// =====================================================

// ============ VEHICLE MASTER ============
app.MapGet("/api/vehicles/master", async (AppDbContext db, string? status, string? vehicleType, string? siteCode) =>
{
    var q = db.Vehicles.AsQueryable();
    if (!string.IsNullOrEmpty(status)) q = q.Where(v => v.Status == status);
    if (!string.IsNullOrEmpty(vehicleType)) q = q.Where(v => v.VehicleType == vehicleType);
    if (!string.IsNullOrEmpty(siteCode)) q = q.Where(v => v.SiteCode == siteCode);
    var data = await q.OrderBy(v => v.VehicleCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/vehicles/master", async (AppDbContext db, VehicleInput input) =>
{
    var exists = await db.Vehicles.AnyAsync(v => v.VehicleCode == input.VehicleCode);
    if (exists) return Results.BadRequest(new { error = "VehicleCode already exists" });
    var v = new Vehicle
    {
        VehicleCode = input.VehicleCode, PoliceNumber = input.PoliceNumber, VehicleType = input.VehicleType,
        Brand = input.Brand, Model = input.Model, FuelType = input.FuelType, SiteCode = input.SiteCode,
        CostCenter = input.CostCenter, CapacityVolume = input.CapacityVolume, CapacityWeight = input.CapacityWeight,
        YearMade = input.YearMade, ChassisNumber = input.ChassisNumber, MachineNumber = input.MachineNumber,
        Remarks = input.Remarks
    };
    db.Vehicles.Add(v);
    await db.SaveChangesAsync();
    return Results.Created($"/api/vehicles/master/{v.Id}", v);
});

app.MapGet("/api/vehicles/master/{id}", async (AppDbContext db, int id) =>
{
    var v = await db.Vehicles.FindAsync(id);
    return v == null ? Results.NotFound() : Results.Ok(v);
});

app.MapPut("/api/vehicles/master/{id}", async (AppDbContext db, int id, VehicleInput input) =>
{
    var v = await db.Vehicles.FindAsync(id);
    if (v == null) return Results.NotFound();
    v.PoliceNumber = input.PoliceNumber; v.VehicleType = input.VehicleType;
    v.Brand = input.Brand; v.Model = input.Model; v.FuelType = input.FuelType;
    v.SiteCode = input.SiteCode; v.CostCenter = input.CostCenter;
    v.CapacityVolume = input.CapacityVolume; v.CapacityWeight = input.CapacityWeight;
    v.YearMade = input.YearMade; v.ChassisNumber = input.ChassisNumber; v.MachineNumber = input.MachineNumber;
    v.Remarks = input.Remarks; v.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(v);
});

app.MapDelete("/api/vehicles/master/{id}", async (AppDbContext db, int id) =>
{
    var v = await db.Vehicles.FindAsync(id);
    if (v == null) return Results.NotFound();
    db.Vehicles.Remove(v);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ DRIVER MASTER ============
app.MapGet("/api/drivers/master", async (AppDbContext db, string? status, string? siteCode, string? departmentCode) =>
{
    var q = db.Drivers.AsQueryable();
    if (!string.IsNullOrEmpty(status)) q = q.Where(d => d.Status == status);
    if (!string.IsNullOrEmpty(siteCode)) q = q.Where(d => d.SiteCode == siteCode);
    if (!string.IsNullOrEmpty(departmentCode)) q = q.Where(d => d.DepartmentCode == departmentCode);
    var data = await q.OrderBy(d => d.DriverCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/drivers/master", async (AppDbContext db, DriverInput input) =>
{
    var exists = await db.Drivers.AnyAsync(d => d.DriverCode == input.DriverCode);
    if (exists) return Results.BadRequest(new { error = "DriverCode already exists" });
    var d = new Driver
    {
        DriverCode = input.DriverCode, FullName = input.FullName, NIK = input.NIK,
        SIM = input.SIM, SIMType = input.SIMType, DateOfBirth = input.DateOfBirth,
        Gender = input.Gender, Phone = input.Phone, Address = input.Address,
        SiteCode = input.SiteCode, DepartmentCode = input.DepartmentCode,
        JoinDate = input.JoinDate, Remarks = input.Remarks
    };
    db.Drivers.Add(d);
    await db.SaveChangesAsync();
    return Results.Created($"/api/drivers/master/{d.Id}", d);
});

app.MapGet("/api/drivers/master/{id}", async (AppDbContext db, int id) =>
{
    var d = await db.Drivers.FindAsync(id);
    return d == null ? Results.NotFound() : Results.Ok(d);
});

app.MapPut("/api/drivers/master/{id}", async (AppDbContext db, int id, DriverInput input) =>
{
    var d = await db.Drivers.FindAsync(id);
    if (d == null) return Results.NotFound();
    d.FullName = input.FullName; d.NIK = input.NIK; d.SIM = input.SIM;
    d.SIMType = input.SIMType; d.DateOfBirth = input.DateOfBirth;
    d.Gender = input.Gender; d.Phone = input.Phone; d.Address = input.Address;
    d.SiteCode = input.SiteCode; d.DepartmentCode = input.DepartmentCode;
    d.JoinDate = input.JoinDate; d.Remarks = input.Remarks; d.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(d);
});

app.MapDelete("/api/drivers/master/{id}", async (AppDbContext db, int id) =>
{
    var d = await db.Drivers.FindAsync(id);
    if (d == null) return Results.NotFound();
    db.Drivers.Remove(d);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ ROUTE MASTER ============
app.MapGet("/api/routes/master", async (AppDbContext db, string? status, string? routeType, string? siteCode) =>
{
    var q = db.RouteMasters.AsQueryable();
    if (!string.IsNullOrEmpty(status)) q = q.Where(r => r.Status == status);
    if (!string.IsNullOrEmpty(routeType)) q = q.Where(r => r.RouteType == routeType);
    if (!string.IsNullOrEmpty(siteCode)) q = q.Where(r => r.SiteCode == siteCode);
    var data = await q.OrderBy(r => r.RouteCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/routes/master", async (AppDbContext db, RouteInput input) =>
{
    var exists = await db.RouteMasters.AnyAsync(r => r.RouteCode == input.RouteCode);
    if (exists) return Results.BadRequest(new { error = "RouteCode already exists" });
    var r = new RouteMaster
    {
        RouteCode = input.RouteCode, RouteName = input.RouteName, SiteCode = input.SiteCode,
        OriginLocation = input.OriginLocation, DestinationLocation = input.DestinationLocation,
        DistanceKM = input.DistanceKm, TravelTimeMin = input.TravelTimeMin,
        HaulCostPerKm = input.HaulCostPerKm, FuelConsumptionPerKm = input.FuelConsumptionPerKm,
        RouteType = input.RouteType, Remarks = input.Remarks
    };
    db.RouteMasters.Add(r);
    await db.SaveChangesAsync();
    return Results.Created($"/api/routes/master/{r.Id}", r);
});

app.MapGet("/api/routes/master/{id}", async (AppDbContext db, int id) =>
{
    var r = await db.RouteMasters.FindAsync(id);
    return r == null ? Results.NotFound() : Results.Ok(r);
});

app.MapPut("/api/routes/master/{id}", async (AppDbContext db, int id, RouteInput input) =>
{
    var r = await db.RouteMasters.FindAsync(id);
    if (r == null) return Results.NotFound();
    r.RouteName = input.RouteName; r.SiteCode = input.SiteCode;
    r.OriginLocation = input.OriginLocation; r.DestinationLocation = input.DestinationLocation;
    r.DistanceKM = input.DistanceKm; r.TravelTimeMin = input.TravelTimeMin;
    r.HaulCostPerKm = input.HaulCostPerKm; r.FuelConsumptionPerKm = input.FuelConsumptionPerKm;
    r.RouteType = input.RouteType;
    r.Remarks = input.Remarks; r.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(r);
});

app.MapDelete("/api/routes/master/{id}", async (AppDbContext db, int id) =>
{
    var r = await db.RouteMasters.FindAsync(id);
    if (r == null) return Results.NotFound();
    db.RouteMasters.Remove(r);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// =====================================================
// P4 MASTER DATA API ENDPOINTS
// =====================================================

// ============ TAX MASTER ============
app.MapGet("/api/taxes/master", async (AppDbContext db, string? taxType, string? status) =>
{
    var q = db.Taxes.AsQueryable();
    if (!string.IsNullOrEmpty(taxType)) q = q.Where(t => t.TaxType == taxType);
    if (!string.IsNullOrEmpty(status)) q = q.Where(t => t.Status == status);
    var data = await q.OrderBy(t => t.TaxCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/taxes/master", async (AppDbContext db, TaxInput input) =>
{
    var exists = await db.Taxes.AnyAsync(t => t.TaxCode == input.TaxCode);
    if (exists) return Results.BadRequest(new { error = "TaxCode already exists" });
    var t = new Tax
    {
        TaxCode = input.TaxCode, TaxName = input.TaxName, TaxType = input.TaxType,
        TaxRate = input.TaxRate, TaxRateType = input.TaxRateType,
        FixedAmount = input.FixedAmount, CoaCode = input.CoaCode,
        TaxBase = input.TaxBase, ApplicableTo = input.ApplicableTo, Remarks = input.Remarks
    };
    db.Taxes.Add(t);
    await db.SaveChangesAsync();
    return Results.Created($"/api/taxes/master/{t.Id}", t);
});

app.MapGet("/api/taxes/master/{id}", async (AppDbContext db, int id) =>
{
    var t = await db.Taxes.FindAsync(id);
    return t == null ? Results.NotFound() : Results.Ok(t);
});

app.MapPut("/api/taxes/master/{id}", async (AppDbContext db, int id, TaxInput input) =>
{
    var t = await db.Taxes.FindAsync(id);
    if (t == null) return Results.NotFound();
    t.TaxName = input.TaxName; t.TaxType = input.TaxType;
    t.TaxRate = input.TaxRate; t.TaxRateType = input.TaxRateType;
    t.FixedAmount = input.FixedAmount; t.CoaCode = input.CoaCode;
    t.TaxBase = input.TaxBase; t.ApplicableTo = input.ApplicableTo;
    t.Remarks = input.Remarks; t.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(t);
});

app.MapDelete("/api/taxes/master/{id}", async (AppDbContext db, int id) =>
{
    var t = await db.Taxes.FindAsync(id);
    if (t == null) return Results.NotFound();
    db.Taxes.Remove(t);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

// ============ COA MASTER ============
app.MapGet("/api/coa/master", async (AppDbContext db, string? accountType, string? accountCategory, string? status) =>
{
    var q = db.COAs.AsQueryable();
    if (!string.IsNullOrEmpty(accountType)) q = q.Where(c => c.AccountType == accountType);
    if (!string.IsNullOrEmpty(accountCategory)) q = q.Where(c => c.AccountCategory == accountCategory);
    if (!string.IsNullOrEmpty(status)) q = q.Where(c => c.Status == status);
    var data = await q.OrderBy(c => c.AccountCode).ToListAsync();
    return Results.Ok(data);
});

app.MapPost("/api/coa/master", async (AppDbContext db, COAInput input) =>
{
    var exists = await db.COAs.AnyAsync(c => c.AccountCode == input.AccountCode);
    if (exists) return Results.BadRequest(new { error = "AccountCode already exists" });
    var c = new COA
    {
        AccountCode = input.AccountCode, AccountName = input.AccountName,
        AccountType = input.AccountType, AccountCategory = input.AccountCategory,
        AccountLevel = input.AccountLevel, ParentAccountCode = input.ParentAccountCode,
        NormalBalance = input.NormalBalance, CostCenterRequired = input.CostCenterRequired,
        SiteCode = input.SiteCode, TaxCode = input.TaxCode,
        Currency = input.Currency ?? "IDR", Description = input.Description
    };
    db.COAs.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/api/coa/master/{c.Id}", c);
});

app.MapGet("/api/coa/master/{id}", async (AppDbContext db, int id) =>
{
    var c = await db.COAs.FindAsync(id);
    return c == null ? Results.NotFound() : Results.Ok(c);
});

app.MapPut("/api/coa/master/{id}", async (AppDbContext db, int id, COAInput input) =>
{
    var c = await db.COAs.FindAsync(id);
    if (c == null) return Results.NotFound();
    c.AccountName = input.AccountName; c.AccountType = input.AccountType;
    c.AccountCategory = input.AccountCategory; c.AccountLevel = input.AccountLevel;
    c.ParentAccountCode = input.ParentAccountCode; c.NormalBalance = input.NormalBalance;
    c.CostCenterRequired = input.CostCenterRequired; c.SiteCode = input.SiteCode;
    c.TaxCode = input.TaxCode; c.Currency = input.Currency ?? "IDR";
    c.Description = input.Description; c.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(c);
});

app.MapDelete("/api/coa/master/{id}", async (AppDbContext db, int id) =>
{
    var c = await db.COAs.FindAsync(id);
    if (c == null) return Results.NotFound();
    db.COAs.Remove(c);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Deleted" });
});

app.Run();
