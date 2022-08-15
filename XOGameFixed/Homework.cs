using System;
using System.Collections.Generic;
using System.Text;

namespace Game;

public class HomeWork : MenuSpace.Work
{
    /// <summary>
    /// Статус игры
    /// </summary>
    enum GameStatus
    {
        Win,
        Draw,
        Break,
        Play,
        Surender,
        Exit
    }

    /// <summary>
    /// Пресеты цветов
    /// </summary>
    enum SetColor
    {
        White,Black,Green,Yellow,Red,Grey
    }

    /// <summary>
    /// Длина серии для победы
    /// </summary>
    int WinSerie { get; set; } = 3;

    /// <summary>
    /// Символы игрового поля
    /// </summary>
    char[] Symbols { get; } = { ' ', 'X', 'O' };

    /// <summary>
    /// Текущий статус игры
    /// </summary>
    GameStatus Status { get; set; } = GameStatus.Play;

    /// <summary>
    /// Максимальное число действий
    /// </summary>
    int MoveCounter { get; set; }

    /// <summary>
    /// Размер игрового поля
    /// </summary>
    int SizeX, SizeY;
    
    /// <summary>
    /// Игровое поле
    /// </summary>
    char[,] Field;

    /// <summary>
    /// Скрытое поле
    /// </summary>
    int[,] IntField;

    /// <summary>
    /// Действия меню
    /// </summary>
    public new MenuSpace.Menu.Runner[] AllRuns { get; }
    
    /// <summary>
    /// Строки меню
    /// </summary>
    string[] Names { get; } = { "Один игрок", "Два игрока", "Установить размер поля", "Установить рамер выигрышной строки" };
    
    /// <summary>
    /// Получить список названий строк меню
    /// </summary>
    /// <returns></returns>
    public override string[] GetNames() => this.Names;
    
    /// <summary>
    /// Получить список действий меню
    /// </summary>
    /// <returns></returns>
    public override MenuSpace.Menu.Runner[] GetRunners() => AllRuns;

    /// <summary>
    /// Создание объекта игры
    /// </summary>
    public HomeWork() => AllRuns = new MenuSpace.Menu.Runner[] { PlayOne, PlayTwo, SetSize, SetWinSerie };

    /// <summary>
    /// Игра с компьютером
    /// </summary>
    void PlayOne()
    {
        InitGame(out char PlayerOneDot, out char PlayerTwoDot, out int X, out int Y, out int aiX, out int aiY, out int[] lastOne, out int[] lastTwo, out int[,] Ofield, out int moveCounter);
        do
        {
            Selector(SizeX, SizeY, ref Field, ref IntField, ref Ofield, PlayerOneDot, ref X, ref Y);
            lastOne[0] = Y;
            lastOne[1] = X;
            if (TurnCheck(in PlayerOneDot, "Player", in lastOne, ref IntField, ref moveCounter)) goto EndPoint;
            ReSetCost(Field, ref IntField);
            ReSetCost(Field, ref Ofield);
            SetCost(PlayerOneDot, Field, ref IntField);
            SetCost(PlayerTwoDot, Field, ref Ofield);
            AIMove(SizeX, SizeY, ref Field, ref IntField, ref Ofield, PlayerTwoDot, ref aiX, ref aiY);
            lastTwo[0] = aiY;
            lastTwo[1] = aiX;
            if (TurnCheck(in PlayerTwoDot, "CPU", in lastTwo, ref Ofield, ref moveCounter)) goto EndPoint;
            EndPoint:;
        } while (Status == GameStatus.Play);
        if (Status != GameStatus.Win)
            EndGame(Status, "player");
    }

