using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Controllers;

public class ProductController
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    public Product CreateProduct(string name, decimal price, int stock)
    {
        return _service.CreateProduct(name, price, stock);
    }

    public Product? GetProductById(int productId)
    {
        return _service.GetProductById(productId);
    }

    public List<Product> GetAllProducts()
    {
        return _service.GetAllProducts();
    }

    public void UpdateProduct(int productId, string? name, decimal? price, int? stock)
    {
        _service.UpdateProduct(productId, name, price, stock);
    }

    public void DeleteProduct(int productId)
    {
        _service.DeleteProduct(productId);
    }
}
