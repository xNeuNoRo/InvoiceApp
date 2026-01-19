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

    public class MenuArgs
    {
        public string MenuTitle { get; set; } = "";
        public string[] Choices { get; set; } = [];
        public string[][]? Pages { get; set; } = null;
        public int CurrentPage { get; set; } = 0;
        public int RowsPerPage { get; set; } = 10;
        public bool IsMainMenu { get; set; } = false;
    }

    public static int Show(MenuArgs args)
    {
        if (
            args
            is not {
                MenuTitle: var menuTitle,
                Choices: var choices,
                Pages: var pages,
                CurrentPage: var currentPage,
                RowsPerPage: var rowsPerPage,
                IsMainMenu: var isMainMenu
            }
        )
            return -999;

        int width = 50;
        int maxLengthChoices = Array.GetMax(Array.Map(choices, c => c.Length));
        int maxLengthMenuTitle = Array.GetMax(
            Array.Map(String.Split(menuTitle, '\n'), menuPart => menuPart.Length)
        );

        // Wao no me vuelvan a meter a diseñar menus de nuevo, despues de 2h llegue a esta conclusion:
        // width - 5 para mantener un margen de 5 y que no ocurran desalineados medio raros
        if (width - 5 <= maxLengthMenuTitle || width - 5 <= maxLengthChoices)
        {
            width =
                maxLengthMenuTitle > maxLengthChoices
                    ? maxLengthMenuTitle + 4
                    : maxLengthChoices + 4;
        }

        int selectedIndex = 0;

        while (true)
        {
            DrawInteractiveMenu(menuTitle, choices, selectedIndex, width);

            if (pages != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Pagina {currentPage + 1}/{pages.Length}");

                Console.ForegroundColor = Color.Warning;
                if (!ShouldHideHints)
                    Console.WriteLine(
                        "\n\t\t\tNavegacion: [↑][←][↓][→] / [W][A][S][D]  -  Interaccion: [Espacio] / [⏎] Enter"
                    );
                Console.ForegroundColor = Color.Success;
                Console.WriteLine(
                    "\n\t\t\tPresiona [H] para mostrar/ocultar las sugerencias de navegacion/interaccion."
                );
                Console.ResetColor();
                var navParams = new KeysArgs
                {
                    Key = Console.ReadKey(true).Key,
                    SelectedIndex = selectedIndex,
                    Choices = choices,
                    Pagination = true,
                    TotalPages = pages.Length,
                    CurrentPage = currentPage,
                    RowsPerPage = rowsPerPage,
                    IsMainMenu = isMainMenu,
                };
                int? result = HandleInteractiveKeys(navParams);
                selectedIndex = navParams.SelectedIndex;
                if (result != null)
                    return (int)result;
            }
            else
            {
                Console.ForegroundColor = Color.Warning;
                if (!ShouldHideHints)
                    Console.WriteLine(
                        "\n\t\t\tNavegacion: [↑][↓] / [W][S]\n\t\t\tInteraccion: [Espacio] / [⏎] Enter"
                    );
                Console.ForegroundColor = Color.Success;
                Console.WriteLine(
                    "\n\t\t\tPresiona [H] para mostrar/ocultar las sugerencias de navegacion/interaccion."
                );
                var navParams = new KeysArgs
                {
                    Key = Console.ReadKey(true).Key,
                    SelectedIndex = selectedIndex,
                    Choices = choices,
                    Pagination = false,
                    TotalPages = 0,
                    CurrentPage = currentPage,
                    RowsPerPage = rowsPerPage,
                    IsMainMenu = isMainMenu,
                };
                int? result = HandleInteractiveKeys(navParams);
                selectedIndex = navParams.SelectedIndex;
                if (result != null)
                    return (int)result;
            }
        }
    }

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

    public static int? HandleInteractiveKeys(KeysArgs p)
    {
        if (p == null)
            return null;

        if (p.Pagination)
        {
            switch (p.Key)
            {
                case ConsoleKey.Escape:
                {
                    return -1;
                }
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    {
                        p.SelectedIndex =
                            (p.SelectedIndex == 0) ? p.Choices.Length - 1 : p.SelectedIndex - 1;
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    {
                        p.SelectedIndex =
                            (p.SelectedIndex == p.Choices.Length - 1) ? 0 : p.SelectedIndex + 1;
                    }
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    {
                        if (p.TotalPages > 1)
                            return -2;
                    }
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    {
                        if (p.TotalPages > 1)
                            return -3;
                    }
                    break;
                case ConsoleKey.H:
                    ShouldHideHints = !ShouldHideHints;
                    break;

                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                {
                    Console.Clear();
                    return (p.CurrentPage * p.RowsPerPage) + p.SelectedIndex;
                }
            }
        }
        else
        {
            switch (p.Key)
            {
                case ConsoleKey.Escape:
                {
                    if (p.IsMainMenu)
                    {
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
                    }
                    else
                        return -1;

                    break;
                }
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    {
                        p.SelectedIndex =
                            (p.SelectedIndex == 0) ? p.Choices.Length - 1 : p.SelectedIndex - 1;
                    }
                    break;

                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    {
                        p.SelectedIndex =
                            (p.SelectedIndex == p.Choices.Length - 1) ? 0 : p.SelectedIndex + 1;
                    }
                    break;

                case ConsoleKey.H:
                    ShouldHideHints = !ShouldHideHints;
                    break;

                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                {
                    while (Console.KeyAvailable)
                        Console.ReadKey(true); // limpia el buffer
                    Console.Clear();
                    return p.SelectedIndex;
                }
            }
        }

        return null;
    }

    public static T[] GetPagination<T>(T[] array, int page = 1, int rowsPerPage = 15)
    {
        int totalPages = array.Length / rowsPerPage;

        if (array.Length % rowsPerPage != 0)
            totalPages++;

        if (array == null || array.Length == 0 || page < 1 || page > totalPages)
            return [];

        // Si la pagina es 1, el offset es 0, si es 2, el offset es rowsPerPage, etc.
        int offset = rowsPerPage * (page - 1);

        return Array.TakeFirstN(array, offset, offset + rowsPerPage);
    }

    private static string CenterText(string textToCenter, int widthOfMenu)
    {
        // "               Hola mundo               " => 40 caracteres en total
        // "Hola mundo" => 10 caracteres
        // 40-10 = 30 caracteres de espacios en blanco
        // 30/2 = 15 caracteres de ambos lados en blanco
        // 40-10-caracteresIzquierda = 15 (si fuera impar se ajustaria mejor)
        int whiteSpacesBetweenTextLeft = (widthOfMenu - textToCenter.Length) / 2;

        if (whiteSpacesBetweenTextLeft < 0)
            whiteSpacesBetweenTextLeft = 0;

        int whiteSpacesBetweenTextRight =
            widthOfMenu - textToCenter.Length - whiteSpacesBetweenTextLeft;

        return String.fillRight(" ", whiteSpacesBetweenTextLeft)
            + textToCenter
            + String.fillRight(" ", whiteSpacesBetweenTextRight);
    }
}
