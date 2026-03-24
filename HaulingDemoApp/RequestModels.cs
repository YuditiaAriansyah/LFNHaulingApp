namespace HaulingDemoApp.Models;

// ==================== API REQUEST MODELS ====================
public class LinkTripRequest
{
    public int? TripId { get; set; }
    public string? TripNumber { get; set; }
}

public class FuelAnalysisGenerateRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal FuelPricePerLiter { get; set; }
}

public class DriverProductivityGenerateRequest
{
    public string DriverId { get; set; } = "";
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class ApproveRequest
{
    public int ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
}

public class RejectRequest
{
    public string? Reason { get; set; }
}

// Inventory adjustment request - uses Inventory entity properties
public class InventoryAdjustmentRequest
{
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string AdjustmentType { get; set; } = "Add"; // Add, Remove, Reduce, Set
    public string? Reason { get; set; }
    public string? AdjustedBy { get; set; }
}

// Journal entry creation request - uses JournalLineInput from Finance.cs
public class JournalEntryCreateRequest
{
    public string? EntryNumber { get; set; }
    public DateTime? EntryDate { get; set; }
    public string EntryType { get; set; } = "MANUAL";
    public string? SourceModule { get; set; }
    public string? SourceId { get; set; }
    public string Description { get; set; } = "";
    public List<JournalLineInput>? Lines { get; set; }
}
