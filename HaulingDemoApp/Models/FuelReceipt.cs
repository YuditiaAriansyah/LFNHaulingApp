namespace HaulingDemoApp.Models
{
    public class FuelReceipt
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime Tanggal { get; set; }
        public string Site { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public decimal Liter { get; set; }
        public string JenisBBM { get; set; } = string.Empty;
        public decimal HargaPerLiter { get; set; }
        public decimal TotalHarga { get; set; }
        public string NoTiket { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Keterangan { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
