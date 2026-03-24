namespace HaulingDemoApp.Models;

public class PurchaseRequestInput
{
    public DateTime? PRDate { get; set; }
    public string Site { get; set; } = "";
    public string Department { get; set; } = "";
    public string? RequestedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
    public List<PRItemInput>? Items { get; set; }
}

public class PRItemInput
{
    public string PartNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal EstimatedPrice { get; set; }
    public string? Purpose { get; set; }
    public string? Priority { get; set; }
}
