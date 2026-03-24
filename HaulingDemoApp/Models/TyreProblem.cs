namespace HaulingDemoApp.Models
{
    public class TyreProblem
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime Tanggal { get; set; }
        public string Site { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string MerkType { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Problem { get; set; } = string.Empty;
        public string Kerusakan { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal StartHM { get; set; }
        public decimal EndHM { get; set; }
        public decimal TotalHM { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
