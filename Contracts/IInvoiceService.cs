using InvoiceApp.Domain;

namespace InvoiceApp.Contracts;

public interface IInvoiceService
{
    Invoice CreateInvoice();
    void AddProductToInvoice(int invoiceId, int productId, int quantity);
    void RemoveProductFromInvoice(int invoiceId, int productId);
    Invoice? GetInvoiceById(int invoiceId);
    List<Invoice> GetAllInvoices();
    void DeleteInvoice(int invoiceId);
}
