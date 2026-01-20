using InvoiceApp.Controllers;
using InvoiceApp.Helpers;
using InvoiceApp.Repositories;
using InvoiceApp.Services;
using InvoiceApp.Views;

namespace InvoiceApp;

public class App
{
    private readonly ProductView _productView;
    private readonly InvoiceView _invoiceView;

    public App()
    {
        // Repos
        var productRepo = new ProductRepository();
        var invoiceRepo = new InvoiceRepository();

        // Servicios
        var productService = new ProductService(productRepo);
        var invoiceService = new InvoiceService(invoiceRepo, productRepo);

        // Controladores
        var productController = new ProductController(productService);
        var invoiceController = new InvoiceController(invoiceService);

        // Vistas (Aquí inyectamos los controladores a las vistas)
        _productView = new ProductView(productController);
        _invoiceView = new InvoiceView(invoiceController);
    }

    public void Run()
    {
        // Menu Principal
        bool loop = true;
        while (loop)
        {
            var selectedChoice = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Invoice App (P2 - Juan Rosario)\nDeveloped By Angel",
                    Choices = ["Modulo de Productos", "Modulo de Facturas", "Salir del programa"],
                    IsMainMenu = true,
                }
            );

            switch (selectedChoice)
            {
                case 2:
                case -1:
                    if (HandleExit(selectedChoice == 2))
                    {
                        loop = false;
                        break;
                    }
                    break;
                case 0:
                    _productView.ShowMenu();
                    break;
                case 1:
                    _invoiceView.ShowMenu();
                    break;
            }
        }
    }

    // Maneja la salida del programa, con confirmación si es necesario
    private bool HandleExit(bool shouldConfirm)
    {
        if (shouldConfirm)
        {
            var confirm = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Estas seguro que deseas salir?",
                    Choices = ["Si, deseo salir.", "No, no quiero salir ahora."],
                }
            );

            if (confirm == 0)
            {
                return true;
            }

            return false;
        }
        else
        {
            return true;
        }
    }
}
