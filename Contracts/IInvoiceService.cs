using InvoiceApp.Domain;

namespace InvoiceApp.Contracts;

public interface IInvoiceService
{
    Invoice CreateInvoice();
    void AddProductToInvoice(int invoiceId, int productId, int quantity);
    void ModifyProductQuantityFromInvoice(int invoiceId, int productId, int newQuantity);
    void RemoveProductFromInvoice(int invoiceId, int productId);
    Invoice? GetInvoiceById(int invoiceId);
    List<Invoice> GetAllInvoices();
    Product? GetProductById(int productId);
    List<Product> GetAllProducts();
    void DeleteInvoice(int invoiceId);
}
