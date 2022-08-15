using System;
using System.Collections.Generic;
using System.Text;
using Game;
using MenuSpace;

Menu mainMenu = new Menu();
Work[] works = new Work[]
{
    new HomeWork()
};
mainMenu.MainMenu(works, "Homework");