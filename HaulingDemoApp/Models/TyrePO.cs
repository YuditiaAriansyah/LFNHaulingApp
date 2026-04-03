namespace HaulingDemoApp.Models
{
    public class TyrePO
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime Tanggal { get; set; }
        public string Site { get; set; } = string.Empty;
        public string? LokasiProblem { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public string NoPO { get; set; } = string.Empty;
        public string MerkType { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
