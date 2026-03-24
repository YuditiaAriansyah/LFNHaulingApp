namespace HaulingDemoApp.Models;

// Purchase Order Input Models
public class PurchaseOrderInput
{
    public DateTime? PODate { get; set; }
    public string Site { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string? VendorCode { get; set; }
    public string? PRNumber { get; set; }
    public string Status { get; set; } = "DRAFT";
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Remarks { get; set; }
    public string? ApprovedBy { get; set; }
    public List<POItemInput>? Items { get; set; }
}

public class POItemInput
{
    public string PartNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// Good Receipt Input Models
public class GoodReceiptInput
{
    public DateTime? GRDate { get; set; }
    public string Site { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string? PONumber { get; set; }
    public string Status { get; set; } = "RECEIVED";
    public string? ReceivedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Remarks { get; set; }
    public List<GRItemInput>? Items { get; set; }
}

public class GRItemInput
{
    public string PartNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? Location { get; set; }
    public string? Remarks { get; set; }
}

// Good Issue Input Models
public class GoodIssueInput
{
    public DateTime? GIDate { get; set; }
    public string Site { get; set; } = "";
    public string? Department { get; set; }
    public string? RequestNumber { get; set; }
    public string Status { get; set; } = "ISSUED";
    public string? IssuedBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Remarks { get; set; }
    public List<GIItemInput>? Items { get; set; }
}

public class GIItemInput
{
    public string PartNumber { get; set; } = "";
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal RequestedQty { get; set; }
    public decimal IssuedQty { get; set; }
    public string? Purpose { get; set; }
    public string? Remarks { get; set; }
}

// Vendor Input Models
public class VendorInput
{
    public string VendorCode { get; set; } = "";
    public string VendorName { get; set; } = "";
    public string? VendorType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? TaxId { get; set; }
    public string? NIB { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankBranch { get; set; }
    public string? PaymentTerms { get; set; }
    public string Category { get; set; } = "";
    public string Status { get; set; } = "ACTIVE";
    public string? Remarks { get; set; }
}
