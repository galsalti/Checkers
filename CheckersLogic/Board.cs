using System;

namespace CheckersLogic
{

    public class Board
    {
        public enum ePositionState
        {
            EmptyPosition,
            FirstPlayer,
            SecondPlayer,
            FirstPlayerKing,
            SecondPlayerKing
        }

        public class BoardCell
        {
            public event Action<int[], ePositionState> m_ReportChangeStateOfCoin;
            private readonly int r_Row;
            private readonly int r_Column;
            private ePositionState m_PositionState;

            public BoardCell(int i_Row, int i_Column, ePositionState i_PositionState)
            {
                r_Row = i_Row;
                r_Column = i_Column;
                m_PositionState = i_PositionState;
            }

            public ePositionState PositionState
            {
                get { return m_PositionState; }
                internal set { m_PositionState = value; }
            }

            public int Row
            {
                get { return r_Row; }
            }

            public int Column
            {
                get { return r_Column; }
            }

            public bool IsKing()
            {
                bool isKing = m_PositionState == ePositionState.FirstPlayerKing
                    || m_PositionState == ePositionState.SecondPlayerKing;
                return isKing;
            }

            internal void OnPositionChanged(ePositionState i_NewPositionState)
            {
                m_ReportChangeStateOfCoin.Invoke(new int[]{ r_Row, r_Column}, i_NewPositionState);
            }
        }

        private BoardCell[,] m_Board;
        private readonly int m_BoardSize;
        private int m_FirstTeamValueOfCoins;
        private int m_SecondTeamValueOfCoins;

        internal Board(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_Board = new BoardCell[m_BoardSize, m_BoardSize];
            m_FirstTeamValueOfCoins = ((i_BoardSize / 2) - 1) * (i_BoardSize / 2);
            m_SecondTeamValueOfCoins = ((i_BoardSize / 2) - 1) * (i_BoardSize / 2);
        }

        public int FirstTeamValueOfCoins
        {
            get { return m_FirstTeamValueOfCoins; }
            internal set { m_FirstTeamValueOfCoins = value; }
        }

        public int SecondTeamValueOfCoins
        {
            get { return m_SecondTeamValueOfCoins; }
            internal set { m_SecondTeamValueOfCoins = value; }
        }

        internal void InitializeBoard()
        {
            bool writeOnOddPositions;
            for (int i = 0; i < m_BoardSize; i++)
            {
                //in the rows with even index the coins positions are the odds columns
                //and in the rows with odd index the coins positions are the even columns
                writeOnOddPositions = (i % 2 == 0);
                for (int j = 0; j < m_BoardSize; j++)
                {
                    if ((writeOnOddPositions && (j % 2 == 1)) || (!writeOnOddPositions && (j % 2 == 0)))
                    {
                        if (i < m_BoardSize / 2 - 1)
                        {
                            m_Board[i, j] = new BoardCell(i, j, ePositionState.SecondPlayer);
                        }
                        else if (i > m_BoardSize / 2)
                        {
                            m_Board[i, j] = new BoardCell(i, j, ePositionState.FirstPlayer);
                        }
                        else
                        {
                            m_Board[i, j] = new BoardCell(i, j, ePositionState.EmptyPosition);
                        }
                    }
                    else
                    {
                        m_Board[i, j] = new BoardCell(i, j, ePositionState.EmptyPosition);
                    }
                }
            }
        }

        public BoardCell GetCell(int i_XScale, int i_YSCale)
        {
            return m_Board[i_XScale, i_YSCale];
        }

        public int BoardSize
        {
            get { return m_BoardSize; }
        }

        internal BoardCell[,] BoardArray
        {
            get { return m_Board; }
        }
    }
}