    /// <summary>
    /// Игра с другим игроком
    /// </summary>
    void PlayTwo()
    {
        InitGame(out char PlayerOneDot, out char PlayerTwoDot, out int X, out int Y, out int X2, out int Y2, out int[] lastOne, out int[] lastTwo, out int[,] Ofield, out int moveCounter);
        do
        {
            PrintGreen("Ход первого игрока".PadRight(SizeX * 2 + 1), 0, SizeY * 2 + 3);
            Selector(SizeX, SizeY, ref Field, ref IntField, ref Ofield, PlayerOneDot, ref X, ref Y);
            lastOne[0] = Y;
            lastOne[1] = X;
            if (TurnCheck(in PlayerOneDot, "Player One", in lastOne, ref IntField, ref moveCounter)) goto EndPoint;
            PrintRed("Ход второго игрока".PadRight(SizeX * 2 + 1), 0, SizeY * 2 + 3);
            Selector(SizeX, SizeY, ref Field, ref Ofield, ref IntField, PlayerTwoDot, ref X2, ref Y2);
            lastTwo[0] = Y2;
            lastTwo[1] = X2;
            if (TurnCheck(in PlayerTwoDot, "Player Two", lastTwo, ref Ofield, ref moveCounter)) goto EndPoint;
            EndPoint:;
        } while (Status == GameStatus.Play);
        if (Status != GameStatus.Win)
            EndGame(Status, "player");
    }

    /// <summary>
    /// Установка размера игрового поля
    /// </summary>
    void SetSize()
    {
        Console.Clear();
        Console.WriteLine("Set Size of field");
        if (Int32.TryParse(Console.ReadLine(), out int val))
        {
            SizeY = SizeX = val;
        }
        else
        {
            Console.WriteLine("bad input");
            Console.ReadLine();
        }
        Console.Clear();
    }
    
    /// <summary>
    /// Установка выигрышной комбинации
    /// </summary>
    void SetWinSerie()
    {
        Console.Clear();
        Console.WriteLine("Set Line Length for win");
        if (Int32.TryParse(Console.ReadLine(), out int val))
        {
            WinSerie = val;
        }
        else
        {
            Console.WriteLine("bad input");
            Console.ReadLine();
        }
        Console.Clear();
    }

    /// <summary>
    /// Подготовка к началу игры
    /// </summary>
    /// <param name="PlayerOneDot">Символ первого игрока</param>
    /// <param name="PlayerTwoDot">Символ второго игрока</param>
    /// <param name="firstX">Координата Х первого игрока</param>
    /// <param name="firstY">Координата Y первого игрока</param>
    /// <param name="secondX">Координата Х второго игрока</param>
    /// <param name="secondY">Координата Y второго игрока</param>
    /// <param name="lastOne">Последние координаты первого игрока</param>
    /// <param name="lastTwo">Последние координаты второго игрока</param>
    /// <param name="Ofield">вторичное поле</param>
    /// <param name="moveCounter">Счётчик действий</param>
    void InitGame(out char PlayerOneDot, out char PlayerTwoDot, out int firstX, out int firstY, out int secondX, out int secondY, out int[] lastOne, out int[] lastTwo, out int[,] Ofield, out int moveCounter)
    {
        Console.Clear();
        PlayerOneDot = Symbols[1];
        PlayerTwoDot = Symbols[2];
        SizeY = SizeX = SizeX < 3 ? 3 : SizeX;
        WinSerie = WinSerie > SizeX ? SizeX : WinSerie;
        moveCounter = MoveCounter = SizeX * SizeY;
        Field = GetField();
        IntField = GetIntField();
        FillEmpty(Field);
        FillInts(IntField);
        Show();
        Console.CursorVisible = false;
        firstX = firstY = secondX = secondY = 0;
        Status = GameStatus.Play;
        lastOne = new int[] { firstY, firstX };
        lastTwo = new int[] { secondY, secondX };
        Ofield = new int[SizeY, SizeX];
        FillInts(Ofield);
        PrintGreen($"Линия для победы должна быть {WinSerie}".PadRight(SizeX * 2 + 1), 0, SizeY * 2 + 3);
        Console.SetCursorPosition(1, 1);
    }

