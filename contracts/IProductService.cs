using InvoiceApp.Domain;

namespace InvoiceApp.Contracts;

public interface IProductService
{
    Product CreateProduct(string name, decimal price, int stock);
    Product? GetProductById(int productId);
    List<Product> GetAllProducts();
    void UpdateProduct(int productId, string? name, decimal? price, int? stock);
    void DeleteProduct(int productId);
}
