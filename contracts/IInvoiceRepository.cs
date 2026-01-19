using InvoiceApp.Domain;

namespace InvoiceApp.Contracts;

public interface IInvoiceRepository
{
    void Create(Invoice invoice);
    void UpdateById(int invoiceId, Invoice updatedInvoice);
    void DeleteById(int invoiceId);
    Invoice? FindById(int invoiceId);
    List<Invoice> FindAll();
}
