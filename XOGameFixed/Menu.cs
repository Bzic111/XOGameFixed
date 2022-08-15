using System;
using System.Collections.Generic;

namespace MenuSpace;

public class Menu
{
    public delegate void Cycler(Dictionary<string, Runner> Dict);
    public delegate void Runner();

    /// <summary>
    /// Метод вывода текста с определённой позиции.
    /// </summary>
    /// <param name="text">Текст</param>
    /// <param name="row">позиция строки</param>
    /// <param name="col">позиция столбца</param>
    public void Print(string text, int row, int col)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.SetCursorPosition(col, row);
        Console.Write(text);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }

    /// <summary>
    /// Метод создания массива строк меню на основе массива строк <paramref name="str"/>. Массив строк копируется в пункты меню.
    /// </summary>
    /// <param name="str">Массива строк</param>
    /// <returns>Массива строк для меню, последний элемент "Exit"</returns>
    public void HomeworkMenu(Work wrk, ref int cursor)
    {
        cursor = 0;
        string selected;
        string[] menuHomework2 = CreateMenu(wrk.GetNames());
        Console.Clear();
        Show(menuHomework2);
        do
        {
            Selector(menuHomework2, out selected, ref cursor);
            if ((selected == menuHomework2[cursor]) & (selected != "Exit"))
            {
                wrk.GetRunners()[cursor]();
                cursor = 0;
                Console.Clear();
                Show(menuHomework2);
            }
        } while (selected != "Exit");
    }

    /// <summary>
    /// Вывод главного меню
    /// </summary>
    /// <param name="works">Объекты</param>
    /// <param name="entryName">Название для всех пунктов меню</param>
    public void MainMenu(Work[] works, string entryName = "Default name")
    {
        string selected;
        int cursor = 0;
        string[] mainMenu = CreateMenu(works.Length, entryName);
        Console.Clear();
        Show(mainMenu);
        do
        {
            Selector(mainMenu, out selected, ref cursor);
            if ((selected == mainMenu[cursor]) & (selected != "Exit"))
            {
                HomeworkMenu(works[cursor], ref cursor);
                cursor = 0;
                Console.Clear();
                Show(mainMenu);
            }
        } while (selected != "Exit");

        Print("Programm End...", mainMenu.Length + 2, 0);
    }

    /// <summary>
    /// Создание Меню
    /// </summary>
    /// <param name="str">Список наименований пунктов</param>
    /// <returns>Меню</returns>
    public string[] CreateMenu(string[] str)
    {
        string[] menu = new string[str.Length + 1];
        for (int i = 0; i < menu.Length; i++)
            if (i < menu.Length - 1)
                menu[i] = str[i];
            else if (i == menu.Length - 1)
                menu[i] = "Exit";
        return menu;
    }

    /// <summary>
    /// Метод создания массива строк меню. 
    /// </summary>
    /// <param name="length">Число основных пунктов</param>
    /// <param name="name">Имя пункта. По умоляанию "Defaul name"</param>
    /// <returns>Массив строк с именами <paramref name="name"/> и номером, последний элемент "Exit"</returns>
    string[] CreateMenu(int length, string name = "Defaul name")
    {
        string[] menu = new string[length + 1];
        for (int i = 0; i < length + 1; i++)
            if (i < menu.Length - 1)
                menu[i] = name + $" {i + 1}";
            else if (i == menu.Length - 1)
                menu[i] = "Exit";
        return menu;
    }

    /// <summary>
    /// Селектор для меню. Управляется стрелками клавиатуры Вверх, Вниз и Ввод или Пробел. Escape - назад или выход.
    /// </summary>
    /// <param name="str">Массив строк меню</param>
    /// <param name="selected">Выбранный пункт меню</param>
    public void Selector(string[] str, out string selected, ref int cursorRow)
    {
        Console.CursorVisible = false;
        selected = null;
        Print(str[cursorRow], cursorRow, 0);
        var move = Console.ReadKey(false);
        switch (move.Key)
        {
            case ConsoleKey.DownArrow:
                Console.SetCursorPosition(0, cursorRow);
                Console.Write(str[cursorRow]);
                cursorRow = cursorRow < (str.Length - 1) ? ++cursorRow : 0;
                Print(str[cursorRow], cursorRow, 0);
                break;
            case ConsoleKey.UpArrow:
                Console.SetCursorPosition(0, cursorRow);
                Console.Write(str[cursorRow]);
                cursorRow = cursorRow > 0 ? --cursorRow : (str.Length - 1);
                Print(str[cursorRow], cursorRow, 0);
                break;
            case ConsoleKey.Enter:
                selected = str[cursorRow];
                break;
            case ConsoleKey.Escape:
                selected = "Exit";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Цикл вывода массива строк
    /// </summary>
    /// <param name="str">массив строк</param>
    public void Show(string[] str)
    {
        for (int i = 0; i < str.Length; i++)
            Console.WriteLine(str[i]);
    }


}
