using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CheckersUI checkersGameUI = new CheckersUI();
            checkersGameUI.OpenGame();
        }
    }
}
