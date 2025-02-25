namespace PRSystem.Model
{
    public class PurchaseRequest
    {
        public string? DocType { get; set; }
        public string? DocDate { get; set; }
        public string? DocDueDate { get; set; }
        public string? RequriedDate { get; set; }
        public string? Requester { get; set; }
        public string? RequesterName { get; set; }
        public int RequesterDepartment { get; set; }
        public string? U_Department { get; set; }
        public string? U_Remarks { get; set; }
        public string? U_TransactionType { get; set; }
        public string? U_PrepBy { get; set; }
        public string? U_CompanyTIN { get; set; }
        public List<PurchaseRequestLine> DocumentLines { get; set; } = new List<PurchaseRequestLine>();
    }

    public class PurchaseRequestLine
    {
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? ItemDescription { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public string? VatGroup { get; set; }
        public string? U_PreferredSupplier { get; set; }
        public string? SupplierName { get; set; }
        public string? RequiredDate { get; set; }
        public string? CostingCode { get; set; }
        public string? ItemCode { get; set; } = string.Empty;
        public string? UoMCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double DiscountPercent { get; set; }
        public string? Size { get; set; } = string.Empty;
        public string? Color { get; set; } = string.Empty;
        public double Tax { get; set; }
        public double L_GrossPrice { get; set; }
        public string? ItemSearchTextCode { get; set; }
        public string? ItemSearchTextName { get; set; }
        public List<OItems>? FilteredItemsCode { get; set; }
        public List<OItems>? FilteredItemsName { get; set; }
        public string? ItemSearchAcctCode { get; set; }
        public string? ItemSearchAcctName { get; set; }
        public List<OGLAccounts>? FilteredAcctName { get; set; }
        public List<OGLAccounts>? FilteredAcctCode { get; set; }
        public List<OSuppliers>? FIlteredSupplier { get; set; }
        public string? SearchSupplier { get; set; }
    }

    public class ItemRequest
    {
        public string? ItemCode { get; set; }
        public string? UoMCode { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPercent { get; set; }
        public string? VatGroup { get; set; }
        public string? U_PreferredSupplier { get; set; }
        public string? RequiredDate { get; set; }
        public string? CostingCode { get; set; }
    }

    public class ServiceRequest
    {
        public string? AccountCode { get; set; }
        public string? ItemDescription { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPercent { get; set; }
        public string? VatGroup { get; set; }
        public string? U_PreferredSupplier { get; set; }
        public string? RequiredDate { get; set; }
        public string? CostingCode { get; set; }
    }

    public class PurchaseRequestResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }

    public class Items
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? U_ID007 { get; set; }
        public string? U_ID011 { get; set; }
        public string? InventoryUOM { get; set; }
    }

    public class Suppliers
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
    }

    public class OGLAccounts
    {
        public string? AcctCode { get; set; }
        public string? AcctName { get; set; }
    }
    public class OItems
    {
        public string? ItemCode { get; set; } = string.Empty;
        public string? ItemName { get; set; } = string.Empty;
        public string? U_ID007 { get; set; } = string.Empty;
        public string? U_ID011 { get; set; } = string.Empty;
        public string? InvntryUom { get; set; } = string.Empty;
    }
    public class OSuppliers
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? Supplier { get; set; }

    }
    public class OTaxCode
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
