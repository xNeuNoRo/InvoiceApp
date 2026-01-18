namespace InvoiceApp.Domain;

public class Invoice
{
    public int Id { get; private set; }
    public DateTime Date { get; private set; } = DateTime.Now;
    public List<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();
    public decimal Subtotal => CalculateSubtotalPrice();
    public decimal Tax { get; set; } = 0.15m; // Impuesto por defecto del 15%
    public decimal Total => CalculateTotalPrice();

    

    public Invoice(int id)
    {
        // Validamos el ID antes de crear la factura
        if (id <= 0)
        {
            throw new ArgumentException("El ID de la factura debe ser mayor a 0");
        }

        Id = id;
    }

    private decimal CalculateSubtotalPrice()
    {
        return Items.Sum(item => item.Subtotal);
    }

    private decimal CalculateTotalPrice()
    {
        return Subtotal + (Subtotal * Tax);
    }

    public void AddItem(InvoiceItem item)
    {
        // Validamos el ítem antes de agregarlo a la factura
        if (item == null)
        {
            throw new ArgumentException("El ítem de la factura no puede ser nulo");
        }

        Items.Add(item);
    }

    public void RemoveItem(InvoiceItem item)
    {
        // Validamos el ítem antes de removerlo de la factura
        if (item == null)
        {
            throw new ArgumentException("El ítem de la factura no puede ser nulo");
        }

        Items.Remove(item);
    }

    public void ClearItems()
    {
        Items.Clear();
    }
}
