namespace HaulingDemoApp.Models;

// ==================== API REQUEST MODELS ====================
public class LinkTripRequest
{
    public int TripId { get; set; }
}

public class FuelAnalysisGenerateRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal FuelPricePerLiter { get; set; }
}

public class DriverProductivityGenerateRequest
{
    public int DriverId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class ApproveRequest
{
    public int ApprovedBy { get; set; }
}

public class RejectRequest
{
    public string? Reason { get; set; }
}
