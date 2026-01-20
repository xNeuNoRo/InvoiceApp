using System.Text.Json;
using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Repositories;

public class ProductRepository : IProductRepository
{
    // Guardamos la ruta del archivo JSON en una constante
    private readonly string _filePath = "products.json";

    public ProductRepository()
    {
        if (!File.Exists(_filePath))
        {
            // Si el archivo no existe, lo creamos con un array vacio
            File.WriteAllText(_filePath, "[]");
        }
    }

    // Funcion helper para leer y deserializar el JSON
    private List<Product> ReadFile()
    {
        // Leemos el contenido del archivo JSON
        var json = File.ReadAllText(_filePath);

        // Si el archivo esta vacio, devolvemos una lista vacia
        if (string.IsNullOrWhiteSpace(json))
            return new List<Product>();

        // Deserializamos el JSON a una lista de productos
        return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }

    // Funcion helper para serializar y guardar el JSON
    private void SaveFile(List<Product> products)
    {
        // La opcion es simplemente para indentar debidamente el JSON
        var options = new JsonSerializerOptions { WriteIndented = true };

        // Serializamos la lista de productos a JSON
        var json = JsonSerializer.Serialize(products, options);

        // Guardamos el JSON en el archivo
        File.WriteAllText(_filePath, json);
    }

    public void Create(Product product)
    {
        // Leemos los productos actuales
        var products = ReadFile();
        // Agregamos el nuevo producto a la lista existente
        products.Add(product);
        // Finalmente guardamos la lista actualizada
        SaveFile(products);
    }

    public void UpdateById(int productId, Product updatedProduct)
    {
        // Leemos los productos actuales
        var products = ReadFile();
        // Buscamos el indice del producto a actualizar
        var index = products.FindIndex(p => p.Id == productId);

        if (index == -1)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        // Actualizamos el producto en la lista
        products[index] = updatedProduct;
        // Finalmente guardamos la lista actualizada
        SaveFile(products);
    }

    public void DeleteById(int productId)
    {
        // Leemos los productos actuales
        var products = ReadFile();

        // Filtramos la lista para eliminar el producto con el ID especificado
        products = products.Where(p => p.Id != productId).ToList();

        // Finalmente guardamos la lista actualizada
        SaveFile(products);
    }

    public Product? FindById(int productId)
    {
        // Leemos los productos actuales
        var products = ReadFile();
        // Buscamos y devolvemos el producto por su ID
        return products.FirstOrDefault(p => p.Id == productId);
    }

    public List<Product> FindAll()
    {
        // Leemos y devolvemos todos los productos
        return ReadFile();
    }

    public Product FindByName(string productName)
    {
        // Leemos los productos actuales
        var products = ReadFile();
        // Buscamos y devolvemos el producto por su nombre
        var product = products.FirstOrDefault(p =>
            p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) // Ignoramos mayusculas/minusculas
        );

        if (product == null)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        return product;
    }
}
