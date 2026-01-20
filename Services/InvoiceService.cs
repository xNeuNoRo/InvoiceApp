using InvoiceApp.Contracts;
using InvoiceApp.Domain;

namespace InvoiceApp.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductRepository _productRepository;

    // Constructor que recibe el repositorio como dependencia asi mantenemos bajo acoplamiento
    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IProductRepository productRepository
    )
    {
        _invoiceRepository = invoiceRepository;
        _productRepository = productRepository;
    }

    // Funcion helper para obtener el siguiente ID disponible
    private int GetNextId()
    {
        // Obtenemos todas las facturas actuales
        var invoices = _invoiceRepository.FindAll();

        // Si la lista esta vacia, empezamos en 1
        if (invoices.Count == 0)
            return 1;

        // Si hay facturas, buscamos el ID mas alto y le sumamos 1
        return invoices.Max(i => i.Id) + 1;
    }

    public Invoice CreateInvoice()
    {
        // Obtenemos el siguiente ID disponible
        var id = GetNextId();

        // Creamos la nueva factura
        var invoice = new Invoice(id);

        // Usamos el repositorio para guardar la factura
        _invoiceRepository.Create(invoice);

        return invoice;
    }

    public void AddProductToInvoice(int invoiceId, int productId, int quantity)
    {
        // Obtenemos la factura y el producto
        var invoice = _invoiceRepository.FindById(invoiceId);
        var product = _productRepository.FindById(productId);

        // Verificamos que existan
        if (invoice == null)
            throw new InvalidOperationException("Factura no encontrada");
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado");

        // Decrementar el stock del producto
        product.DecreaseStock(quantity);

        // Creamos el item de la factura y lo agregamos a la misma
        var item = new InvoiceItem(product, quantity);
        invoice.AddItem(item);

        // Actualizamos la factura y el producto en sus respectivos repositorios
        _invoiceRepository.UpdateById(invoiceId, invoice); // La factura por su nuevo item agregado
        _productRepository.UpdateById(productId, product); // Y el producto por su stock modificado
    }

    public void ModifyProductQuantityFromInvoice(int invoiceId, int productId, int newQuantity)
    {
        // Obtenemos la factura y el producto
        var invoice = _invoiceRepository.FindById(invoiceId);
        var product = _productRepository.FindById(productId);

        // Verificamos que existan
        if (invoice == null)
            throw new InvalidOperationException("Factura no encontrada");
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado");

        // Buscamos el item correspondiente en la factura
        var item = invoice.Items.FirstOrDefault(invoiceItem => invoiceItem.Product.Id == productId);
        if (item == null)
            throw new InvalidOperationException("El producto no se encuentra en la factura");

        // Calculamos la diferencia de cantidad
        int quantityDifference = newQuantity - item.Quantity;

        // Ajustamos el stock del producto segun la diferencia
        if (quantityDifference > 0)
        {
            product.DecreaseStock(quantityDifference);
        }
        else if (quantityDifference < 0)
        {
            product.IncreaseStock(-quantityDifference);
        }

        // Modificamos la cantidad del item en la factura
        item.UpdateQuantity(newQuantity);
        item.UpdateProduct(product); // Para sincronizar el stock del producto en el item

        // Actualizamos la factura y el producto en sus respectivos repositorios
        _invoiceRepository.UpdateById(invoiceId, invoice); // La factura por el item modificado
        _productRepository.UpdateById(productId, product); // Y el producto por su stock modificado
    }

    public void RemoveProductFromInvoice(int invoiceId, int productId)
    {
        // Obtenemos la factura y el producto
        var invoice = _invoiceRepository.FindById(invoiceId);
        var product = _productRepository.FindById(productId);

        // Verificamos que existan
        if (invoice == null)
            throw new InvalidOperationException("Factura no encontrada");
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado");

        // Buscamos el item correspondiente en la factura
        var item = invoice.Items.FirstOrDefault(invoiceItem => invoiceItem.Product.Id == productId);
        if (item == null)
            throw new InvalidOperationException("El producto no se encuentra en la factura");

        // Incrementar el stock del producto
        product.IncreaseStock(item.Quantity);

        // Removemos el item de la factura
        invoice.RemoveItem(item);

        // Actualizamos la factura y el producto en sus respectivos repositorios
        _invoiceRepository.UpdateById(invoiceId, invoice); // La factura por el item removido
        _productRepository.UpdateById(productId, product); // Y el producto por su stock modificado
    }

    public Invoice? GetInvoiceById(int invoiceId)
    {
        return _invoiceRepository.FindById(invoiceId);
    }

    public List<Invoice> GetAllInvoices()
    {
        return _invoiceRepository.FindAll();
    }

    public Product? GetProductById(int productId)
    {
        return _productRepository.FindById(productId);
    }

    public List<Product> GetAllProducts()
    {
        return _productRepository.FindAll();
    }

    public void DeleteInvoice(int invoiceId)
    {
        var existingInvoice = _invoiceRepository.FindById(invoiceId);

        // Verificamos que la factura exista
        if (existingInvoice == null)
        {
            throw new InvalidOperationException("Factura no encontrada.");
        }

        // Eliminamos la factura usando el repositorio
        _invoiceRepository.DeleteById(invoiceId);
    }
}
