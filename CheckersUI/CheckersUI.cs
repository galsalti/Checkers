using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersLogic;

namespace CheckersUI
{
    public class CheckersUI
    {
        private FormGame m_FormGame;
        private CheckersGame m_Game;

        public CheckersUI()
        {
            m_FormGame = new FormGame(this);
        }

        internal Board Board
        {
            get { return m_Game.Board; }
        }

        public string FirstPlayerName
        {
            get { return m_Game.FirstPlayerName; }
        }
        
        public string SecondPlayerName
        {
            get { return m_Game.SecondPlayerName; }
        }
        public void OpenGame()
        {
            m_FormGame.ShowDialog();
        }

        internal void InitiallizeGame(string firstPlayerName, string secondPlayerName, bool vsComputer, ushort boardSize)
        {
            m_Game = new CheckersGame(firstPlayerName, secondPlayerName, vsComputer, boardSize);
        }

        internal void ApplyUserMove(int[] i_ActionPositions)
        {
            bool playerHasAnotherMove;
            bool moveIsLegal = m_Game.TryMove(i_ActionPositions, out playerHasAnotherMove);
            bool computerModePossible = true;
            bool gameEnded = false;
            byte? winningPlayerIndex;
            int[] ComputerAction;

            gameEnded = m_Game.CheckIfGameEnds(out winningPlayerIndex);
            while (moveIsLegal && computerModePossible && !gameEnded && !playerHasAnotherMove)
            {
                computerModePossible = m_Game.ApplyMoveInComputerModeIfPossible(out ComputerAction);
                gameEnded = m_Game.CheckIfGameEnds(out winningPlayerIndex);
            }

            if (gameEnded)
            {
                string messageToUser;
                if (winningPlayerIndex == null)
                {
                    messageToUser = "Tie!";
                }
                else if (winningPlayerIndex == 0)
                {
                    messageToUser = string.Format("{0} Won!", m_Game.FirstPlayerName);
                }
                else
                {
                    messageToUser = string.Format("{0} Won!", m_Game.SecondPlayerName);
                }

                m_FormGame.NotifyGameEndedAndCheckForAnotherGame(messageToUser);
            }
        }

        internal int[] ReceiveUpdatedPointsAndInitiallizeNewGame()
        {
            int[] updatedPoints = { m_Game.FirstTeamPoints, m_Game.SecondTeamPoints };
            m_Game.InitallizeAnotherRound();

            return updatedPoints;
        }
    }
}
