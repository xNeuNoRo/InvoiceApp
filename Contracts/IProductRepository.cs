using InvoiceApp.Domain;

namespace InvoiceApp.Contracts;

public interface IProductRepository
{
    void Create(Product product);
    void UpdateById(int productId, Product updatedProduct);
    void DeleteById(int productId);
    Product? FindById(int productId);
    List<Product> FindAll();
    Product FindByName(string productName);
}