using System.Text;

namespace InvoiceApp.Helpers;

public static class String
{
    public static string[] Split(string stringToSplit, char separator)
    {
        // Declaramos el acumulador del string, el array que guardara los caracteres y el index inicial
        StringBuilder stringAccumulator = new StringBuilder();
        string[] stringArr = [];

        // Comprobamos todos los caracteres del string uno por uno
        for (int i = 0; i < stringToSplit.Length; i++)
        {
            char actualChar = stringToSplit[i];
            // Si el caracter es igual al separador,
            // guardamos el string acumulado en el array, reseteamos el acumulador y aumentamos el index del array
            if (separator == actualChar)
            {
                Array.Push(ref stringArr, stringAccumulator.ToString()); // Agregamos el string acumulado al array
                stringAccumulator.Clear(); // Reseteamos el acumulador
            }
            // En el caso contrario seguimos almacenando los caracteres en el acumulador
            else
            {
                stringAccumulator.Append(actualChar);
            }
        }

        // Agrego el ultimo string acumulado
        Array.Push(ref stringArr, stringAccumulator.ToString());

        return stringArr;
    }

    // Rellenar un string por la izquierda hasta una longitud dada, con un caracter dado (por defecto espacio)
    public static string fillLeft(string strInput, int spacesToFill, char charToAdd = ' ')
    {
        // Si el string ya es mayor o igual al tamaño requerido, retornarlo tal cual
        if (strInput.Length >= spacesToFill)
            return strInput;

        // Calcular los espacios a llenar
        int realSpacesToFill = spacesToFill - strInput.Length;

        // Construir el nuevo string
        StringBuilder strAcc = new StringBuilder();

        // Iterar y agregar los caracteres al inicio
        for (int i = realSpacesToFill; i != 0; i--)
        {
            strAcc.Append(charToAdd);
        }

        // Agregamos el string original al final
        strAcc.Append(strInput);

        // Retornamos el string resultante
        return strAcc.ToString();
    }

    // Rellenar un string por la derecha hasta una longitud dada, con un caracter dado (por defecto espacio)
    public static string fillRight(string strInput, int spacesToFill, char charToAdd = ' ')
    {
        // Si el string ya es mayor o igual al tamaño requerido, retornarlo tal cual
        if (strInput.Length >= spacesToFill)
            return strInput;

        // Calcular los espacios a llenar
        int realSpacesToFill = spacesToFill - strInput.Length;

        // Construir el nuevo string
        StringBuilder strAcc = new StringBuilder(strInput);

        // Iterar y agregar los caracteres al final
        for (int i = realSpacesToFill; i != 0; i--)
        {
            strAcc.Append(charToAdd);
        }

        // Retornamos el string resultante
        return strAcc.ToString();
    }

    // Obtener un "trozo" de un string dado un indice inicial y un indice final opcional
    public static string Substring(string strInput, int startIndex, int? expectedEndIndex = null)
    {
        // Validaciones basicas
        if (string.IsNullOrWhiteSpace(strInput))
            return "";

        // Si no se pasa el indice final, se toma la longitud del string
        int endIndex = expectedEndIndex ?? strInput.Length;

        // Validaciones basicas de los indices
        if (startIndex < 0 || endIndex > strInput.Length || startIndex >= endIndex)
            return "";

        // Si llegamos hasta aqui, quiere decir q es un substring valido.

        // Creamos un array de chars para almacenar el substring
        int expectedLength = endIndex - startIndex;
        char[] result = new char[expectedLength];

        // Rellenamos el array de chars con los caracteres del string original
        for (int i = 0; i < expectedLength; i++)
        {
            result[i] = strInput[startIndex + i];
        }

        // Finalmente, retornamos el nuevo string creado a partir del array de chars
        return new string(result);
    }
}