    /// <summary>
    /// Проверка хода. Устанавливает статус игры
    /// </summary>
    /// <param name="PlayerChar">Символ игрока</param>
    /// <param name="player">Имя игрока</param>
    /// <param name="last">Последние координаты</param>
    /// <param name="field">Поле для проверки</param>
    /// <param name="moveCounter">Счётчик действий</param>
    /// <returns>true - Игра завершается. false - игра продолжается</returns>
    bool TurnCheck(in char PlayerChar, string player, in int[] last, ref int[,] field, ref int moveCounter)
    {
        if (WinCheck(PlayerChar, last, Field, ref field))
        {
            Status = GameStatus.Win;
            EndGame(GameStatus.Win, player);
            return true;
        }
        moveCounter--;
        if (moveCounter == 0)
        {
            Status = GameStatus.Draw;
            return true;
        }
        if (Status == GameStatus.Break)
            return true;
        return false;
    }
    
    /// <summary>
    /// Создание нового игрового поля
    /// </summary>
    /// <returns>Пустой массив символов</returns>
    char[,] GetField() => new char[SizeY, SizeX];
    
    /// <summary>
    /// Создание скрытого поля
    /// </summary>
    /// <returns>Пустой массив чисел</returns>
    int[,] GetIntField() => new int[SizeY, SizeX];
    
    /// <summary>
    /// Заполнение поля символов пробелами
    /// </summary>
    /// <param name="str">Заполненный массив символов</param>
    void FillEmpty(char[,] str)
    {
        for (int i = 0; i < str.GetLength(0); i++)
            for (int j = 0; j < str.GetLength(1); j++)
                str[i, j] = Symbols[0];
    }
    
    /// <summary>
    /// Заполнение массива чисел 0-ми
    /// </summary>
    /// <param name="field">Заполненный массив чисел</param>
    void FillInts(int[,] field)
    {
        for (int i = 0; i < field.GetLength(0); i++)
            for (int j = 0; j < field.GetLength(1); j++)
                field[i, j] = 0;
    }

