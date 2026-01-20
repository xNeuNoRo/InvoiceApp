namespace InvoiceApp.Helpers;

public static class InteractiveMenu
{
    private static bool ShouldHideHints = false;

    private static void DrawInteractiveMenu(
        string menuTitle,
        string[] choices,
        int selectedIndex,
        int width
    )
    {
        Console.Clear();

        // Dividimos el ancho en dos mitades, izquierda y derecha
        int leftHalf = width / 2;
        int rightHalf = width - leftHalf;

        // Dividimos el titulo del menu en lineas por si tiene saltos de linea
        string[] menuTitleSplitted = String.Split(menuTitle, '\n');

        // Calculamos el maximo largo entre el titulo y las opciones
        int maxTitleLength = Array.GetMax(Array.Map(menuTitleSplitted, title => title.Length));
        int maxChoicesLength = Array.GetMax(Array.Map(choices, c => c.Length));

        // Decidimos cual es el maximo largo entre los dos
        int maxLengthInMenu =
            maxTitleLength >= maxChoicesLength ? maxTitleLength : maxChoicesLength;

        // Calculamos las tabulaciones dependiendo del maximo largo calculado previamente
        int calculatedTabsVal = 4; // Default 4 tabs

        // Ajuste dinamico de las tabs dependiendo del largo del menu calculado previamente
        if (maxLengthInMenu > 70)
            calculatedTabsVal = 2;
        else if (maxLengthInMenu > 60)
            calculatedTabsVal = 3;

        // Simplemente rellenamos con las tabs calculadas. (Con una tab ya por default que incluye el menu)
        // Serian por ej: 4+1 o 3+1 o 2+1 tabs dependiendo del largo del menu
        string calculatedTabs = String.fillRight("\t", calculatedTabsVal, '\t');

        // Espacios para alinear el menu
        Console.WriteLine("\n\n");
        // Ajustamos el background del menu
        Console.BackgroundColor = ConsoleColor.Black;

        // Imprimimos el borde izquierdo del menu de color azul
        WriteColorLines(
            $"{calculatedTabs}╭" + String.fillRight("━", leftHalf, '━'),
            ConsoleColor.Blue
        );
        // Imprimimos el borde derecho del menu de color rojo
        WriteColorLines(String.fillRight("━", rightHalf, '━') + "╮", ConsoleColor.Red);

        // Iteramos entre todos los posibles saltos de linea del titulo del menu
        foreach (string menuTitleParsed in menuTitleSplitted)
        {
            // Si el titulo no es vacio
            if (!string.IsNullOrWhiteSpace(menuTitleParsed))
            {
                // Centramos el texto
                string centeredText = CenterText(menuTitleParsed, width);

                // Dividimos el texto en dos mitades para colorearlas diferente
                int splitIndex = centeredText.Length / 2;

                // Imprimimos la primera mitad en azul y la segunda en rojo
                WriteColorLines(
                    $"\n{calculatedTabs}┃" + String.Substring(centeredText, 0, splitIndex),
                    ConsoleColor.Blue
                );
                WriteColorLines(String.Substring(centeredText, splitIndex) + "┃", ConsoleColor.Red);
            }
            else
            {
                // Si el titulo es vacio, imprimimos una linea vacia con los bordes
                WriteColorLines(
                    String.fillRight($"\n{calculatedTabs}┃", width + 2),
                    ConsoleColor.Blue
                );
                WriteColorLines(String.fillRight("┃", width + 2), ConsoleColor.Red);
            }
        }

        // Imprimimos el footer del titulo, dividiendolo en dos mitades para colorearlas diferente
        WriteColorLines(
            $"\n{calculatedTabs}┃" + String.fillRight("━", leftHalf, '━'),
            ConsoleColor.Blue
        );
        WriteColorLines(String.fillRight("━", rightHalf, '━') + "┃\n", ConsoleColor.Red);

        // Iteramos entre todas las opciones
        for (int i = 0; i < choices.Length; i++)
        {
            // Si es la opcion la cual esta haciendo "hover" el selector
            if (i == selectedIndex)
            {
                // Cambiamos los colores para resaltar la opcion seleccionada
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = Color.Selector;
            }
            // En caso contrario
            else
            {
                // Restauramos los colores por defecto
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }

            // Imprimimos la opcion centrada dentro del menu
            Console.WriteLine($"{calculatedTabs}┃" + CenterText(choices[i], width) + "┃");
        }

        // Imprimimos el borde inferior del menu
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine($"{calculatedTabs}╰" + String.fillRight("━", width, '━') + "╯");
        Console.ResetColor();
    }

    // Simple helper para evitar algo de boilerplate al escribir en colores
    private static void WriteColorLines(string str, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(str);
    }

