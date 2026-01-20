using InvoiceApp.Controllers;
using InvoiceApp.Domain;
using InvoiceApp.Helpers;

namespace InvoiceApp.Views;

public class InvoiceView
{
    private readonly InvoiceController _controller;
    private readonly string _continuePrompt = "Presione [Enter] para continuar...";

    public InvoiceView(InvoiceController controller)
    {
        _controller = controller;
    }

    public void ShowMenu()
    {
        // Variable para controlar el regreso al menu principal
        bool backToMainMenu = false;

        while (!backToMainMenu)
        {
            int option = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Modulo de Facturas",
                    Choices =
                    [
                        "Crear Factura",
                        "Ver Facturas",
                        "Volver al Menu Principal", // Siempre es útil tener una opción de volver explícita
                    ],
                }
            );

            switch (option)
            {
                case -1:
                case 2:
                    backToMainMenu = true;
                    break;
                case 0:
                {
                    HandleCreateInvoice();
                    break;
                }
                case 1:
                {
                    HandleInvoiceList();
                    break;
                }
            }
        }
    }

    // Handler para crear una nueva factura
    private void HandleCreateInvoice()
    {
        Console.Clear();
        Console.WriteLine("==== Crear Nueva Factura ====");

        // Intentamos crear una nueva factura mediante el controlador
        Invoice? newInvoice = null;
        try
        {
            newInvoice = _controller.CreateInvoice();
            Console.WriteLine($"\nFactura con ID {newInvoice.Id} creada exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al crear la factura: {ex.Message}");
        }

        // Esperamos a que el usuario presione Enter para continuar
        Console.WriteLine(_continuePrompt);
        Console.ReadLine();

        // Llamamos al handler para las acciones de creacion de factura
        if (newInvoice != null)
        {
            HandleCreateInvoiceActions(newInvoice);
        }
    }

    private void HandleCreateInvoiceActions(Invoice createdInvoice)
    {
        while (true)
        {
            Invoice invoice = _controller.GetInvoiceById(createdInvoice.Id)!;

            int actionOption = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = $"Factura ID '{createdInvoice.Id}'",
                    Choices =
                    [
                        "Ver detalles de la factura",
                        "Agregar Producto",
                        "Modificar Producto",
                        "Remover Producto",
                        "Emitir Factura",
                        "Cancelar Factura",
                    ],
                }
            );

            switch (actionOption)
            {
                case 0:
                    DisplayInvoiceDetails(invoice);
                    break;
                case 1:
                    HandleAddProductsToInvoice(invoice);
                    break;
                case 2:
                    HandleModifyProductsInInvoice(invoice);
                    break;
                case 3:
                    HandleRemoveProductsFromInvoice(invoice);
                    break;
                case 4:
                    // No hacemos nada especial puesto que la factura ya fue creada como DRAFT y la mantenemos asi
                    Console.WriteLine("Factura emitida exitosamente.");
                    Console.WriteLine(_continuePrompt);
                    Console.ReadLine();
                    return;
                case 5:
                    HandleInvoiceCancel(invoice);
                    return;
            }
        }
    }

    // Handler para agregar productos a la factura
    private void HandleAddProductsToInvoice(Invoice invoice)
    {
        while (true)
        {
            var products = _controller.GetAllProducts();
            if (products.Count == 0)
            {
                Console.WriteLine("No hay productos disponibles para agregar.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            int selectedProductIdx = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Selecciona un producto para agregar",
                    Choices = products
                        .Select(p =>
                            $"ID: {p.Id} | Nombre: {p.Name} | Precio: ${p.Price} | Stock: {p.Stock}"
                        )
                        .ToArray(),
                }
            );

            // SI el usuario selecciona -1, salimos del menu de productos
            if (selectedProductIdx == -1)
            {
                break;
            }

            var selectedProduct = products[selectedProductIdx];

            int? quantity = AskForQuantity(selectedProduct);

            if (quantity == null)
            {
                // Si el usuario no ingreso una cantidad, volvemos al menu de productos
                continue;
            }

            // Agregamos el producto a la factura mediante el controlador
            try
            {
                _controller.AddProductToInvoice(invoice.Id, selectedProduct.Id, quantity.Value);
                Console.WriteLine(
                    $"Producto '{selectedProduct.Name}' agregado a la factura en cantidad {quantity}."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar el producto: {ex.Message}");
            }

            Console.WriteLine(_continuePrompt);
            Console.ReadLine();
        }
    }

    // Pedir y validar la cantidad de un producto
    private int? AskForQuantity(Product product)
    {
        // Variable para la cantidad
        int? quantity;
        do
        {
            // Pedimos la cantidad a agregar
            quantity = Input.ReadRequiredInt(
                $"Ingresa la cantidad de '{product.Name}' a agregar (Stock disponible: {product.Stock}): ",
                new Input.ReadRequiredIntArgs { AllowEmpty = false }
            );

            // Validamos la cantidad ingresada
            if (quantity <= 0 || quantity > product.Stock)
            {
                Console.WriteLine("Cantidad invalida. Intenta de nuevo.");
            }

            // Si no es valida, el while hara el resto
        } while (quantity <= 0 || quantity > product.Stock);

        // Retornamos la cantidad valida
        return quantity;
    }

    private void HandleModifyProductsInInvoice(Invoice invoice)
    {
        while (true)
        {
            // Refrescamos la factura para obtener los datos mas recientes
            invoice = _controller.GetInvoiceById(invoice.Id)!;

            if (invoice.Items.Count == 0)
            {
                Console.WriteLine("No hay productos en la factura para modificar.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            // Obtenemos la lista de productos en la factura
            var items = invoice.Items.ToList();

            // Mostramos el menu para seleccionar un producto
            int selectedItemIdx = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Selecciona un producto para modificar",
                    Choices = items
                        .Select(i =>
                            $"ID: {i.Product.Id} | Nombre: {i.Product.Name} | Precio: ${i.Product.Price} | Cantidad: {i.Quantity}"
                        )
                        .ToArray(),
                }
            );

            // SI el usuario selecciona -1, salimos del menu de productos
            if (selectedItemIdx == -1)
            {
                break;
            }

            var selectedProduct = items[selectedItemIdx].Product;

            if (selectedProduct == null)
            {
                Console.WriteLine("Producto no encontrado en la factura.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            int? newQuantity = AskForQuantity(selectedProduct);

            if (newQuantity == null)
            {
                // Si el usuario no ingreso una cantidad, volvemos al menu de productos
                continue;
            }

            try
            {
                _controller.ModifyProductFromInvoice(
                    invoice.Id,
                    selectedProduct.Id,
                    newQuantity.Value
                );
                Console.WriteLine("Cantidad del producto modificada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar la cantidad: {ex.Message}");
            }

            Console.WriteLine(_continuePrompt);
            Console.ReadLine();
        }
    }

    private void HandleRemoveProductsFromInvoice(Invoice invoice)
    {
        while (true)
        {
            // Refrescamos la factura para obtener los datos mas recientes
            invoice = _controller.GetInvoiceById(invoice.Id)!;

            if (invoice.Items.Count == 0)
            {
                Console.WriteLine("No hay productos en la factura para remover.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            // Obtenemos la lista de productos en la factura
            var items = invoice.Items.ToList();

            // Mostramos el menu para seleccionar un producto
            int selectedItemIdx = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Selecciona un producto para remover",
                    Choices = items
                        .Select(i =>
                            $"ID: {i.Product.Id} | Nombre: {i.Product.Name} | Precio: ${i.Product.Price} | Cantidad: {i.Quantity}"
                        )
                        .ToArray(),
                }
            );

            // SI el usuario selecciona -1, salimos del menu de productos
            if (selectedItemIdx == -1)
            {
                break;
            }

            var selectedProduct = items[selectedItemIdx].Product;

            if (selectedProduct == null)
            {
                Console.WriteLine("Producto no encontrado en la factura.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            try
            {
                _controller.RemoveProductFromInvoice(invoice.Id, selectedProduct.Id);
                Console.WriteLine($"Producto '{selectedProduct.Name}' removido de la factura.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al remover el producto: {ex.Message}");
            }

            Console.WriteLine(_continuePrompt);
            Console.ReadLine();
        }
    }

    private void HandleInvoiceCancel(Invoice invoice)
    {
        // Removemos todos los productos de la factura antes de borrarla
        // Eso permitira restaurar el stock de los productos
        invoice.Items.ForEach(
            (item) =>
            {
                _controller.RemoveProductFromInvoice(invoice.Id, item.Product.Id);
            }
        );

        // Borramos la factura
        _controller.DeleteInvoice(invoice.Id);

        // Confirmamos la cancelacion al usuario
        Console.WriteLine("Factura cancelada.");
        Console.WriteLine(_continuePrompt);
        Console.ReadLine();
    }

    // Handler para listar las facturas
    private void HandleInvoiceList()
    {
        while (true)
        {
            // Obtenemos todas las facturas desde el controlador
            var invoices = _controller.GetAllInvoices();

            // Si no hay facturas, mostramos un mensaje y regresamos
            if (invoices.Count == 0)
            {
                Console.WriteLine("No hay facturas disponibles actualmente.");
                Console.WriteLine(_continuePrompt);
                Console.ReadLine();
                return;
            }

            // Mostramos el menu interactivo para seleccionar una factura
            int selectedInvoiceIdx = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Lista de Facturas",
                    Choices = invoices
                        .Select(i => $"ID: {i.Id} | Fecha: {i.Date} | Total: ${i.Total}")
                        .ToArray(),
                }
            );

            // Si el usuario decide salir, rompemos el ciclo
            if (selectedInvoiceIdx == -1)
            {
                break;
            }

            var selectedInvoice = invoices[selectedInvoiceIdx];

            // Manejamos las acciones para la factura seleccionada
            HandleInvoiceActions(selectedInvoice);
        }
    }

    // Handler para las acciones de una factura seleccionada
    private static void HandleInvoiceActions(Invoice selectedInvoice)
    {
        while (true)
        {
            // Mostramos el menu de acciones para la factura seleccionada
            int actionOption = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = $"Factura ID '{selectedInvoice.Id}'",
                    Choices = ["Ver Detalles de la Factura", "Volver al listado de facturas"],
                }
            );

            // Manejo de la seleccion del usuario
            switch (actionOption)
            {
                case -1:
                case 1:
                    // Volver al listado de facturas
                    return;
                case 0:
                    // Ver detalles de la factura
                    DisplayInvoiceDetails(selectedInvoice);
                    break;
            }
        }
    }

    private static void DisplayInvoiceDetails(Invoice selectedInvoice)
    {
        Console.Clear();

        // Mostramos los detalles de la factura
        Console.WriteLine($"==== Detalles de la Factura ID: {selectedInvoice.Id} ====\n");
        Console.WriteLine($"Fecha: {selectedInvoice.Date}");
        Console.WriteLine("Items:");
        // Iteramos sobre los items de la factura para mostrarlos
        foreach (var item in selectedInvoice.Items)
        {
            Console.WriteLine(
                $"- {item.Product.Name}: Cantidad {item.Quantity}, Precio Unitario ${item.UnitPrice}, Subtotal ${item.Subtotal}"
            );
        }
        // Mostramos los calculos de subtotal, impuesto y total
        Console.WriteLine($"Subtotal: ${selectedInvoice.Subtotal}");
        Console.WriteLine($"Impuesto: {selectedInvoice.Tax * 100}%");
        Console.WriteLine($"Total: ${selectedInvoice.Total}");
        Console.WriteLine("\nPresione [Enter] para volver...");
        Console.ReadLine();
    }
}
