using InvoiceApp.Helpers;

namespace InvoiceApp;

public class App
{
    public App()
    {
        var selectedChoice = InteractiveMenu.Show(
            new InteractiveMenu.InteractiveMenuParams
            {
                MenuTitle = "Seleccione una opcion:",
                Choices = ["Crear factura", "Ver facturas", "Salir"],
                RowsPerPage = 5,
            }
        );
        Console.WriteLine($"Opcion seleccionada: {selectedChoice}");
    }
}
