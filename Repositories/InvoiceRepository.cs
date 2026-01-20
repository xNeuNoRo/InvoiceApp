using System.Text.Json;
using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly string _filePath = "invoices.json";

    public InvoiceRepository()
    {
        if (!File.Exists(_filePath))
        {
            // Si el archivo no existe, lo creamos con un array vacio
            File.WriteAllText(_filePath, "[]");
        }
    }

    // Funcion helper para leer y deserializar el JSON
    private List<Invoice> ReadFile()
    {
        // Leemos el contenido del archivo JSON
        var json = File.ReadAllText(_filePath);

        // Si el archivo esta vacio, devolvemos una lista vacia
        if (string.IsNullOrWhiteSpace(json))
            return new List<Invoice>();

        // Deserializamos el JSON a una lista de facturas
        return JsonSerializer.Deserialize<List<Invoice>>(json) ?? new List<Invoice>();
    }

    // Funcion helper para serializar y guardar el JSON
    private void SaveFile(List<Invoice> invoices)
    {
        // La opcion es simplemente para indentar debidamente el JSON
        var options = new JsonSerializerOptions { WriteIndented = true };

        // Serializamos la lista de facturas a JSON
        var json = JsonSerializer.Serialize(invoices, options);

        // Guardamos el JSON en el archivo
        File.WriteAllText(_filePath, json);
    }

    public void Create(Invoice invoice)
    {
        // Leemos las facturas actuales
        var invoices = ReadFile();
        // Agregamos la nueva factura a la lista existente
        invoices.Add(invoice);
        // Finalmente guardamos la lista actualizada
        SaveFile(invoices);
    }

    public void UpdateById(int invoiceId, Invoice updatedInvoice)
    {
        // Leemos las facturas actuales
        var invoices = ReadFile();
        // Buscamos la factura por ID
        var index = invoices.FindIndex(inv => inv.Id == invoiceId);

        if (index != -1)
        {
            // Si la encontramos, la actualizamos
            invoices[index] = updatedInvoice;
            // Guardamos la lista actualizada
            SaveFile(invoices);
        }
    }

    public void DeleteById(int invoiceId)
    {
        // Leemos las facturas actuales
        var invoices = ReadFile();

        // Filtramos la lista para eliminar la factura con el ID especificado
        invoices = invoices.Where(inv => inv.Id != invoiceId).ToList();

        // Guardamos la lista actualizada
        SaveFile(invoices);
    }

    public Invoice? FindById(int invoiceId)
    {
        // Leemos las facturas actuales
        var invoices = ReadFile();

        // Buscamos y devolvemos la factura con el ID especificado
        return invoices.FirstOrDefault(inv => inv.Id == invoiceId);
    }

    public List<Invoice> FindAll()
    {
        // Leemos y devolvemos todas las facturas
        return ReadFile();
    }
}
