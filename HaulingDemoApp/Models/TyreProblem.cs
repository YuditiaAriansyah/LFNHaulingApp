using System.ComponentModel.DataAnnotations.Schema;

namespace HaulingDemoApp.Models
{
    public class TyreProblem
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime Tanggal { get; set; }
        public string? Site { get; set; }
        public string? Post { get; set; }
        public string? UnitNo { get; set; }

        [Column("NoSeriTyre")]
        public string? SerialNumber { get; set; }

        public string? MerkType { get; set; }
        public string? Size { get; set; }

        [Column("ProblemDescription")]
        public string? Problem { get; set; }

        public string? Kerusakan { get; set; }
        public string? Location { get; set; }
        public decimal? StartHM { get; set; }
        public decimal? EndHM { get; set; }
        public decimal? TotalHM { get; set; }
        public decimal? Cost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