    // Clase para pasar argumentos al menu interactivo
    public class MenuArgs
    {
        public string MenuTitle { get; set; } = "";
        public string[] Choices { get; set; } = [];
        public string[][]? Pages { get; set; } = null;
        public int CurrentPage { get; set; } = 0;
        public int RowsPerPage { get; set; } = 10;
        public bool IsMainMenu { get; set; } = false;
    }

    // Metodo principal para mostrar el menu interactivo
    public static int Show(MenuArgs args)
    {
        if (args is not { MenuTitle: var title, Choices: var choices })
            return -999;

        int width = CalculateAutoWidth(title, choices);
        int selectedIndex = 0;
        bool isPagination = args.Pages != null;
        int totalPages = args.Pages?.Length ?? 0;

        while (true)
        {
            // Dibujamos el menu interactivo
            DrawInteractiveMenu(title, choices, selectedIndex, width);

            // Imprimimos el footer
            PrintMenuFooterInfo(args.CurrentPage, totalPages, isPagination);

            // Esperamos a que el usuario presione una tecla
            var navParams = new KeysArgs
            {
                Key = Console.ReadKey(true).Key,
                SelectedIndex = selectedIndex,
                Choices = choices,
                Pagination = isPagination,
                TotalPages = totalPages,
                CurrentPage = args.CurrentPage,
                RowsPerPage = args.RowsPerPage,
                IsMainMenu = args.IsMainMenu,
            };

            // Manejamos la tecla presionada
            int? result = HandleInteractiveKeys(navParams);

            // Actualizamos el indice seleccionado
            selectedIndex = navParams.SelectedIndex;

            // Si se selecciono una opcion valida, la retornamos
            if (result != null)
                return result.Value;
        }
    }

    // Calcula el ancho automatico del menu basado en el titulo y las opciones
    private static int CalculateAutoWidth(string title, string[] choices)
    {
        // El ancho minimo sera 50
        int width = 50;

        // Obtener el maximo largo entre las opciones
        int maxChoiceLen = Array.GetMax(Array.Map(choices, c => c.Length));

        // Dividimos el titulo en lineas por si tiene saltos de linea
        string[] splittedTitle = String.Split(title, '\n');
        // Mapeamos a un nuevo array de enteros con el Length de cada linea
        int[] titleLengths = Array.Map(splittedTitle, t => t.Length);
        // Obtener el maximo largo entre las lineas del titulo
        int maxTitleLen = Array.GetMax(titleLengths);

        // Calculamos el ancho requerido
        int requiredWidth = Math.Max(maxTitleLen, maxChoiceLen) + 4;

        // 5 seria el margen de seguridad para evitar desalineados raros
        if (width - 5 <= requiredWidth)
        {
            width = requiredWidth;
        }

        // Retornamos el ancho calculado
        return width;
    }

    private static void PrintMenuFooterInfo(int currentPage, int totalPages, bool isPagination)
    {
        if (isPagination)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Pagina {currentPage + 1}/{totalPages}");
        }

        if (ShouldHideHints)
            return;

        Console.ForegroundColor = Color.Warning;
        string navKeys = isPagination ? "[↑][←][↓][→] / [W][A][S][D]" : "[↑][↓] / [W][S]";

        Console.WriteLine($"\n\t\t\tNavegacion: {navKeys}");
        Console.WriteLine("\t\t\tInteraccion: [Espacio] / [⏎] Enter");

