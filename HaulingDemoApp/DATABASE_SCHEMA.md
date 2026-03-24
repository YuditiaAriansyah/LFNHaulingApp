# 📊 Database Schema - LFN  App

## 🗄️ Struktur Database

Aplikasi ini menggunakan **2 tabel utama**:

### 1. Tabel: `fuel_receipts` (Fuel Receipt/Penerimaan BBM)

Tabel untuk menyimpan data penerimaan fuel/solar dari supplier.

#### Kolom-kolom:

| Kolom | Tipe Data | Keterangan | Wajib |
|-------|-----------|------------|-------|
| `id` | INT / SERIAL | Primary Key, Auto Increment | ✅ |
| `no` | INTEGER | Nomor urut transaksi | ✅ |
| `tanggal` | DATE | Tanggal transaksi | ✅ |
| `site` | VARCHAR(100) | Nama site/lokasi | ✅ |
| `supplier` | VARCHAR(200) | Nama supplier BBM | ✅ |
| `liter` | DECIMAL(18,2) | Jumlah liter yang diterima | ✅ |
| `jenis_bbm` | VARCHAR(50) | Jenis BBM (Solar, Premium, dll) | ✅ |
| `harga_per_liter` | DECIMAL(18,2) | Harga per liter | ✅ |
| `total_harga` | DECIMAL(18,2) | Total harga (liter × harga) | ✅ |
| `no_tiket` | VARCHAR(50) | Nomor tiket/receipt | ✅ |
| `start_time` | TIME | Waktu mulai pengisian | ✅ |
| `end_time` | TIME | Waktu selesai pengisian | ✅ |
| `keterangan` | VARCHAR(500) | Keterangan tambahan | ❌ |
| `created_at` | TIMESTAMP | Waktu record dibuat | ✅ Auto |
| `updated_at` | TIMESTAMP | Waktu record terupdate | ✅ Auto |

#### Index:
- `idx_tanggal` - untuk filter berdasarkan tanggal
- `idx_site` - untuk filter berdasarkan site
- `idx_site_tanggal` - untuk filter kombinasi site dan tanggal

---

### 2. Tabel: `fuel_usages` (Fuel Usage/Penggunaan BBM)

Tabel untuk menyimpan data penggunaan fuel/solar oleh unit/operational.

#### Kolom-kolom:

| Kolom | Tipe Data | Keterangan | Wajib |
|-------|-----------|------------|-------|
| `id` | INT / SERIAL | Primary Key, Auto Increment | ✅ |
| `no` | INTEGER | Nomor urut transaksi | ✅ |
| `tanggal` | DATE | Tanggal transaksi | ✅ |
| `site` | VARCHAR(100) | Nama site/lokasi | ✅ |
| `unit_no` | VARCHAR(50) | Nomor unit/alat berat | ✅ |
| `operator_name` | VARCHAR(100) | Nama operator | ✅ |
| `liter_awal` | DECIMAL(18,2) | Liter awal (before use) | ✅ |
| `liter_akhir` | DECIMAL(18,2) | Liter akhir (after use) | ✅ |
| `pemakaian` | DECIMAL(18,2) | Jumlah pemakaian | ✅ |
| `jam_kerja` | DECIMAL(18,2) | Jam kerja (hours) | ✅ |
| `efisiensi` | DECIMAL(18,2) | Efisiensi (liter/hour) | ✅ |
| `keterangan` | VARCHAR(500) | Keterangan tambahan | ❌ |
| `created_at` | TIMESTAMP | Waktu record dibuat | ✅ Auto |
| `updated_at` | TIMESTAMP | Waktu record terupdate | ✅ Auto |

#### Index:
- `idx_tanggal` - untuk filter berdasarkan tanggal
- `idx_site` - untuk filter berdasarkan site
- `idx_site_tanggal` - untuk filter kombinasi site dan tanggal

---

## 🔄 Flow Data

### Upload Flow:
```
CSV File → Upload Endpoint → Parse CSV → Insert to Database
```

