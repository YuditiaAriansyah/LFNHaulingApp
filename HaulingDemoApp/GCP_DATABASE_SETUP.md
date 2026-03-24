# 🗄️ GCP Database Setup Guide - LFN  App

## 📋 Daftar Isi
1. [Persiapan di GCP Console](#persiapan-di-gcp-console)
2. [Membuat Database di Cloud SQL](#membuat_database_di_cloud_sql)
3. [Menjalankan Skema Database](#menjalankan_skema_database)
4. [Konfigurasi Koneksi di Aplikasi](#konfigurasi_koneksi_di_aplikasi)
5. [Testing Koneksi](#testing_koneksi)
6. [Troubleshooting](#troubleshooting)

---

## 🌩️ Persiapan di GCP Console

### 1. Buat Project di GCP
Jika belum ada:
1. Buka [GCP Console](https://console.cloud.google.com)
2. Klik "Select a project" → "New Project"
3. Beri nama project: `LFN-Hauling-App`
4. Klik "Create"

### 2. Enable Cloud SQL API
1. Buka [APIs & Services](https://console.cloud.google.com/apis/dashboard)
2. Klik "+ Enable APIs and Services"
3. Cari "Cloud SQL API"
4. Klik "Enable"

---

## 🏗️ Membuat Database di Cloud SQL

### Opsi A: Cloud SQL PostgreSQL (Recommended)

#### 1. Buat Instance PostgreSQL
1. Buka [Cloud SQL](https://console.cloud.google.com/sql/instances)
2. Klik "Create Instance" → "Choose PostgreSQL"
3. Konfigurasi:
   - **Instance ID**: `lfn-hauling-db`
   - **Password**: Buat password yang kuat
   - **Database version**: PostgreSQL 15 (atau latest)
   - **Region**: pilih region terdekat (misal: `asia-southeast1` - Singapore)
   - **Zone**: (Optional) pilih specific zone
4. Tab "Configuration":
   - **Machine type**: pilih sesuai budget (Shared core untuk development)
   - **Storage**: 10GB SSD untuk awal
5. Klik "Create Instance"

#### 2. Setup Koneksi
1. Setelah instance aktif, klik instance name
2. Tab "Connections"
3. Scroll ke "Authorized networks"
4. Klik "Add network"
5. Untuk development:
   - **Name**: `Development IP`
   - **Network**: IP publik komputer Anda (cari di Google "what is my ip")
   - Atau gunakan `0.0.0.0/0` untuk testing (TIDAK RECOMMENDED untuk production)

#### 3. Buat Database
1. Tab "Databases"
2. Klik "Create database"
3. **Database name**: `lfn_hauling_db`
4. Klik "Create"

#### 4. Dapatkan Connection String
1. Tab "Overview"
2. Copy "Connection name" dengan format:
   ```
   <PROJECT_ID>:<REGION>:<INSTANCE_ID>
   Contoh: lfn-hauling-app:asia-southeast1:lfn-hauling-db
   ```
3. Untuk koneksi dari aplikasi, gunakan format:
   ```
   Host=<INSTANCE_PUBLIC_IP>;Port=5432;Database=lfn_hauling_db;Username=postgres;Password=<YOUR_PASSWORD>
   ```

### Opsi B: Cloud SQL MySQL

#### 1. Buat Instance MySQL
1. Buka [Cloud SQL](https://console.cloud.google.com/sql/instances)
2. Klik "Create Instance" → "Choose MySQL"
3. Konfigurasi:
   - **Instance ID**: `lfn-hauling-db`
   - **Root password**: Buat password yang kuat
   - **Database version**: MySQL 8.0 (atau latest)
   - **Region**: pilih region terdekat
4. Klik "Create Instance"

#### 2-4. Sama seperti langkah PostgreSQL di atas, dengan penyesuaian port MySQL (3306)

---

## 📝 Menjalankan Skema Database

### Pilih skema sesuai database yang Anda buat:

### Untuk PostgreSQL:
1. Buka Cloud SQL instance
2. Klik "Connect" → "Connect using Cloud Shell"
3. Jalankan perintah:
   ```bash
   gcloud sql databases lfn_hauling_db \
       --instance=lfn-hauling-db \
       --project=<PROJECT_ID>
   ```
4. Upload file `Scripts/schema_postgresql.sql`
5. Atau copy-paste isi file tersebut ke Cloud Shell

### Untuk MySQL:
1. Buka Cloud SQL instance
2. Klik "Connect" → "Connect using Cloud Shell"
3. Upload dan jalankan file `Scripts/schema_mysql.sql`

### Verifikasi Tabel:
```sql
-- PostgreSQL
SELECT table_name FROM information_schema.tables
WHERE table_schema = 'public';

-- MySQL
SHOW TABLES;
```

Hasil harusnya:
- `fuel_receipts`
- `fuel_usages`

---

## ⚙️ Konfigurasi Koneksi di Aplikasi

### 1. Edit `appsettings.json`

#### Untuk GCP PostgreSQL:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=<GCP_PUBLIC_IP>;Port=5432;Database=lfn_hauling_db;Username=postgres;Password=<YOUR_PASSWORD>;SSL Mode=Require;Trust Server Certificate=true",
    "UseProvider": "PostgreSQL"
  }
}
```

#### Untuk GCP MySQL:
```json
{
  "ConnectionStrings": {
    "MySQL": "Server=<GCP_PUBLIC_IP>;Port=3306;Database=lfn_hauling_db;User=root;Password=<YOUR_PASSWORD>;SSL Mode=Required;AllowPublicKeyRetrieval=true",
    "UseProvider": "MySQL"
  }
}
```

### 2. Ganti Placeholder:
- `<GCP_PUBLIC_IP>` → IP publik dari Cloud SQL instance
- `<YOUR_PASSWORD>` → Password yang dibuat saat setup

### 3. Untuk Production - Gunakan Cloud SQL Proxy
Lebih aman menggunakan Cloud SQL proxy alih-alih IP publik langsung:

#### Install Cloud SQL Proxy:
```bash
# Windows (PowerShell)
Invoke-WebRequest -Uri "https://dl.google.com/cloudsql/cloud_sql_proxy_x64.exe" -OutFile "cloud_sql_proxy.exe"

# Linux/Mac
wget https://dl.google.com/cloudsql/cloud_sql_proxy.linux.amd64 -O cloud_sql_proxy
chmod +x cloud_sql_proxy
```

#### Jalankan Proxy:
```bash
# Windows
.\cloud_sql_proxy.exe -instances="<PROJECT_ID>:<REGION>:lfn-hauling-db"=tcp:5432

# Linux/Mac
./cloud_sql_proxy -instances="<PROJECT_ID>:<REGION>:lfn-hauling-db"=tcp:5432
```

#### Update Connection String (via Proxy):
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=lfn_hauling_db;Username=postgres;Password=<YOUR_PASSWORD>",
    "UseProvider": "PostgreSQL"
  }
}
```

---

## 🧪 Testing Koneksi

### 1. Test dari Aplikasi
Jalankan aplikasi:
```bash
cd HaulingDemoApp
dotnet run
```

### 2. Cek Health Endpoint
Buka browser:
```
http://localhost:5000/health
```

Response harusnya:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-15T...",
  "database": "PostgreSQL"
}
```

### 3. Test Upload Data
1. Buka `http://localhost:5000`
2. Pilih Site dan Data Type
3. Upload file CSV
4. Cek apakah data masuk ke database

### 4. Test Download Data
1. Pilih Site dan Data Type
2. Klik "Download Data"
3. File harusnya berisi data dari database

---

## 🔧 Troubleshooting

### Error: "Connection refused"
- Cek apakah Cloud SQL instance sudah aktif (Running)
- Verifikasi IP address di connection string
- Pastikan firewall mengizinkan koneksi (cek Authorized Networks)

### Error: "Authentication failed"
- Verifikasi username dan password
- Pastikan database user sudah dibuat dengan permission yang benar

### Error: "Database does not exist"
- Pastikan database `lfn_hauling_db` sudah dibuat
- Cek tab "Databases" di Cloud SQL console

### Error: "Relation does not exist"
- Pastikan skema database sudah dijalankan
- Verifikasi tabel sudah dibuat dengan query `SHOW TABLES;`

### Slow Performance
- Upgrade instance size di Cloud SQL
- Tambahkan index sesuai kebutuhan
- Gunakan connection pooling di aplikasi

### Security Best Practices untuk Production:
1. ❌ JANGAN gunakan IP public langsung
2. ✅ GUNAKAN Cloud SQL Proxy atau Cloud SQL Connector
3. ✅ GUNAKAN Private IP dengan VPC
4. ✅ GUNAKAN IAM authentication alih-alih password
5. ✅ GUNAKAN Secrets Manager untuk menyimpan password
6. ✅ ENABLE SSL/TLS untuk semua koneksi
7. ✅ LIMIT IP addresses di Authorized Networks
8. ✅ ROTATE passwords secara berkala

---

## 📚 Referensi

- [Cloud SQL Documentation](https://cloud.google.com/sql/docs)
- [Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [MySQL Connection Strings](https://dev.mysql.com/doc/connector-net/en/connector-net-connection-options.html)

---

## ✅ Checklist Setelah Setup:

- [ ] Cloud SQL instance sudah dibuat dan running
- [ ] Database `lfn_hauling_db` sudah dibuat
- [ ] Skema database sudah dijalankan
- [ ] Tabel `fuel_receipts` dan `fuel_usages` sudah ada
- [ ] Connection string sudah dikonfigurasi di `appsettings.json`
- [ ] IP address sudah ditambahkan ke Authorized Networks
- [ ] Aplikasi berhasil connect (health check OK)
- [ ] Upload data berhasil
- [ ] Download data berhasil

Good luck! 🚀
