using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersUI
{
    public enum eCellType
    {
        BlackCoin,
        WhiteCoin,
        BlackKing,
        WhiteKing,
        EmptyCell,
        GrayCell
    }

    class CheckersCell : PictureBox
    {
        private readonly int r_Row;
        private readonly int r_Column;
        private eCellType m_CellType;

        public CheckersCell(eCellType i_CellType, int i_Row, int i_Column, int i_Size)
        {
            SetCellProperties(i_CellType);
            r_Row = i_Row;
            r_Column = i_Column;
            this.Size = new Size(i_Size, i_Size);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        internal eCellType CellType
        {
            get { return m_CellType; }
        }

        internal void SetCellProperties(eCellType i_CellType)
        {
            m_CellType = i_CellType;

            switch (i_CellType)
            {
                case eCellType.BlackCoin:
                    this.Load("http://chongzizil.github.io/Checkers-SMG/imgs/black_man.png");
                    this.BackColor = Color.White;
                    break;
                case eCellType.WhiteCoin:
                    this.Load("http://chongzizil.github.io/Checkers-SMG/imgs/white_man.png");
                    this.BackColor = Color.White;
                    break;
                case eCellType.BlackKing:
                    this.Load("http://chongzizil.github.io/Checkers-SMG/imgs/black_cro.png");
                    this.BackColor = Color.White;
                    break;
                case eCellType.WhiteKing:
                    this.Load("http://chongzizil.github.io/Checkers-SMG/imgs/white_cro.png");
                    this.BackColor = Color.White;
                    break;
                case eCellType.EmptyCell:
                    this.BackColor = Color.White;
                    this.Image = null;
                    break;
                case eCellType.GrayCell:
                    this.BackColor = Color.Gray;
                    break;
            }

        }
        internal int Row
        {
            get { return r_Row; }
        }

        internal int Column
        {
            get { return r_Column; }
        }
    }
}
