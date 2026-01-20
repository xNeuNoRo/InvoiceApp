namespace InvoiceApp.Domain;

public class InvoiceItem
{
    public Product Product { get; private set; }
    public int Quantity { get; private set; }

    // Precio del producto
    public decimal UnitPrice => Product.Price;

    // Subtotal calculado dependiendo de la cantidad
    public decimal Subtotal => CalculateSubtotalPrice();

    public InvoiceItem(Product product, int quantity)
    {
        // Validamos los parámetros antes de crear el ítem de factura
        ValidateCreationParams(product, quantity);
        Product = product;
        Quantity = quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
        // Validamos la nueva cantidad
        if (newQuantity <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a 0");
        }

        Quantity = newQuantity;
    }

    public void UpdateProduct(Product newProduct)
    {
        // Validamos el nuevo producto
        if (newProduct == null)
        {
            throw new ArgumentException("El producto no puede ser nulo");
        }

        Product = newProduct;
    }

    private decimal CalculateSubtotalPrice()
    {
        return Product.Price * Quantity;
    }

    private static void ValidateCreationParams(Product product, int quantity)
    {
        // Validamos el producto
        if (product == null)
        {
            throw new ArgumentException("El producto no puede ser nulo");
        }

        // Validamos la cantidad
        if (quantity <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a 0");
        }
    }
}