        Console.ForegroundColor = Color.Success;
        Console.WriteLine("\n\t\t\tPresiona [H] para mostrar/ocultar las sugerencias.");
        Console.ResetColor();
    }

    // Clase para pasar argumentos al handler de teclas
    public class KeysArgs
    {
        public ConsoleKey Key { get; init; }
        public int SelectedIndex { get; set; }
        public string[] Choices { get; init; } = [];
        public bool Pagination { get; init; }
        public int TotalPages { get; init; }
        public int CurrentPage { get; init; }
        public int RowsPerPage { get; init; }
        public bool IsMainMenu { get; init; }
    }

    // Handler de las teclas interactivas
    public static int? HandleInteractiveKeys(KeysArgs args)
    {
        // Si los args son nulos, retornamos null
        if (args == null)
            return null;

        return args.Key switch
        {
            // Navegacion Vertical
            ConsoleKey.W or ConsoleKey.UpArrow => HandleVerticalMove(args, -1),
            ConsoleKey.S or ConsoleKey.DownArrow => HandleVerticalMove(args, 1),

            // Navegacion Horizontal (Solo Paginacion)
            ConsoleKey.A or ConsoleKey.LeftArrow when args.Pagination => HandlePageMove(args, -1), // -1 le indicara que es izquierda
            ConsoleKey.D or ConsoleKey.RightArrow when args.Pagination => HandlePageMove(args, 1), // 1 le indicara que es derecha

            // Acciones de Confirmacion / Salida / Hints
            ConsoleKey.Spacebar or ConsoleKey.Enter => HandleConfirm(args),
            ConsoleKey.Escape => HandleExit(args),
            ConsoleKey.H => ToggleHints(),

            // Default: ignorar cualquier otra tecla
            _ => null,
        };
    }

    // Handler de movimiento vertical
    private static int? HandleVerticalMove(KeysArgs args, int direction)
    {
        // direction: -1 (Arriba), 1 (Abajo)
        // Si direction es negativo, movemos hacia arriba
        if (direction < 0)
            args.SelectedIndex =
                (args.SelectedIndex == 0) ? args.Choices.Length - 1 : args.SelectedIndex - 1;
        // Si direction es positivo, movemos hacia abajo
        else
            args.SelectedIndex =
                (args.SelectedIndex == args.Choices.Length - 1) ? 0 : args.SelectedIndex + 1;

        // Sino se selecciono ninguna opcion valida, retornamos null para seguir el flujo normal
        return null;
    }

    // Handler de cambio de pagina
    private static int? HandlePageMove(KeysArgs args, int direction)
    {
        // direction: -1 (Izquierda), 1 (Derecha) => retorna -2 (Izquierda) o -3 (Derecha)
        int moveToDirection = direction < 0 ? -2 : -3;

        // Si hay mas de una pagina, retornamos codigos especiales para que el padre maneje el cambio
        return args.TotalPages > 1 ? moveToDirection : null;
    }

    // Handler de la confirmacion de seleccion
    private static int HandleConfirm(KeysArgs args)
    {
        // Limpiamos buffer por si acaso
        while (Console.KeyAvailable)
            Console.ReadKey(true);
        Console.Clear();

        // Retornamos el indice seleccionado, ajustado si es paginado
        return args.Pagination
            ? (args.CurrentPage * args.RowsPerPage) + args.SelectedIndex
            : args.SelectedIndex;
    }

    // Handler de la salida del menu
    private static int? HandleExit(KeysArgs args)
    {
        // Si no es menú principal o es paginación simple, salimos directo (-1)
        if (!args.IsMainMenu || args.Pagination)
            return -1;

        // Confirmacion antes de cerrar el programa
        int selectedChoice = Show(
            new MenuArgs
            {
                MenuTitle = "Estas seguro que deseas salir?",
                Choices = ["Si, deseo salir.", "No, no quiero salir ahora."],
            }
        );

        if (selectedChoice == 0)
        {
            Console.Clear();
            return -1;
        }

        // Si llego hasta aqui, quiere decir que cancelo la salida.
        return null;
    }

    // Handler de toggle de hints
    private static int? ToggleHints()
    {
        // Alternamos el estado de los hints
        ShouldHideHints = !ShouldHideHints;
        return null;
    }

    // Helper para paginar un array
    public static T[] GetPagination<T>(T[] array, int page = 1, int rowsPerPage = 15)
    {
        // Calculamos el total de paginas
        int totalPages = array.Length / rowsPerPage;

        // Si hay sobran valores, agregamos una pagina mas para mostrarlos
        if (array.Length % rowsPerPage != 0)
            totalPages++;

        // Validamos los parametros
        if (array == null || array.Length == 0 || page < 1 || page > totalPages)
            return [];

        // Si la pagina es 1, el offset es 0, si es 2, el offset es rowsPerPage, etc.
        // De esa forma con la utilidad TakeFirstN podemos tomar los valores correctos desde N hasta M indice del array
        int offset = rowsPerPage * (page - 1);

        return Array.TakeFirstN(array, offset, offset + rowsPerPage);
    }

    // Helper para centrar texto dentro de un ancho dado
    private static string CenterText(string textToCenter, int widthOfMenu)
    {
        // Como funciona?
        // "               Hola mundo               " => 40 caracteres en total
        // "Hola mundo" => 10 caracteres
        // 40-10 = 30 caracteres de espacios en blanco
        // 30/2 = 15 caracteres de ambos lados en blanco
        // 40-10-caracteresIzquierda = 15 (si fuera impar se ajustaria mejor)

        // Calculamos los espacios en blanco necesarios a la izquierda y derecha
        int whiteSpacesBetweenTextLeft = (widthOfMenu - textToCenter.Length) / 2;

        // Aseguramos que no sea negativo
        if (whiteSpacesBetweenTextLeft < 0)
            whiteSpacesBetweenTextLeft = 0;

        // Calculamos los espacios en blanco a la derecha
        int whiteSpacesBetweenTextRight =
            widthOfMenu - textToCenter.Length - whiteSpacesBetweenTextLeft;

        // Rellenamos espacios en blanco a ambos lados y retornamos el texto centrado
        return String.fillRight(" ", whiteSpacesBetweenTextLeft)
            + textToCenter
            + String.fillRight(" ", whiteSpacesBetweenTextRight);
    }
}
