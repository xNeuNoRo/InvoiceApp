namespace InvoiceApp.Helpers;

public static class Input
{
    public class ReadRequiredStrArgs
    {
        public bool AllowEmpty { get; set; } = false;
    }

    // Pedir un input string valido
    public static string ReadRequiredStr(string? prompt, ReadRequiredStrArgs args)
    {
        string? strValue;

        do
        {
            // Imprimir un prompt inicial si se le pasa
            if (!string.IsNullOrEmpty(prompt))
            {
                Console.Write(prompt);
            }

            // Solicitar el string
            strValue = Console.ReadLine();

            // Validar si es vacio o nulo dependiendo del allowEmpty
            if (!args.AllowEmpty && string.IsNullOrWhiteSpace(strValue))
            {
                Console.WriteLine("Debes ingresar un valor valido!");
            }
            else
            {
                // Rompemos el bucle si es valido
                break;
            }
        } while (true);

        // Removemos cualquier posible espacio en blanco con el trim()
        return (strValue ?? string.Empty).Trim();
    }

    public class ReadRequiredIntArgs
    {
        public bool AllowEmpty { get; set; } = false;
    }

    // Pedir un input entero valido
    public static int? ReadRequiredInt(string? prompt, ReadRequiredIntArgs args)
    {
        while (true)
        {
            // Imprimir un prompt inicial si se le pasa
            if (!string.IsNullOrEmpty(prompt))
            {
                Console.Write(prompt);
            }

            string? strValue = Console.ReadLine();

            // Validar si es vacio o nulo dependiendo del allowEmpty
            if (string.IsNullOrWhiteSpace(strValue))
            {
                if (args.AllowEmpty)
                {
                    // Retorna null si se permite vacio
                    return null;
                }

                Console.WriteLine("Debes ingresar un valor valido!");
                continue;
            }

            // Intentar parsear el entero
            if (int.TryParse(strValue, out int number))
            {
                return number;
            }

            Console.WriteLine("Debes ingresar un numero entero valido.");
        }
    }

    public class ReadRequiredDecArgs
    {
        public bool AllowEmpty { get; set; } = false;
    }

    public static decimal? ReadRequiredDec(string? prompt, ReadRequiredDecArgs args)
    {
        while (true)
        {
            // Imprimir un prompt inicial si se le pasa
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt);

            string? strValue = Console.ReadLine();

            // Validar si es vacio o nulo dependiendo del allowEmpty
            if (string.IsNullOrWhiteSpace(strValue))
            {
                if (args.AllowEmpty)
                {
                    // Retorna null si se permite vacio
                    return null;
                }

                Console.WriteLine("Debes ingresar un valor valido!");
                continue;
            }

            // Intentar parsear el decimal
            if (decimal.TryParse(strValue, out decimal number))
            {
                return number;
            }

            Console.WriteLine("Debes ingresar un numero valido.");
        }
    }

    public static bool ReadRequiredBool(string? prompt)
    {
        while (true)
        {
            // Imprimir un prompt inicial si se le pasa
            if (!string.IsNullOrEmpty(prompt))
            {
                Console.Write(prompt);
            }

            // Solicitar el string, y le aplicamos trim y lo convertimos a minus
            var strValue = Console.ReadLine()?.Trim().ToLower();

            // Si dice q si, retornamos true
            if (strValue == "y")
            {
                return true;
            }

            // Si dice q no, retornamos false
            if (strValue == "n")
            {
                return false;
            }

            // SI llego hasta aqui, quiere decir que no es valido, simplemente mostramos el mensaje
            Console.WriteLine("Debes ingresar solamente 'y' o 'n'.");
        }
    }
}
