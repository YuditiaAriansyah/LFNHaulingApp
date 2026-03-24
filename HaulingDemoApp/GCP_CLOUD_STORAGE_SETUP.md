# 📦 GCP Cloud Storage Setup Guide - LFN  App

## 📋 Daftar Isi
1. [Persiapan di GCP Console](#persiapan-di-gcp-console)
2. [Membuat Storage Bucket](#membuat_storage_bucket)
3. [Setup Authentication](#setup_authentication)
4. [Konfigurasi di Aplikasi](#konfigurasi_di_aplikasi)
5. [Menggunakan GcsService](#menggunakan_gcsservice)
6. [Folder Structure di GCS](#folder_structure_di_gcs)
7. [Testing Upload/Download](#testing_uploaddownload)
8. [Integrasi dengan BigQuery](#integrasi_dengan_bigquery)
9. [Troubleshooting](#troubleshooting)

---

## 🌩️ Persiapan di GCP Console

### 1. Enable Cloud Storage API
1. Buka [APIs & Services](https://console.cloud.google.com/apis/dashboard)
2. Klik "+ Enable APIs and Services"
3. Cari "Cloud Storage JSON API"
4. Klik "Enable"

---

## 🗂️ Membuat Storage Bucket

### 1. Buat Bucket Baru
1. Buka [Cloud Storage](https://console.cloud.google.com/storage/browser)
2. Klik **"Create Bucket"**
3. Konfigurasi:
   ```
   Name: lfn-hauling-raw-data-<project-id>
   Location type: Region
   Location: asia-southeast1 (Singapore) - sama dengan Cloud SQL
   Storage class: Standard
   Access control: Uniform (recommended) atau Fine-grained
   Encryption: Google-managed key
   ```

### 2. Setup Public Access (Opsional)
Jika file perlu diakses publik:
1. Klik nama bucket
2. Tab **"Permissions"**
3. Klik **"Add members"**
4. Member: `allUsers`
5. Role: **Storage Object Viewer**
6. Klik **"Save"**
7. ⚠️ **Warning**: Ini membuat semua file di bucket bisa diakses publik!

---

## 🔐 Setup Authentication

### Opsi A: Service Account Key (Development)

#### 1. Buat Service Account
1. Buka [IAM & Admin → Service Accounts](https://console.cloud.google.com/iam-admin/serviceaccounts)
2. Klik **"Create Service Account"**
3. Konfigurasi:
   ```
   Service account name: lfn-hauling-app
   Description: Service account untuk LFN  App
   ```
4. Klik **"Create and Continue"**

#### 2. Tambah Roles
Klik **"Continue"** lalu **"Done"** (roles bisa ditambahkan nanti)

#### 3. Grant Access ke Bucket
1. Buka [Cloud Storage Browser](https://console.cloud.google.com/storage/browser)
2. Klik nama bucket
3. Tab **"Permissions"**
4. Klik **"Grant access"**
5. Principal: `lfn-hauling-app@<project-id>.iam.gserviceaccount.com`
6. Role: **Storage Object Admin** (full access)
   - Atau **Storage Object Creator** (hanya upload)
   - Atau **Storage Object Viewer** (hanya download)
7. Klik **"Grant"**

#### 4. Download Service Account Key
1. Klik service account yang baru dibuat
2. Tab **"Keys"**
3. Klik **"Add Key"** → **"Create new key"**
4. Pilih **"JSON"**
5. Klik **"Create"**
6. File JSON akan terdownload. **SIMPAN FILE INI DENGAN AMAN!**
7. ❌ **JANGAN commit file ini ke Git!**
8. ✅ **Tambahkan ke .gitignore**: `**/*.json`

---

## ⚙️ Konfigurasi di Aplikasi

### 1. Simpan Service Account Key
Simpan file JSON di lokasi yang aman, misalnya:
```
Windows: C:\Credentials\gcp-lfn-hauling-key.json
Linux/Mac: ~/.config/gcp-lfn-hauling-key.json
```

### 2. Update `appsettings.json`

#### Untuk Development:
```json
{
  "GoogleCloudStorage": {
    "BucketName": "lfn-hauling-raw-data-your-project-id",
    "CredentialPath": "C:\\Credentials\\gcp-lfn-hauling-key.json",
    "ProjectId": "your-project-id"
  }
}
```

#### Untuk Production:
Gunakan environment variable:
```bash
# Windows (PowerShell)
$env:GOOGLE_CLOUD_PROJECT="your-project-id"
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\path\to\key.json"

# Linux/Mac
export GOOGLE_CLOUD_PROJECT="your-project-id"
export GOOGLE_APPLICATION_CREDENTIALS="~/.config/gcp-lfn-hauling-key.json"
```

Atau gunakan **Workload Identity Federation** untuk production (lebih aman):

```json
{
  "GoogleCloudStorage": {
    "BucketName": "lfn-hauling-raw-data-your-project-id",
    "ProjectId": "your-project-id"
  }
}
```

Service akan otomatis menggunakan ADC (Application Default Credentials) saat di-deploy ke GCP.

### 3. Update `.gitignore`
```gitignore
# GCP Credentials
**/gcp-*-key.json
**/service-account-*.json
```

---

## 🚀 Menggunakan GcsService

### GcsService sudah terdaftar di Program.cs:
```csharp
builder.Services.AddSingleton<GcsService>();
```

### Contoh Penggunaan di Endpoints:

#### 1. Upload File ke GCS
```csharp
app.MapPost("/api/upload-to-gcs", async (HttpRequest request, GcsService gcsService) =>
{
    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var site = form["site"].ToString();
    var datatype = form["datatype"].ToString();

    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file uploaded" });

    try
    {
        // Generate object name dengan folder structure
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
            message = "File uploaded successfully to GCS",
            objectName = objectName,
            publicUrl = publicUrl,
            bucket = "lfn-hauling-raw-data"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = $"Upload failed: {ex.Message}" });
    }
});
```

#### 2. List Files dari GCS
```csharp
app.MapGet("/api/gcs-files", async (GcsService gcsService, string? prefix) =>
{
    try
    {
        var files = await gcsService.ListFilesAsync(prefix);
        return Results.Ok(new { files, count = files.Count });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

#### 3. Download File dari GCS
```csharp
app.MapGet("/api/download-from-gcs", async (GcsService gcsService, string objectName) =>
{
    try
    {
        var stream = await gcsService.DownloadToStreamAsync(objectName);

        return Results.File(
            stream,
            "application/octet-stream",
            Path.GetFileName(objectName)
        );
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

#### 4. Delete File dari GCS
```csharp
app.MapDelete("/api/gcs-files/{objectName}", async (GcsService gcsService, string objectName) =>
{
    try
    {
        await gcsService.DeleteFileAsync(objectName);
        return Results.Ok(new { message = "File deleted successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

---

## 📁 Folder Structure di GCS

GcsService akan membuat struktur folder seperti ini:

```
lfn-hauling-raw-data/
├── fuel-receipts/
│   ├── Site-A/
│   │   ├── 2025-03-04/
│   │   │   ├── receipt_data_123456789.csv
│   │   │   └── another_receipt_123456790.csv
│   │   └── 2025-03-05/
│   └── Site-B/
│       └── 2025-03-04/
└── fuel-usage/
    ├── Site-A/
    │   └── 2025-03-04/
    │       └── usage_data_123456789.csv
    └── Site-B/
        └── 2025-03-04/
```

**Object Name Format**:
```
{datatype}/{site}/{date}/{sanitized_filename}_{timestamp}{extension}
```

Contoh:
```
fuel-receipts/Site-A/2025-03-04/receipt_data_123456789.csv
fuel-usage/Site-B/2025-03-04/usage_report_123456790.csv
```

---

## 🧪 Testing Upload/Download

### 1. Test dari Browser
Buka aplikasi dan coba upload file.

### 2. Test dengan cURL
```bash
# Upload file
curl -X POST http://localhost:5000/api/upload-to-gcs \
  -F "file=@test.csv" \
  -F "site=Site-A" \
  -F "datatype=receipt"

# List files
curl "http://localhost:5000/api/gcs-files?prefix=fuel-receipts/Site-A/"

# Download file
curl -O "http://localhost:5000/api/download-from-gcs?objectName=fuel-receipts/Site-A/2025-03-04/test_123.csv"
```

### 3. Cek di GCP Console
1. Buka [Cloud Storage Browser](https://console.cloud.google.com/storage/browser)
2. Klik nama bucket
3. Verifikasi file sudah terupload dengan benar

### 4. Test Direct GCS URL
Jika bucket public:
```
https://storage.googleapis.com/lfn-hauling-raw-data/fuel-receipts/Site-A/2025-03-04/test_123.csv
```

---

## 🔄 Integrasi dengan BigQuery

### Flow Data:
```
Upload App → Cloud Storage (Raw Data) → BigQuery (Clean Data) → Dashboard
```

### Opsi 1: BigQuery Data Transfer Service
1. Buka [BigQuery](https://console.cloud.google.com/bigquery)
2. Klik dataset → **"Transfer"** → **"Create Transfer"**
3. Source: **Cloud Storage**
4. Destination: Dataset di BigQuery
5. Schedule: Setiap jam/hari (sesuai kebutuhan)
6. File format: CSV

### Opsi 2: BigQuery External Table
Query langsung dari GCS tanpa copy data:

```sql
CREATE EXTERNAL TABLE `lfn_hauling_db.fuel_receipts_external`
OPTIONS (
  format = 'CSV',
  uris = ['gs://lfn-hauling-raw-data/fuel-receipts/*.csv'],
  skip_leading_rows = 1
);
```

### Opsi 3: Load Job dari Code
```csharp
using Google.Cloud.BigQuery.V2;

// Load dari GCS ke BigQuery
var client = BigQueryClient.Create(projectId);
var job = client.CreateLoadJob(
    projectId: projectId,
    datasetId: "lfn_hauling_db",
    tableId: "fuel_receipts",
    sourceUri: "gs://lfn-hauling-raw-data/fuel-receipts/Site-A/*.csv"
);
await job.PollUntilCompletedAsync();
```

---

## 🔧 Troubleshooting

### Error: "Application Default Credentials"
**Masalah**: Service account key tidak ditemukan

**Solusi**:
1. Pastikan path di `appsettings.json` benar
2. Pastikan file JSON ada dan bisa diakses
3. Set environment variable:
   ```bash
   export GOOGLE_APPLICATION_CREDENTIALS="/path/to/key.json"
   ```

### Error: "Access Denied"
**Masalah**: Service account tidak punya akses ke bucket

**Solusi**:
1. Cek apakah service account sudah ditambahkan ke bucket permissions
2. Pastikan role yang benar: **Storage Object Admin** atau **Storage Object Creator**
3. Cek bucket access control setting (Uniform vs Fine-grained)

### Error: "Bucket not found"
**Masalah**: Nama bucket salah atau belum dibuat

**Solusi**:
1. Pastikan bucket sudah dibuat di GCP Console
2. Cek nama bucket di `appsettings.json` harus sama persis
3. Bucket name harus globally unique

### Error: "Invalid credentials"
**Masalah**: Service account key invalid atau expired

**Solusi**:
1. Download service account key lagi
2. Pastikan tidak ada editing yang merubah format JSON
3. Cek apakah key sudah di-revoke di GCP Console

### File tidak muncul di Console
**Masalah**: Upload success tapi file tidak terlihat

**Solusi**:
1. Refresh browser
2. Cek folder prefix dengan benar
3. Pastikan upload tidak throw exception (cek logs)

### Performance Issues
**Masalah**: Upload/download lambat

**Solusi**:
1. Gunakan region yang sama dengan Cloud SQL
2. Untuk file besar, gunakan **resumable upload**
3. Pertimbangkan menggunakan **parallel upload** untuk banyak file
4. Cek network bandwidth

---

## 🚨 Security Best Practices untuk Production

### ❌ JANGAN:
- Commit service account key ke Git
- Gunakan IP public langsung untuk database
- Berikan akses public ke bucket yang berisi data sensitif
- Gunakan bucket access control yang terlalu permissive

### ✅ GUNAKAN:
- **Workload Identity Federation** alih-alih service account keys
- **Secrets Manager** untuk menyimpan credentials
- **IAM Conditions** untuk granular access control
- **Bucket Lock** untuk data yang tidak boleh dihapus
- **Object Versioning** untuk mencegah accidental deletion
- **VPC Service Controls** untuk keamanan tambahan
- **Audit Logs** untuk monitoring akses

### Example: Workload Identity Federation
```yaml
# Kubernetes deployment example
spec:
  template:
    spec:
      serviceAccountName: lfn-hauling-app-ksa
  # GKE workload identity binding
  # gcloud iam service-accounts add-iam-policy-binding lfn-hauling-app@project.iam.gserviceaccount.com \
  #   --role roles/iam.workloadIdentityUser \
  #   --member "serviceAccount:project.svc.id.goog[namespace/lfn-hauling-app-ksa]"
```

---

## 📊 Monitoring dan Logging

### 1. Cloud Monitoring
- Setup alerts untuk:
  - Upload failures
  - Storage usage exceeded
  - API error rate
  - Latency

### 2. Cloud Logging
Filter logs untuk BigQuery:
```bash
resource.type="gcs_bucket"
resource.labels.bucket_name="lfn-hauling-raw-data"
severity>=WARNING
```

### 3. Cost Management
- Setup budget alerts
- Monitor storage usage
- Review access patterns
- Cleanup old files (lifecycle management)

### Example: Lifecycle Management
```bash
# Delete files older than 30 days
gsutil lifecycle set lifecycle.json gs://lfn-hauling-raw-data

# lifecycle.json:
{
  "lifecycle": {
    "rule": [
      {
        "action": {"type": "Delete"},
        "condition": {"age": 30}
      }
    ]
  }
}
```

---

## 📚 Referensi

- [Cloud Storage Documentation](https://cloud.google.com/storage/docs)
- [Cloud Storage for .NET](https://cloud.google.com/storage/docs/reference/libraries)
- [BigQuery Documentation](https://cloud.google.com/bigquery/docs)
- [IAM Best Practices](https://cloud.google.com/iam/docs/best-practices)
- [Workload Identity Federation](https://cloud.google.com/iam/docs/workload-identity-federation)

---

## ✅ Checklist Setup Cloud Storage:

### GCP Setup:
- [ ] Cloud Storage API sudah di-enable
- [ ] Bucket `lfn-hauling-raw-data` sudah dibuat
- [ ] Region sudah dipilih (sesuai dengan Cloud SQL)
- [ ] Access control sudah dikonfigurasi

### Authentication:
- [ ] Service account sudah dibuat
- [ ] Service account sudah diberikan akses ke bucket
- [ ] Service account key sudah didownload
- [ ] Key sudah disimpan dengan aman
- [ ] Key ditambahkan ke .gitignore

### Application Setup:
- [ ] Package `Google.Cloud.Storage.V1` sudah diinstall
- [ ] `GcsService` sudah diregister di Program.cs
- [ ] `appsettings.json` sudah dikonfigurasi
- [ ] Path ke service account key sudah benar

### Testing:
- [ ] Upload file berhasil
- [ ] File muncul di GCP Console
- [ ] List files berhasil
- [ ] Download file berhasil
- [ ] Delete file berhasil
- [ ] Folder structure sesuai format

### Integration:
- [ ] Siap untuk integrasi dengan BigQuery
- [ ] Monitoring dan logging sudah disetup
- [ ] Cost alerts sudah dikonfigurasi

---

Good luck! 🚀
