using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    // Constructor que recibe el repositorio como dependencia asi mantenemos bajo acoplamiento
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    // Funcion helper para obtener el siguiente ID disponible
    private int GetNextId()
    {
        // Obtenemos todos los productos actuales
        var products = _repository.FindAll();

        // Si la lista esta vacia, empezamos en 1
        if (products.Count == 0)
            return 1;

        // Si hay productos, buscamos el ID mas alto y le sumamos 1
        return products.Max(p => p.Id) + 1;
    }

    public Product CreateProduct(string name, decimal price, int stock)
    {
        // Obtenemos el siguiente ID disponible
        var id = GetNextId();

        // Creamos el nuevo producto
        var product = new Product(id, name, price, stock);

        // Usamos el repositorio para guardar el producto
        _repository.Create(product);

        return product;
    }

    public Product? GetProductById(int productId)
    {
        return _repository.FindById(productId);
    }

    public List<Product> GetAllProducts()
    {
        return _repository.FindAll();
    }

    public void UpdateProduct(int productId, string? name, decimal? price, int? stock)
    {
        var existingProduct = _repository.FindById(productId);

        // Verificamos que el producto exista
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Producto no encontrado.");
        }

        // Actualizamos los valores del producto existente segun los parametros proporcionados
        UpdateProductValues(existingProduct, name, price, stock);

        // Usamos el repositorio para actualizar el producto por ID
        _repository.UpdateById(productId, existingProduct);
    }

    private static void UpdateProductValues(
        Product product,
        string? name,
        decimal? price,
        int? stock
    )
    {
        // Actualizamos solo si el nomnbre no es nulo o vacio
        if (!string.IsNullOrEmpty(name))
        {
            product.UpdateName(name);
        }

        // Actualizamos solo si el precio tiene valor valido
        if (price.HasValue)
        {
            product.UpdatePrice(price.Value);
        }

        // Actualizamos solo si el stock tiene valor valido
        if (stock.HasValue)
        {
            // Ajustamos el stock al valor proporcionado
            var currentStock = product.Stock;
            var newStock = stock.Value;

            if (newStock > currentStock)
            {
                product.IncreaseStock(newStock - currentStock);
            }
            else if (newStock < currentStock)
            {
                product.DecreaseStock(currentStock - newStock);
            }
        }
    }

    public void DeleteProduct(int productId)
    {
        var existingProduct = _repository.FindById(productId);

        // Verificamos que el producto exista
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Producto no encontrado.");
        }

        _repository.DeleteById(productId);
    }
}
