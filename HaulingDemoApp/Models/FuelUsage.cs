namespace HaulingDemoApp.Models
{
    public class FuelUsage
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime Tanggal { get; set; }
        public string Site { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public decimal HM { get; set; }
        public decimal KM { get; set; }
        public decimal Pemakaian { get; set; }
        public decimal JamKerja { get; set; }
        public decimal EFisiensi { get; set; }
        public string Keterangan { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