### Download Flow:
```
Database → Query by Site & Date → Generate CSV → Download File
```

---

## ✅ Cek Kesesuaian Database Anda

Jika Anda sudah membuat database di GCP, pastikan:

### 1. Nama Database
- ✅ Database: `lfn_hauling_db`

### 2. Nama Tabel
- ✅ Tabel 1: `fuel_receipts`
- ✅ Tabel 2: `fuel_usages`

### 3. Struktur Tabel Fuel Receipts
Cek dengan query:
```sql
-- PostgreSQL
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'fuel_receipts'
ORDER BY ordinal_position;

-- MySQL
DESCRIBE fuel_receipts;
```

Pastikan kolom-kolom berikut ada:
- `id` (primary key, auto increment)
- `no`
- `tanggal`
- `site`
- `supplier`
- `liter`
- `jenis_bbm`
- `harga_per_liter`
- `total_harga`
- `no_tiket`
- `start_time`
- `end_time`
- `keterangan`
- `created_at`
- `updated_at`

### 4. Struktur Tabel Fuel Usages
Cek dengan query:
```sql
-- PostgreSQL
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'fuel_usages'
ORDER BY ordinal_position;

-- MySQL
DESCRIBE fuel_usages;
```

Pastikan kolom-kolom berikut ada:
- `id` (primary key, auto increment)
- `no`
- `tanggal`
- `site`
- `unit_no`
- `operator_name`
- `liter_awal`
- `liter_akhir`
- `pemakaian`
- `jam_kerja`
- `efisiensi`
- `keterangan`
- `created_at`
- `updated_at`

---

## 🔧 Jika Database Belum Sesuai

### Opsi 1: Jalankan Skema Database
Gunakan file skema yang sudah disediakan:
- PostgreSQL: `Scripts/schema_postgresql.sql`
- MySQL: `Scripts/schema_mysql.sql`

### Opsi 2: Mapping Custom Table
Jika Anda sudah punya tabel dengan nama/struktur berbeda:

1. **Edit Model Classes** di `Models/FuelReceipt.cs` dan `Models/FuelUsage.cs`
2. **Edit DbContext** di `Data/AppDbContext.cs`
3. Sesuaikan nama tabel dan kolom

Contoh mapping tabel berbeda:
```csharp
// Di AppDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<FuelReceipt>(entity =>
    {
        entity.ToTable("nama_tabel_anda"); // Ganti nama tabel
        entity.Property(e => e.Supplier).HasColumnName("nama_kolom_supplier"); // Ganti nama kolom
    });
}
```

---

## 📝 Sample Data

### Fuel Receipts Sample:
```csv
No;Tanggal;Site;Supplier;Liter;JenisBBM;HargaPerLiter;TotalHarga;NoTiket;Start Time;End Time;Keterangan
1;2025-01-15;Sungai Dua;PT.Supplier ABC;5000;Solar;15000;75000000;TKT001;08:00;09:30;Penerimaan rutin
2;2025-01-15;Sungai Dua;PT.Supplier XYZ;3000;Solar;15000;45000000;TKT002;10:00;10:45;Penerimaan tambahan
```

### Fuel Usages Sample:
```csv
No;Tanggal;Site;UnitNo;Operator;LiterAwal;LiterAkhir;Pemakaian;JamKerja;EFisiensi;Keterangan
1;2025-01-15;Sungai Dua;DT-001;John Doe;1000;1500;500;8;62.5;Penggunaan normal
2;2025-01-15;Sungai Dua;DT-002;Jane Smith;2000;2300;300;6;50.0;Penggunaan siang
```

---

## 🚀 Next Steps

Setelah database sesuai:
1. Configure connection string di `appsettings.json`
2. Test koneksi dengan endpoint `/health`
3. Test upload data via form
4. Test download data via tombol download

Lihat panduan lengkap di: **[GCP_DATABASE_SETUP.md](./GCP_DATABASE_SETUP.md)**

---

Dibuat untuk LFN  App 🚛⛽
