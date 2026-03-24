# 🔒 Validasi Duplikasi Data - LFN  App

## 📋 Overview

Sistem sekarang memiliki validasi duplikasi otomatis untuk mencegah data ganda saat upload CSV. Hanya data baru yang akan di-insert ke database.

---

## 🔑 Keys untuk Validasi Duplikasi

### 1. Fuel Receipt (Penerimaan BBM)

**Unique Key:** `no_tiket` (Nomor Tiket)

✅ Setiap nomor tiket harus unik
✅ Jika no_tiket sudah ada, data akan di-skip

### 2. Fuel Usage (Penggunaan BBM)

**Unique Key:** Kombinasi `site + tanggal + unit_no`

✅ Satu unit tidak boleh ada 2 record di site yang sama pada hari yang sama
✅ Jika kombinasi sudah ada, data akan di-skip

---

## 🎯 Cara Kerja Validasi

### Flow Upload:

```
1. Upload CSV
   ↓
2. Parse File
   ↓
3. Untuk setiap baris:
   - Cek apakah data sudah ada berdasarkan Unique Key
   - Jika sudah ada → SKIP
   - Jika belum ada → INSERT
   ↓
4. Return Summary:
   - Total Rows
   - Inserted (data baru)
   - Skipped (data duplikat)
   - Errors (data invalid)
```

---

## 📊 Response Format

Contoh response saat upload berhasil:

```json
{
  "message": "File data.csv processed successfully",
  "site": "Sungai Dua",
  "datatype": "receipt",
  "totalRows": 10,
  "inserted": 7,
  "skipped": 3,
  "errors": 0,
  "skippedRecords": [
    "Receipt with NoTiket 'TKT001' already exists",
    "Receipt with NoTiket 'TKT005' already exists",
    "Receipt with NoTiket 'TKT008' already exists"
  ],
  "warning": "Some records were skipped because they already exist in the database."
}
```

---

## 🔍 Contoh Skenario

### Skenario 1: Upload Data Baru

**CSV Data:**
```csv
No;Tanggal;Site;Supplier;...;NoTiket;...
1;2025-01-15;Sungai Dua;Supplier ABC;...;TKT100;...
2;2025-01-15;Sungai Dua;Supplier XYZ;...;TKT101;...
```

**Database:** Kosong
**Result:**
-  Inserted: 2
- ⊘ Skipped: 0

---

### Skenario 2: Upload Data Duplikat

**CSV Data:**
```csv
No;Tanggal;Site;Supplier;...;NoTiket;...
1;2025-01-15;Sungai Dua;Supplier ABC;...;TKT001;...
2;2025-01-15;Sungai Dua;Supplier XYZ;...;TKT002;...
```

**Database:** TKT001 sudah ada
**Result:**
-  Inserted: 1 (TKT002)
- ⊘ Skipped: 1 (TKT001)

---

### Skenario 3: Upload Campuran (Baru + Duplikat)

**CSV Data:**
```csv
No;Tanggal;Site;Supplier;...;NoTiket;...
1;2025-01-15;Sungai Dua;Supplier ABC;...;TKT001;...  ← Duplikat
2;2025-01-15;Sungai Dua;Supplier XYZ;...;TKT002;...  ← Duplikat
3;2025-01-15;Sungai Dua;Supplier DEF;...;TKT100;...  ← Baru
4;2025-01-15;Sungai Dua;Supplier GHI;...;TKT101;...  ← Baru
```

**Database:** TKT001, TKT002 sudah ada
**Result:**
-  Inserted: 2 (TKT100, TKT101)
- ⊘ Skipped: 2 (TKT001, TKT002)

---

## 🗄️ Database Constraints

### PostgreSQL:
```sql
-- Fuel Receipts
CONSTRAINT uq_fuel_receipts_tiket UNIQUE (no_tiket)

-- Fuel Usages
CONSTRAINT uq_fuel_usages_site_tanggal_unit UNIQUE (site, tanggal, unit_no)
```

### MySQL:
```sql
-- Fuel Receipts
UNIQUE KEY uq_fuel_receipts_tiket (no_tiket)

-- Fuel Usages
UNIQUE KEY uq_fuel_usages_site_tanggal_unit (site, tanggal, unit_no)
```

---

## 🎨 Tampilan Frontend

Saat upload dengan duplikat:

```
┌─────────────────────────────────────────┐
│ Upload Completed with Warnings          │
├─────────────────────────────────────────┤
│ Total Rows: 10                          │
│ ✓ Inserted: 7                          │
│ ⊘ Skipped: 3 (already exists)          │
│                                         │
│ Skipped Records:                        │
│ • Receipt with NoTiket 'TKT001'...    │
│ • Receipt with NoTiket 'TKT005'...    │
│ • Receipt with NoTiket 'TKT008'...    │
│                                         │
│            [OK]                         │
└─────────────────────────────────────────┘
```

---

## ⚠️ Error Handling

### Jika terjadi error:
- Error tidak akan menghentikan proses upload
- Record bermasalah akan di-skip
- Error count akan ditampilkan di summary
- Detail error di-log di console (server side)

### Contoh Error:
```json
{
  "totalRows": 10,
  "inserted": 8,
  "skipped": 1,
  "errors": 1,
  "warning": "Some records were skipped..."
}
```

---

## 🔧 Customization

### Mengubah Unique Key:

#### 1. Di Database Schema
Edit `Scripts/schema_postgresql.sql` atau `schema_mysql.sql`:

```sql
-- Contoh: Ganti ke unique key berdasarkan tanggal saja
CONSTRAINT uq_fuel_receipts_tanggal UNIQUE (tanggal)
```

#### 2. Di Program.cs
Edit validasi di endpoint `/api/upload`:

```csharp
// Ganti logic cek duplikasi
var existingReceipt = await db.FuelReceipts
    .FirstOrDefaultAsync(r => r.Tanggal == tanggal);
```

#### 3. Di Model
Edit `Data/AppDbContext.cs` jika perlu

---

## 💡 Best Practices

### 1. Upload Berkala
- Upload data harian secara rutin
- Jangan upload ulang data yang sudah ada
- Sistem akan otomatis skip duplikat

### 2. Cek Summary
Selalu periksa summary setelah upload:
- **Inserted** - Data baru berhasil ditambahkan
- **Skipped** - Data sudah ada, tidak perlu khawatir
- **Errors** - Perlu diperbaiki datanya

### 3. Handle Skipped Data
Jika banyak data yang di-skip:
- Cek apakah memang duplikat
- Atau mungkin perlu update data yang sudah ada (bukan insert baru)

---

## 🚀 Next Steps

Untuk fitur tambahan:
1. **Update Existing Data** - Jika ingin update data duplikat alih-alih skip
2. **Soft Delete** - Tandai data sebagai dihapus alih-alih menghapus
3. **Versioning** - Track perubahan data (history)
4. **Bulk Operations** - Upload dan update dalam batch besar

---

Dibuat untuk LFN  App 🚛⛽
