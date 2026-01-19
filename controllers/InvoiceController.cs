using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Controllers;

public class InvoiceController
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    public Invoice CreateInvoice()
    {
        return _service.CreateInvoice();
    }

    public void AddProductToInvoice(int invoiceId, int productId, int quantity)
    {
        _service.AddProductToInvoice(invoiceId, productId, quantity);
    }

    public void RemoveProductFromInvoice(int invoiceId, int productId)
    {
        _service.RemoveProductFromInvoice(invoiceId, productId);
    }

    public Invoice? GetInvoiceById(int invoiceId)
    {
        return _service.GetInvoiceById(invoiceId);
    }

    public List<Invoice> GetAllInvoices()
    {
        return _service.GetAllInvoices();
    }

    public void DeleteInvoice(int invoiceId)
    {
        _service.DeleteInvoice(invoiceId);
    }
}
