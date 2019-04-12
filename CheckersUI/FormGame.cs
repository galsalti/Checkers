using CheckersLogic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersUI
{
    public class FormGame : Form
    {
        private CheckersUI m_CheckersUI;
        private FormLogin m_FormLogin = new FormLogin();
        private const int k_CellSize = 50;
        private const int k_SideMarginSize = 20;
        private const int k_TopMarginSize = 60;
        private int m_FirstPlayerPoints = 0;
        private int m_SecondPlayerPoints = 0;
        private CheckersCell m_LastClickedCoin;
        CheckersCell[,] m_CoinsBoard;

        public FormGame(CheckersUI i_CheckersUI)
        {
            m_CheckersUI = i_CheckersUI;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Damka";
            this.MaximizeBox = false;
        }

        private void cellWithCoin_Click(object sender, EventArgs e)
        {
            if (((CheckersCell)sender).BackColor != Color.LightBlue)
            {
                if (m_LastClickedCoin != null)
                {
                    m_LastClickedCoin.BackColor = Color.White;
                }

                m_LastClickedCoin = (CheckersCell)sender;
                ((CheckersCell)sender).BackColor = Color.LightBlue;
            }
            else
            {
                m_LastClickedCoin = null;
                ((CheckersCell)sender).BackColor = Color.White;
            }
        }

        private void emptyCell_Click(object sender, EventArgs e)
        {
            int[] actionPositions;

            if (m_LastClickedCoin != null)
            {
                actionPositions = new int[4];
                actionPositions[0] = m_LastClickedCoin.Row;
                actionPositions[1] = m_LastClickedCoin.Column;
                actionPositions[2] = ((CheckersCell)sender).Row;
                actionPositions[3] = ((CheckersCell)sender).Column;
                m_LastClickedCoin.BackColor = Color.White;
                m_LastClickedCoin = null;
                m_CheckersUI.ApplyUserMove(actionPositions);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            int boardSizeInCells;

            m_FormLogin.ShowDialog();
            if (!m_FormLogin.LoginSuccessful)
            {
                this.Close();
            }
            else
            {
                boardSizeInCells = m_FormLogin.BoardSize;
                m_CoinsBoard = new CheckersCell[boardSizeInCells, boardSizeInCells];
                this.ClientSize = new Size(boardSizeInCells * k_CellSize + k_SideMarginSize * 2, boardSizeInCells * k_CellSize + (int)(1.5 * k_TopMarginSize));
                this.CenterToScreen();
                m_CheckersUI.InitiallizeGame(m_FormLogin.FirstPlayerName, m_FormLogin.SecondPlayerName, m_FormLogin.VsComputer, m_FormLogin.BoardSize);
                initializeCoinsBoardAndControls();
                UpdateFormToCurrentCoinsBoardState();
            }
        }

        //initialize the controls in the matrix that represent the board
        private void initializeCoinsBoardAndControls()
        {
            initializeFirstPlayerControls();
            initializeSecondPlayerControls();
        }

        private void initializeFirstPlayerControls()
        {
            for (int i = 0; i < m_CoinsBoard.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < m_CoinsBoard.GetLength(1); j++)
                {
                    if (i == m_CoinsBoard.GetLength(0) / 2 - 1) // one of the two empty rows at the middle of the board
                    {
                        if (i % 2 == j % 2) // grey cell
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.GrayCell, i, j, k_CellSize);
                        }
                        else // cell without coin
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.EmptyCell, i, j, k_CellSize);
                            m_CoinsBoard[i, j].Click += new EventHandler(emptyCell_Click);
                            m_CheckersUI.Board.GetCell(i, j).m_ReportChangeStateOfCoin += updatePositionOfCell;
                        }
                    }
                    else // the rows that contains coins
                    {
                        if (i % 2 == j % 2) // grey cell
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.GrayCell, i, j, k_CellSize);
                        }
                        else // cell with black coin
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.BlackCoin, i, j, k_CellSize);
                            m_CoinsBoard[i, j].Click += new EventHandler(cellWithCoin_Click);
                            m_CheckersUI.Board.GetCell(i, j).m_ReportChangeStateOfCoin += updatePositionOfCell;
                        }
                    }

                    this.Controls.Add(m_CoinsBoard[i, j]);
                }
            }
        }

        private void initializeSecondPlayerControls()
        {
            for (int i = m_CoinsBoard.GetLength(0) / 2; i < m_CoinsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < m_CoinsBoard.GetLength(1); j++)
                {
                    if (i == m_CoinsBoard.GetLength(0) / 2) // one of the two empty rows at the middle of the board
                    {
                        if (i % 2 == j % 2) // grey cell
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.GrayCell, i, j, k_CellSize);
                        }
                        else
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.EmptyCell, i, j, k_CellSize);
                            m_CoinsBoard[i, j].Click += new EventHandler(emptyCell_Click);
                            m_CheckersUI.Board.GetCell(i, j).m_ReportChangeStateOfCoin += updatePositionOfCell;
                        }
                    }
                    else // the rows that contains coins
                    {
                        if (i % 2 == j % 2) // grey cell
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.GrayCell, i, j, k_CellSize);
                        }
                        else // cell with black coin
                        {
                            m_CoinsBoard[i, j] = new CheckersCell(eCellType.WhiteCoin, i, j, k_CellSize);
                            m_CoinsBoard[i, j].Click += new EventHandler(cellWithCoin_Click);
                            m_CheckersUI.Board.GetCell(i, j).m_ReportChangeStateOfCoin += updatePositionOfCell;
                        }
                    }

                    this.Controls.Add(m_CoinsBoard[i, j]);
                }
            }
        }
         
        private void updatePositionOfCell(int[] i_CellPosition, Board.ePositionState i_PositionState)
        {
            eCellType newType = ConvertEpositionStateToEcellType(i_PositionState);
            CheckersCell cellToChangePoisiton = m_CoinsBoard[i_CellPosition[0], i_CellPosition[1]];

            if (newType != eCellType.EmptyCell)
            {
                cellToChangePoisiton.Click += new EventHandler(cellWithCoin_Click);
                cellToChangePoisiton.Click -= new EventHandler(emptyCell_Click);
            }
            else
            {
                cellToChangePoisiton.Click -= new EventHandler(cellWithCoin_Click);
                cellToChangePoisiton.Click += new EventHandler(emptyCell_Click);
            }

            m_CoinsBoard[i_CellPosition[0], i_CellPosition[1]].SetCellProperties(newType);
        }

        private bool areTheSameCellType(Board.BoardCell i_LogicBoardCell, CheckersCell i_UiBoardCell)
        {
            return (i_UiBoardCell.CellType == ConvertEpositionStateToEcellType(i_LogicBoardCell.PositionState));
        }

        internal eCellType ConvertEpositionStateToEcellType(Board.ePositionState i_PositionState)
        {
            eCellType appropriateCellType;

            switch (i_PositionState)
            {
                case Board.ePositionState.EmptyPosition:
                    appropriateCellType = eCellType.EmptyCell;
                    break;
                case Board.ePositionState.FirstPlayer:
                    appropriateCellType = eCellType.WhiteCoin;
                    break;
                case Board.ePositionState.FirstPlayerKing:
                    appropriateCellType = eCellType.WhiteKing;
                    break;
                case Board.ePositionState.SecondPlayer:
                    appropriateCellType = eCellType.BlackCoin;
                    break;
                case Board.ePositionState.SecondPlayerKing:
                    appropriateCellType = eCellType.BlackKing;
                    break;
                default:  // when i_PositionState == null
                    appropriateCellType = eCellType.GrayCell;
                    break;
            }

            return appropriateCellType;
        }

        internal void NotifyGameEndedAndCheckForAnotherGame(string i_MessageToUser)
        {
            if (MessageBox.Show(
                string.Format(@"{0}
Another Round?", i_MessageToUser),
                Text,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question) == DialogResult.OK)
            {
                int[] updatedPoints = m_CheckersUI.ReceiveUpdatedPointsAndInitiallizeNewGame();
                m_FirstPlayerPoints = updatedPoints[0];
                m_SecondPlayerPoints = updatedPoints[1];
                Controls.Clear();
                initializeCoinsBoardAndControls();
                UpdateFormToCurrentCoinsBoardState();
            }
            else
            {
                Close();
            }
        }

        internal void UpdateFormToCurrentCoinsBoardState()
        {
            Label labelPlayerOnePoints = new Label();
            Label labelPlayerTwoPoints = new Label();
            labelPlayerOnePoints.Text = String.Format("{0}: {1}", m_CheckersUI.FirstPlayerName, m_FirstPlayerPoints);
            labelPlayerOnePoints.Location = new Point(((this.ClientSize.Width - labelPlayerTwoPoints.Width) / 2) - 10 * m_FormLogin.BoardSize, 10);
            labelPlayerTwoPoints.Text = String.Format("{0}: {1}", m_CheckersUI.SecondPlayerName, m_SecondPlayerPoints);
            labelPlayerTwoPoints.Location = new Point((this.ClientSize.Width - labelPlayerTwoPoints.Width) / 2 + 10 * m_FormLogin.BoardSize, 10);
            labelPlayerOnePoints.Font = new Font(FontFamily.GenericSansSerif, 10.0F, FontStyle.Bold);
            labelPlayerTwoPoints.Font = new Font(FontFamily.GenericSansSerif, 10.0F, FontStyle.Bold);
            this.Controls.AddRange(new Control[] { labelPlayerOnePoints, labelPlayerTwoPoints });

            for (int i = 0; i < m_CoinsBoard.GetLength(0); i++)
            {
                for (int j = 0; j < m_CoinsBoard.GetLength(1); j++)
                {
                    m_CoinsBoard[i, j].Location = new Point(j * k_CellSize + k_SideMarginSize, i * k_CellSize + k_TopMarginSize);
                }
            }
        }
    }
}
