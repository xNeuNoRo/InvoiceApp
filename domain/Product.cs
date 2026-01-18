namespace InvoiceApp.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; } = 0;

    public Product(int id, string name, decimal price, int stock)
    {
        // Validamos los parametros antes de crear el producto
        ValidateCreationParams(id, name, price, stock);

        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public void DecreaseStock(int quantity)
    {
        // Comprobamos que la cantidad que se pase para decrementar sea válida
        if (quantity <= 0)
        {
            throw new ArgumentException("La cantidad para decrementar debe ser mayor a 0");
        }

        // Comprobamos que haya suficiente stock para decrementar la cantidad solicitada
        if (quantity > Stock)
        {
            throw new InvalidOperationException(
                "No hay suficiente stock para decrementar la cantidad solicitada"
            );
        }

        // Finalmente decrementamos el stock
        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        // Comprobamos que la cantidad que se pase para incrementar sea válida
        if (quantity <= 0)
        {
            throw new ArgumentException("La cantidad para incrementar debe ser mayor a 0");
        }

        // Incrementamos el stock
        Stock += quantity;
    }

    public void UpdatePrice(decimal newPrice)
    {
        // Comprobamos que el nuevo precio sea válido
        if (newPrice < 0)
        {
            throw new ArgumentException("El nuevo precio no puede ser negativo");
        }

        // Actualizamos el precio
        Price = newPrice;
    }

    public void UpdateName(string newName)
    {
        // Comprobamos que el nuevo nombre no sea nulo o vacio
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("El nuevo nombre no puede ser nulo o vacio");
        }

        // Actualizamos el nombre
        Name = newName;
    }

    private static void ValidateCreationParams(int id, string name, decimal price, int stock)
    {
        // Validamos los parámetros de creación del producto

        if (id <= 0)
        {
            throw new ArgumentException("El ID del producto debe ser mayor a 0");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del producto no puede ser nulo o vacio");
        }

        if (price < 0)
        {
            throw new ArgumentException("El precio del producto no puede ser negativo");
        }

        if (stock < 0)
        {
            throw new ArgumentException("El stock del producto no puede ser negativo");
        }
    }
}