    /// <summary>
    /// Управление с клавиатуры
    /// </summary>
    /// <param name="maxX">Максимальное положение для горизонтального смещения</param>
    /// <param name="maxY">Максимальное положение для вертикального смещения</param>
    /// <param name="chr">Игровое поле</param>
    /// <param name="IntField">Скрытое поле</param>
    /// <param name="secondField">вторичное поле</param>
    /// <param name="dot">Символ игрока</param>
    /// <param name="X">Координата Х</param>
    /// <param name="Y">Координата Y</param>
    void Selector(int maxX, int maxY, ref char[,] chr, ref int[,] IntField, ref int[,] secondField, char dot, ref int X, ref int Y)
    {
        PrintWhite(chr[Y, X], Y, X);
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar: //show intField cell
                    PrintRed(IntField[Y, X].ToString(), X == 0 ? 1 : (X * 2 + 1), Y == 0 ? 1 : (Y * 2 + 1));
                    continue;
                case ConsoleKey.Backspace: //show secondField cell
                    PrintRed(secondField[Y, X].ToString(), X == 0 ? 1 : (X * 2 + 1), Y == 0 ? 1 : (Y * 2 + 1));
                    continue;
                case ConsoleKey.Enter:
                    if (chr[Y, X] == Symbols[0])
                    {
                        SetDot(Y, X, ref IntField, ref secondField, ref chr, dot);
                        PrintBlack(chr[Y, X], Y, X);
                    }
                    break;
                case ConsoleKey.Escape:
                    Status = GameStatus.Break;
                    ExitGame();
                    break;
                case ConsoleKey.LeftArrow:
                    PrintBlack(chr[Y, X], Y, X);
                    X = X > 0 ? --X : maxX - 1;
                    break;
                case ConsoleKey.UpArrow:
                    PrintBlack(chr[Y, X], Y, X);
                    Y = Y > 0 ? --Y : maxY - 1;
                    break;
                case ConsoleKey.RightArrow:
                    PrintBlack(chr[Y, X], Y, X);
                    X = X < (maxX - 1) ? ++X : 0;
                    break;
                case ConsoleKey.DownArrow:
                    PrintBlack(chr[Y, X], Y, X);
                    Y = Y < (maxY - 1) ? ++Y : 0;
                    break;
                default:
                    break;
            }
        PrintWhite(chr[Y, X], Y, X);
        } while (key.Key != ConsoleKey.Enter & key.Key != ConsoleKey.Escape);
    }

    /// <summary>
    /// Вывод символа на консоль с заданным цветом - Чёрный текст, белый фон
    /// </summary>
    /// <param name="chr">символ</param>
    /// <param name="Y">координата Y</param>
    /// <param name="X">координата Х</param>
    void PrintWhite(char chr, int Y, int X)
    {
        Console.CursorLeft = X == 0 ? 1 : (X * 2 + 1);
        Console.CursorTop = Y == 0 ? 1 : (Y * 2 + 1);
        Color(SetColor.White);
        Console.Write(chr);
        DropColor();
    }

    /// <summary>
    /// Вывод символа на консоль с заданным цветом - Белый текст, чёрный фон
    /// </summary>
    /// <param name="chr">символ</param>
    /// <param name="Y">координата Y</param>
    /// <param name="X">координата Х</param>
    void PrintBlack(char chr, int Y, int X)
    {
        Console.CursorLeft = X == 0 ? 1 : (X * 2 + 1);
        Console.CursorTop = Y == 0 ? 1 : (Y * 2 + 1);
        Color(SetColor.Black);
        Console.Write(chr);
        DropColor();
    }

    /// <summary>
    /// Вывод строки на консоль с заданным цветом - Чёрный текст, Красный фон
    /// </summary>
    /// <param name="str">текст</param>
    /// <param name="left">отступ слева</param>
    /// <param name="top">отступ сверху</param>
    void PrintRed(string str, int left, int top)
    {
        Console.CursorLeft = left;
        Console.CursorTop = top;
        Color(SetColor.Red);
        Console.Write(str);
        DropColor();
    }
    
    /// <summary>
    /// Вывод строки на консоль с заданным цветом - Чёрный текст, Зелёный фон
    /// </summary>
    /// <param name="str">текст</param>
    /// <param name="left">отступ слева</param>
    /// <param name="top">отступ сверху</param>
    void PrintGreen(string str, int left, int top)
    {
        Console.CursorLeft = left;
        Console.CursorTop = top;
        Color(SetColor.Green);
        Console.Write(str);
        DropColor();
    }
    
    /// <summary>
    /// Установить цвет текста и фона для консоли
    /// </summary>
    /// <param name="color">Пресет</param>
    void Color(SetColor color)
    {
        switch (color)
        {
            case SetColor.Grey:
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case SetColor.White:
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case SetColor.Black:
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case SetColor.Green:
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case SetColor.Yellow:
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case SetColor.Red:
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Сбросить цвет текста и фона для консоли
    /// </summary>
    void DropColor()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    /// <summary>
    /// Вывести на консоль игровое поле
    /// </summary>
    void Show()
    {
        Console.Write("╔");
        for (int i = 0; i < Field.GetLength(1) - 1; i++)
            Console.Write("═╦");
        Console.Write("═╗\n");
        for (int i = 0; i < Field.GetLength(0); i++)
        {
            if (i < Field.GetLength(0) - 1)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                    Console.Write("║" + Field[i, j]);
                Console.Write("║\n╠");
                for (int l = 0; l < Field.GetLength(1) - 1; l++)
                    Console.Write("═╬");
                Console.WriteLine("═╣");
            }
            else
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                    Console.Write("║" + Field[i, j]);
                Console.Write("║\n╚");
                for (int l = 0; l < Field.GetLength(1) - 1; l++)
                    Console.Write("═╩");
                Console.WriteLine("═╝");
            }
        }
    }
    
    /// <summary>
    /// Выход из игры
    /// </summary>
    void ExitGame()
    {
        Console.Clear();
        Console.WriteLine("Press Any Key to return in Menu.");
        //Status = GameStatus.Exit;
    }
    
    /// <summary>
    /// Финал игры
    /// </summary>
    /// <param name="type">Статус игры к финалу</param>
    /// <param name="player">имя игрока</param>
    /// <returns></returns>
    bool EndGame(GameStatus type, string player)
    {
        Console.SetCursorPosition((Console.WindowWidth / 2)-20, Console.WindowHeight / 2);
        switch (type)
        {
            case GameStatus.Win:
                Color(SetColor.Yellow);
                Console.WriteLine($"Congratulation!!! Player {player} WIN!!!");
                goto default;
            case GameStatus.Draw:
                Color(SetColor.Grey);
                Console.WriteLine("It is DRAW.");
                goto default;
            case GameStatus.Break:
                Color(SetColor.Red);
                Console.WriteLine("Exit game without ending, progress not saved.");
                goto default;
            default:
                DropColor();
                Console.ReadKey(false);
                ExitGame();
                break;
        }
        return false;
    }
    
    /// <summary>
    /// Установка символа игрока
    /// </summary>
    /// <param name="Y">Координата Y</param>
    /// <param name="X">Координата Х</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="SecondField">Вторичное поле</param>
    /// <param name="Field">Поле</param>
    /// <param name="playerDot">Символ игрока</param>
    void SetDot(int Y, int X, ref int[,] intField, ref int[,] SecondField, ref char[,] Field, char playerDot)
    {
        intField[Y, X] = -1;
        SecondField[Y, X] = -1;
        Field[Y, X] = playerDot;
    }

    /// <summary>
    /// Ход ИИ
    /// </summary>
    /// <param name="maxX">Максимальное смещение по горизонтали</param>
    /// <param name="maxY">Максимальное смещение по вертикали</param>
    /// <param name="str">имя ИИ</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="SecondField">Вторичное поле</param>
    /// <param name="playerDot">символ ИИ</param>
    /// <param name="X">Координата Х</param>
    /// <param name="Y">Координата Y</param>
    void AIMove(int maxX, int maxY, ref char[,] str, ref int[,] intField, ref int[,] SecondField, char playerDot, ref int X, ref int Y)
    {
        int[] plYX = FindGoodPoint(intField, out _);
        int[] aiYX = FindGoodPoint(SecondField, out int valAi);
        if (valAi >= 3)
        {
            Y = aiYX[0];
            X = aiYX[1];
            SetDot(Y, X, ref SecondField, ref intField, ref str, playerDot);
            PrintBlack(str[Y, X], Y, X);
            Status = GameStatus.Play;
        }
        else
        {
            Y = plYX[0];
            X = plYX[1];
            SetDot(Y, X, ref SecondField, ref intField, ref str, playerDot);
            PrintBlack(str[Y, X], Y, X);
            Status = GameStatus.Play;
        }
    }
    
    /// <summary>
    /// Поиск позиции для ИИ
    /// </summary>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="temp">Вес клетки</param>
    /// <returns>Координаты клетки с максимальным весом</returns>
    int[] FindGoodPoint(int[,] intField, out int temp)
    {
        temp = 0;
        int[] XY = new int[2];
        for (int i = 0; i < intField.GetLength(0); i++)
            for (int j = 0; j < intField.GetLength(1); j++)
                if (intField[i, j] > temp)
                {
                    temp = intField[i, j];
                    XY[0] = i;
                    XY[1] = j;
                }
        if (temp == 0)
            for (int i = 0; i < intField.GetLength(0); i++)
                for (int j = 0; j < intField.GetLength(1); j++)
                    if (intField[i, j] >= temp)
                    {
                        temp = intField[i, j];
                        XY[0] = i;
                        XY[1] = j;
                    }
        return XY;
    }
    
    /// <summary>
    /// Сбросить веса клеток
    /// </summary>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    void ReSetCost(char[,] field, ref int[,] intField)
    {
        for (int i = 0; i < field.GetLength(0); i++)
            for (int j = 0; j < field.GetLength(1); j++)
                if (field[i, j] == Symbols[0])
                    intField[i, j] = 0;
    }

    /// <summary>
    /// Установить вес клетки
    /// </summary>
    /// <param name="playerChar">символ игрока</param>
    /// <param name="field">поле</param>
    /// <param name="intField">Скрытое поле</param>
    void SetCost(char playerChar, char[,] field, ref int[,] intField)
    {
        int[] Dot = new int[2];
        for (int fieldY = 0; fieldY < field.GetLength(0); fieldY++)
            for (int fieldX = 0; fieldX < field.GetLength(1); fieldX++)
                if (field[fieldY, fieldX] == Symbols[0])
                {
                    Dot[0] = fieldY;
                    Dot[1] = fieldX;
                    int NW = GetCostNW(Dot, field, intField, playerChar);
                    int N = GetCostN(Dot, field, intField, playerChar);
                    int NE = GetCostNE(Dot, field, intField, playerChar);
                    int E = GetCostE(Dot, field, intField, playerChar);
                    int SE = GetCostSE(Dot, field, intField, playerChar);
                    int S = GetCostS(Dot, field, intField, playerChar);
                    int SW = GetCostSW(Dot, field, intField, playerChar);
                    int W = GetCostW(Dot, field, intField, playerChar);
                    int[] NWSE = { N + S, W + E, NW + SE, NE + SW };
                    int result = 0;
                    for (int i = 0; i < NWSE.Length; i++)
                        result = NWSE[i] > result ? NWSE[i] : result;
                    intField[fieldY, fieldX] = result;
                }
    }

    /// <summary>
    /// Проверка на выход за пределы игрового поля
    /// </summary>
    /// <param name="Y">Текущая координата Y</param>
    /// <param name="X">Текущая координата Х</param>
    /// <param name="arr">Поле</param>
    /// <returns>true - координаты в пределах поля, false - выход за пределы поля</returns>
    bool InRange(int Y, int X, char[,] arr) => (Y >= 0 & Y < arr.GetLength(0)) & (X >= 0 & X < arr.GetLength(1));
    
    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вверх, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostN(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] - 1; i >= Dot[0] - WinSerie; i--)
            if (InRange(i, Dot[1], field))
            {
                if (field[i, Dot[1]] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вниз, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostS(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] + 1; i <= Dot[0] + WinSerie; i++)
            if (InRange(i, Dot[1], field))
            {
                if (field[i, Dot[1]] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вправо, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostE(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[1] + 1; i <= Dot[1] + WinSerie; i++)
            if (InRange(Dot[0], i, field))
            {
                if (field[Dot[0], i] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Влево, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostW(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[1] - 1; i >= Dot[1] - WinSerie; i--)
            if (InRange(Dot[0], i, field))
            {
                if (field[Dot[0], i] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вверх и влево, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostNW(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] - 1, j = Dot[1] - 1; (i >= Dot[0] - WinSerie) & (j >= Dot[1] - WinSerie); i--, j--)
            if (InRange(i, j, field))
            {
                if (field[i, j] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вниз и вправо, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostSE(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] + 1, j = Dot[1] + 1; (i <= Dot[0] + WinSerie) & (j <= Dot[1] + WinSerie); i++, j++)
            if (InRange(i, j, field))
            {
                if (field[i, j] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вверх и вправо, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostNE(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] - 1, j = Dot[1] + 1; (i >= Dot[0] - WinSerie) & (j <= Dot[1] + WinSerie); i--, j++)
            if (InRange(i, j, field))
            {
                if (field[i, j] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }

            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Получить максимальный вес клетки в массиве внутри поля, начинающегося в точке <paramref name="Dot"/> и идущего по направлению Вниз и влево, с максимальной длинной равной выигрышной серии.
    /// </summary>
    /// <param name="Dot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>Вес клиетки</returns>
    int GetCostSW(int[] Dot, char[,] field, int[,] intField, char playerChar)
    {
        int result = 0;
        int counter = 0;
        for (int i = Dot[0] + 1, j = Dot[1] - 1; (i <= Dot[0] + WinSerie) & (j >= Dot[1] - WinSerie); i++, j--)
            if (InRange(i, j, field))
            {
                if (field[i, j] == playerChar)
                {
                    counter++;
                }
                else
                {
                    result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
                    break;
                }
            }
            else
            {
                result = counter > intField[Dot[0], Dot[1]] ? counter : intField[Dot[0], Dot[1]];
            }
        return result;
    }

    /// <summary>
    /// Проверка на выход за пределы игрового поля для выигрышной серии
    /// </summary>
    /// <param name="fieldX">Координата Х</param>
    /// <param name="fieldY">Координата Y</param>
    /// <param name="field">Поле</param>
    /// <param name="counter">Счётчик</param>
    /// <param name="playerChar">Символ игрока</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool CheckInRange(in int fieldX, in int fieldY, in char[,] field, ref int counter, in char playerChar)
    {
        if (InRange(fieldX, fieldY, field))
            if (field[fieldX, fieldY] == playerChar)
            {
                counter++;
                if (counter == WinSerie)
                    return true;
            }
            else if (field[fieldX, fieldY] != playerChar)
            {
                counter = 0;
            }
        return false;
    }

    /// <summary>
    /// Проверка диагонали А проходящей через точку отсчёта
    /// </summary>
    /// <param name="playerChar">Символ игрока</param>
    /// <param name="lastDot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool CheckDiagonalUp(char playerChar, int[] lastDot, char[,] field, ref int[,] intField)
    {
        int Xmin = lastDot[1] - WinSerie;
        int Xmax = lastDot[1] + WinSerie;
        int Ymin = lastDot[0] - WinSerie;
        int Ymax = lastDot[0] + WinSerie;
        int counter = 0;
        for (int i = Ymax, j = Xmin; (i >= Ymin) & (j <= Xmax); i--, j++)
            if (CheckInRange(in i, in j, field, ref counter, playerChar))
                return true;

        return false;
    }

    /// <summary>
    /// Проверка диагонали Б проходящей через точку отсчёта
    /// </summary>
    /// <param name="playerChar">Символ игрока</param>
    /// <param name="lastDot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool CheckDiagonalDown(char playerChar, int[] lastDot, char[,] field, ref int[,] intField)
    {
        int Xmin = lastDot[1] - WinSerie;
        int Xmax = lastDot[1] + WinSerie;
        int Ymin = lastDot[0] - WinSerie;
        int Ymax = lastDot[0] + WinSerie;
        int counter = 0;
        for (int i = Ymin, j = Xmin; (i <= Ymax) & (j <= Xmax); i++, j++)
            if (CheckInRange(in i, in j, in field, ref counter, playerChar))
                return true;

        return false;
    }

    /// <summary>
    /// Проверка горизонтали проходящей через точку отсчёта
    /// </summary>
    /// <param name="playerChar">Символ игрока</param>
    /// <param name="lastDot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool CheckHorizontal(char playerChar, int[] lastDot, char[,] field, ref int[,] intField)
    {
        int Xmin = lastDot[1] - WinSerie;
        int Xmax = lastDot[1] + WinSerie;
        int counter = 0;
        for (int i = Xmin; i <= Xmax; i++)
            if (CheckInRange(in lastDot[0], in i, field, ref counter, playerChar))
                return true;

        return false;
    }

    /// <summary>
    /// Проверка вертикали проходящей через точку отсчёта
    /// </summary>
    /// <param name="playerChar">Символ игрока</param>
    /// <param name="lastDot">Точка отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool CheckVertical(char playerChar, int[] lastDot, char[,] field, ref int[,] intField)
    {
        int Ymin = lastDot[0] - WinSerie;
        int Ymax = lastDot[0] + WinSerie;
        int counter = 0;
        for (int i = Ymin; i <= Ymax; i++)
            if (CheckInRange(in i, in lastDot[1], field, ref counter, playerChar))
                return true;

        return false;
    }

    /// <summary>
    /// Проверка выигрышной серии.
    /// </summary>
    /// <param name="playerChar">Символ игрока</param>
    /// <param name="lastDot">Координаты точки отсчёта</param>
    /// <param name="field">Поле</param>
    /// <param name="intField">Скрытое поле</param>
    /// <returns>true - собрана выигрышная серия, false - все проверки прошли и серия не набрана</returns>
    bool WinCheck(char playerChar, int[] lastDot, char[,] field, ref int[,] intField)
    {
        bool ver = CheckVertical(playerChar, lastDot, field, ref intField);
        bool hor = CheckHorizontal(playerChar, lastDot, field, ref intField);
        bool up = CheckDiagonalUp(playerChar, lastDot, field, ref intField);
        bool dn = CheckDiagonalDown(playerChar, lastDot, field, ref intField);
        return ver | hor | up | dn;
    }
}