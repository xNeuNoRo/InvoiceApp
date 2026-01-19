namespace InvoiceApp.Helpers;

public static class Array
{
    // Copia los elementos de un array en otro nuevo
    public static T[] Copy<T>(T[] originalArr, T[] copyArr)
    {
        for (int i = 0; i < originalArr.Length; i++)
        {
            copyArr[i] = originalArr[i];
        }

        return copyArr;
    }

    // Agrega un elemento al final de un array (expandiendo su tamaño)
    public static void Push<T>(ref T[] arr, T value)
    {
        T[] newArr = new T[arr.Length + 1];
        T[] copyNewArr = Copy(arr, newArr);

        copyNewArr[arr.Length] = value;

        arr = copyNewArr;
    }

    // Obtener el valor maximo de un array de enteros
    public static int GetMax(int[] numbers)
    {
        int max = numbers[0];

        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
                max = numbers[i];
        }

        return max;
    }

    // Mapear un array a otro usando una funcion flecha (tipo JavaScript)
    public static TResult[] Map<TInput, TResult>(TInput[] inputArray, Func<TInput, TResult> arrowFn)
    {
        // Validaciones basicas
        if (inputArray == null || arrowFn == null)
            return [];

        // Crear el array resultado con el mismo tamaño que el de entrada
        TResult[] result = new TResult[inputArray.Length];

        // Recorrer el array de entrada y aplicar la funcion flecha a cada elemento
        for (int i = 0; i < inputArray.Length; i++)
        {
            result[i] = arrowFn(inputArray[i]); // Guardando su resultado en el array resultado
        }

        return result;
    }

    // Tomar los primeros N elementos de un array desde un indice inicial
    public static T[] TakeFirstN<T>(T[] array, int start, int limit)
    {
        // Validaciones iniciales
        if (array == null || array.Length == 0 || start >= array.Length || limit < 1)
            return [];

        // Ajustar el limite si es mayor que la longitud del array
        if (limit > array.Length)
            limit = array.Length;

        // Crear el nuevo array
        T[] newArray = [];

        // Agregar los elementos del rango especificado al nuevo array
        for (int i = start; i < limit; i++)
        {
            Push(ref newArray, array[i]); // Usando el metodo Push para agregar elementos
        }

        return newArray;
    }
}
