-- =====================================================
-- LFN  App Database Schema (PostgreSQL)
-- =====================================================

-- Drop tables if exist (untuk fresh install)
DROP TABLE IF EXISTS fuel_receipts CASCADE;
DROP TABLE IF EXISTS fuel_usages CASCADE;

-- Create Fuel Receipts Table
CREATE TABLE fuel_receipts (
    id SERIAL PRIMARY KEY,
    no INTEGER NOT NULL,
    tanggal DATE NOT NULL,
    site VARCHAR(100) NOT NULL,
    supplier VARCHAR(200) NOT NULL,
    liter NUMERIC(18,2) NOT NULL DEFAULT 0,
    jenis_bbm VARCHAR(50) NOT NULL,
    harga_per_liter NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_harga NUMERIC(18,2) NOT NULL DEFAULT 0,
    no_tiket VARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    keterangan VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    -- Unique constraint untuk mencegah duplikasi berdasarkan no tiket
    CONSTRAINT uq_fuel_receipts_tiket UNIQUE (no_tiket)
);

-- Create Fuel Usages Table
CREATE TABLE fuel_usages (
    id SERIAL PRIMARY KEY,
    no INTEGER NOT NULL,
    tanggal DATE NOT NULL,
    site VARCHAR(100) NOT NULL,
    unit_no VARCHAR(50) NOT NULL,
    operator_name VARCHAR(100) NOT NULL,
    liter_awal NUMERIC(18,2) NOT NULL DEFAULT 0,
    liter_akhir NUMERIC(18,2) NOT NULL DEFAULT 0,
    pemakaian NUMERIC(18,2) NOT NULL DEFAULT 0,
    jam_kerja NUMERIC(18,2) NOT NULL DEFAULT 0,
    efisiensi NUMERIC(18,2) NOT NULL DEFAULT 0,
    keterangan VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    -- Unique constraint untuk mencegah duplikasi unit di site yang sama pada hari yang sama
    CONSTRAINT uq_fuel_usages_site_tanggal_unit UNIQUE (site, tanggal, unit_no)
);

-- Create Indexes untuk performa
CREATE INDEX idx_fuel_receipts_tanggal ON fuel_receipts(tanggal DESC);
CREATE INDEX idx_fuel_receipts_site ON fuel_receipts(site);
CREATE INDEX idx_fuel_receipts_site_tanggal ON fuel_receipts(site, tanggal);
CREATE INDEX idx_fuel_receipts_no_tiket ON fuel_receipts(no_tiket);
CREATE INDEX idx_fuel_usages_tanggal ON fuel_usages(tanggal DESC);
CREATE INDEX idx_fuel_usages_site ON fuel_usages(site);
CREATE INDEX idx_fuel_usages_site_tanggal ON fuel_usages(site, tanggal);
CREATE INDEX idx_fuel_usages_unit_no ON fuel_usages(unit_no);

-- Create Function untuk auto update updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create Triggers untuk updated_at
CREATE TRIGGER update_fuel_receipts_updated_at
    BEFORE UPDATE ON fuel_receipts
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_fuel_usages_updated_at
    BEFORE UPDATE ON fuel_usages
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Insert sample data
INSERT INTO fuel_receipts (no, tanggal, site, supplier, liter, jenis_bbm, harga_per_liter, total_harga, no_tiket, start_time, end_time, keterangan)
VALUES
    (1, CURRENT_DATE, 'Sungai Dua', 'PT.Supplier ABC', 5000, 'Solar', 15000, 75000000, 'TKT001', '08:00:00', '09:30:00', 'Penerimaan rutin'),
    (2, CURRENT_DATE, 'Sungai Dua', 'PT.Supplier XYZ', 3000, 'Solar', 15000, 45000000, 'TKT002', '10:00:00', '10:45:00', 'Penerimaan tambahan')
ON CONFLICT (no_tiket) DO NOTHING;

INSERT INTO fuel_usages (no, tanggal, site, unit_no, operator_name, liter_awal, liter_akhir, pemakaian, jam_kerja, efisiensi, keterangan)
VALUES
    (1, CURRENT_DATE, 'Sungai Dua', 'DT-001', 'John Doe', 1000, 1500, 500, 8, 62.5, 'Penggunaan normal'),
    (2, CURRENT_DATE, 'Sungai Dua', 'DT-002', 'Jane Smith', 2000, 2300, 300, 6, 50.0, 'Penggunaan siang')
ON CONFLICT (site, tanggal, unit_no) DO NOTHING;

-- Verification query
SELECT 'fuel_receipts' as table_name, COUNT(*) as row_count FROM fuel_receipts
UNION ALL
SELECT 'fuel_usages' as table_name, COUNT(*) as row_count FROM fuel_usages;
