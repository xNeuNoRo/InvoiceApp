using InvoiceApp.Controllers;

namespace InvoiceApp.Views;

public class InvoiceView
{
    private readonly InvoiceController _controller;

    public InvoiceView(InvoiceController controller)
    {
        _controller = controller;
    }

    public void ShowMenu() { }
}
