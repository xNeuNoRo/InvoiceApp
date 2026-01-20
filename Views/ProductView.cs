using InvoiceApp.Controllers;
using InvoiceApp.Domain;
using InvoiceApp.Helpers;

namespace InvoiceApp.Views;

public class ProductView
{
    private readonly ProductController _controller;

    public ProductView(ProductController controller)
    {
        _controller = controller;
    }

    public void ShowMenu()
    {
        // Variable para controlar el regreso al menu principal
        bool backToMainMenu = false;

        // Mientras no se regrese al menu principal
        while (!backToMainMenu)
        {
            // Mostrar el menu de productos
            int option = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Modulo de Productos",
                    Choices =
                    [
                        "Crear Producto",
                        "Ver Productos",
                        "Volver al Menu Principal", // Siempre es útil tener una opción de volver explícita
                    ],
                }
            );

            // Manejamos la opción seleccionada
            switch (option)
            {
                case -1:
                case 2:
                    backToMainMenu = true;
                    break;
                case 0:
                {
                    HandleCreateProduct();
                    break;
                }
                case 1:
                {
                    HandleProductList();
                    break;
                }
            }
        }
    }

    private void HandleCreateProduct()
    {
        // Argumentos de cada funcion de input requerida
        Input.ReadRequiredStrArgs strArgs = new Input.ReadRequiredStrArgs { AllowEmpty = false };
        Input.ReadRequiredDecArgs decArgs = new Input.ReadRequiredDecArgs { AllowEmpty = false };
        Input.ReadRequiredIntArgs intArgs = new Input.ReadRequiredIntArgs { AllowEmpty = false };

        // Pedir nombre
        string name = Input.ReadRequiredStr("Nombre del producto: ", strArgs);

        // Pedir precio
        decimal? price = Input.ReadRequiredDec("Precio del producto: ", decArgs);

        // Pedir stock
        int? stock = Input.ReadRequiredInt("Stock inicial del producto: ", intArgs);

        // Asegurarse de que no son nulos
        if (price == null)
            throw new InvalidOperationException("Precio no proporcionado.");
        if (stock == null)
            throw new InvalidOperationException("Stock no proporcionado.");

        _controller.CreateProduct(name, price.Value, stock.Value);
    }

    // Maneja la lista de productos y las acciones sobre ellos
    private void HandleProductList()
    {
        while (true)
        {
            // Obtener todos los productos
            var products = _controller.GetAllProducts();

            // Si no hay productos, mostrar mensaje y esperar a que el usuario presione Enter
            if (products.Count == 0)
            {
                Console.WriteLine("No hay productos disponibles actualmente.");
                Console.WriteLine("Presiona [Enter] para volver al menu de productos...");
                Console.ReadLine();
                return;
            }

            // Mostrar la lista de productos
            int selectedProductIdx = InteractiveMenu.Show(
                new InteractiveMenu.MenuArgs
                {
                    MenuTitle = "Lista de Productos",
                    Choices = products.Select(p => $"ID: {p.Id} | Nombre: {p.Name} | Precio: ${p.Price} | Stock: {p.Stock}").ToArray(),
                }
            );

            // SI el usuario selecciona -1, salimos del menu de productos
            if (selectedProductIdx == -1)
            {
                break;
            }

            // Obtener el producto seleccionado
            var selectedProduct = products[selectedProductIdx];

            // Manejar acciones sobre el producto seleccionado
            HandleProductActions(selectedProduct);
        }
    }

    // Maneja las acciones para un producto seleccionado
    private void HandleProductActions(Product selectedProduct)
    {
        // Mostrar opciones para el producto seleccionado
        int choice = InteractiveMenu.Show(
            new InteractiveMenu.MenuArgs
            {
                MenuTitle = $"Producto '{selectedProduct.Name}'",
                Choices =
                [
                    "Actualizar Producto",
                    "Eliminar Producto",
                    "Volver al listado de productos",
                ],
            }
        );

        // Manejar la elección del usuario
        switch (choice)
        {
            case -1:
            case 2:
                return; // Volver al menu de productos
            case 0:
                HandleUpdateProduct(selectedProduct);
                break;
            case 1:
                _controller.DeleteProduct(selectedProduct.Id);
                break;
        }
    }

    // Maneja la actualización de un producto
    private void HandleUpdateProduct(Product product)
    {
        // Argumentos de cada funcion de input requerida
        Input.ReadRequiredStrArgs strArgs = new Input.ReadRequiredStrArgs { AllowEmpty = true };
        Input.ReadRequiredDecArgs decArgs = new Input.ReadRequiredDecArgs { AllowEmpty = true };
        Input.ReadRequiredIntArgs intArgs = new Input.ReadRequiredIntArgs { AllowEmpty = true };

        // Pedir nuevo nombre
        string newName = Input.ReadRequiredStr(
            "Nuevo nombre (dejar en blanco para no cambiar): ",
            strArgs
        );

        // Pedir nuevo precio
        decimal? newPrice = Input.ReadRequiredDec(
            "Nuevo precio (dejar en blanco para no cambiar): ",
            decArgs
        );

        // Pedir nuevo stock
        int? newStock = Input.ReadRequiredInt(
            "Nuevo stock (dejar en blanco para no cambiar): ",
            intArgs
        );

        // Si el usuario dejo en blanco, mantenemos valores anteriores
        string name = string.IsNullOrWhiteSpace(newName) ? product.Name : newName;
        decimal price = newPrice ?? product.Price;
        int stock = newStock ?? product.Stock;

        // Actualizamos el producto
        _controller.UpdateProduct(product.Id, name, price, stock);
    }
}
