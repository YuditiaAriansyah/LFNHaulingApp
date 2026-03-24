-- =====================================================
-- LFN  App Database Schema (MySQL)
-- =====================================================

-- Drop tables if exist (untuk fresh install)
DROP TABLE IF EXISTS fuel_receipts;
DROP TABLE IF EXISTS fuel_usages;

-- Create Fuel Receipts Table
CREATE TABLE fuel_receipts (
    id INT AUTO_INCREMENT PRIMARY KEY,
    no INT NOT NULL,
    tanggal DATE NOT NULL,
    site VARCHAR(100) NOT NULL,
    supplier VARCHAR(200) NOT NULL,
    liter DECIMAL(18,2) NOT NULL DEFAULT 0,
    jenis_bbm VARCHAR(50) NOT NULL,
    harga_per_liter DECIMAL(18,2) NOT NULL DEFAULT 0,
    total_harga DECIMAL(18,2) NOT NULL DEFAULT 0,
    no_tiket VARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    keterangan VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Unique constraint untuk mencegah duplikasi berdasarkan no tiket
    UNIQUE KEY uq_fuel_receipts_tiket (no_tiket),
    INDEX idx_tanggal (tanggal),
    INDEX idx_site (site),
    INDEX idx_site_tanggal (site, tanggal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Fuel Usages Table
CREATE TABLE fuel_usages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    no INT NOT NULL,
    tanggal DATE NOT NULL,
    site VARCHAR(100) NOT NULL,
    unit_no VARCHAR(50) NOT NULL,
    operator_name VARCHAR(100) NOT NULL,
    liter_awal DECIMAL(18,2) NOT NULL DEFAULT 0,
    liter_akhir DECIMAL(18,2) NOT NULL DEFAULT 0,
    pemakaian DECIMAL(18,2) NOT NULL DEFAULT 0,
    jam_kerja DECIMAL(18,2) NOT NULL DEFAULT 0,
    efisiensi DECIMAL(18,2) NOT NULL DEFAULT 0,
    keterangan VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Unique constraint untuk mencegah duplikasi unit di site yang sama pada hari yang sama
    UNIQUE KEY uq_fuel_usages_site_tanggal_unit (site, tanggal, unit_no),
    INDEX idx_tanggal (tanggal),
    INDEX idx_site (site),
    INDEX idx_site_tanggal (site, tanggal),
    INDEX idx_unit_no (unit_no)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert sample data dengan IGNORE untuk skip duplikat
INSERT IGNORE INTO fuel_receipts (no, tanggal, site, supplier, liter, jenis_bbm, harga_per_liter, total_harga, no_tiket, start_time, end_time, keterangan)
VALUES
    (1, CURDATE(), 'Sungai Dua', 'PT.Supplier ABC', 5000, 'Solar', 15000, 75000000, 'TKT001', '08:00:00', '09:30:00', 'Penerimaan rutin'),
    (2, CURDATE(), 'Sungai Dua', 'PT.Supplier XYZ', 3000, 'Solar', 15000, 45000000, 'TKT002', '10:00:00', '10:45:00', 'Penerimaan tambahan');

INSERT IGNORE INTO fuel_usages (no, tanggal, site, unit_no, operator_name, liter_awal, liter_akhir, pemakaian, jam_kerja, efisiensi, keterangan)
VALUES
    (1, CURDATE(), 'Sungai Dua', 'DT-001', 'John Doe', 1000, 1500, 500, 8, 62.5, 'Penggunaan normal'),
    (2, CURDATE(), 'Sungai Dua', 'DT-002', 'Jane Smith', 2000, 2300, 300, 6, 50.0, 'Penggunaan siang');

-- Verification query
SELECT 'fuel_receipts' as table_name, COUNT(*) as row_count FROM fuel_receipts
UNION ALL
SELECT 'fuel_usages' as table_name, COUNT(*) as row_count FROM fuel_usages;
