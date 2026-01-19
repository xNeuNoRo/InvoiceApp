namespace InvoiceApp.Helpers;

public static class Input
{
    // Pedir un input string valido
    public static string ReadRequiredStr(string? prompt)
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

            // Si no es valido (nulo o espacios en blanco) entonces le arrojamos el mensaje
            if (string.IsNullOrWhiteSpace(strValue))
            {
                Console.WriteLine("Debes ingresar un valor valido!");
            }

            // El while se encargara de reiniciarlo si es que no es valido
        } while (string.IsNullOrWhiteSpace(strValue));

        // Removemos cualquier posible espacio en blanco con el trim()
        return strValue.Trim();
    }

    // Pedir un input entero valido
    public static int ReadRequiredInt(string? prompt)
    {
        // Inicializamos un bucle infinito hasta que ingrese un num valido
        while (true)
        {
            // Imprimir un prompt inicial si se le pasa
            if (!string.IsNullOrEmpty(prompt))
            {
                Console.Write(prompt);
            }

            // Solicitar el string
            var strValue = Console.ReadLine();

            // Si es valido
            if (int.TryParse(strValue, out int number))
            {
                // Retornamos - ESto sale del bucle autom.
                return number;
            }

            // Si llego hasta aqui es pq no es valido
            Console.WriteLine("Debes ingresar un numero entero valido!");
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
