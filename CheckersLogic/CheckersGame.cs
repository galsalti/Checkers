using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersLogic
{
    public class CheckersGame
    {
        private eVsComputerOrPlayer m_VersusMode;
        private string m_SecondPlayerName;
        private string m_FirstPlayerName;
        private CheckersLogicManager m_CheckersLogicManager;
        private Board m_Board;
        private int m_FirstTeamPoints = 0;
        private int m_SecondTeamPoints = 0;
        private byte m_PlayerIndex = 0;
        private int[] m_ForcedPositionToStartFrom = null;

        public eVsComputerOrPlayer VersusMode
        {
            get { return m_VersusMode; }
            internal set { m_VersusMode = value; }
        }

        public enum eVsComputerOrPlayer
        {
            Computer = 1,
            Player = 2
        }

        public CheckersGame(string i_FirstPlayerName, string i_SecondPlayerName, bool i_VsComputer, int i_BoardSize)
        {
            m_FirstPlayerName = i_FirstPlayerName;
            m_SecondPlayerName = i_SecondPlayerName;
            CreateBoard(i_BoardSize);
            if (i_VsComputer)
            {
                m_VersusMode = eVsComputerOrPlayer.Computer;
            }
            else
            {
                m_VersusMode = eVsComputerOrPlayer.Player;
            }

            m_FirstTeamPoints = 0;
            m_SecondTeamPoints = 0;
        }

        public string CurrentPlayerName
        {
            get { return m_PlayerIndex == 0 ? m_FirstPlayerName : m_SecondPlayerName; }
        }
        public string FirstPlayerName
        {
            get { return m_FirstPlayerName; }
            internal set { m_FirstPlayerName = value; }
        }

        public string SecondPlayerName
        {
            get { return m_SecondPlayerName; }
            internal set { m_SecondPlayerName = value; }
        }

        public int SecondTeamPoints
        {
            get { return m_SecondTeamPoints; }
            internal set { m_SecondTeamPoints = value; }
        }

        public int FirstTeamPoints
        {
            get { return m_FirstTeamPoints; }
            internal set { m_FirstTeamPoints = value; }
        }
        public Board Board
        {
            get { return m_Board; }
        }

        internal void CreateBoard(int i_BoardSize)
        {
            m_Board = new Board(i_BoardSize);
            m_CheckersLogicManager = new CheckersLogicManager(this, m_Board);
            m_Board.InitializeBoard();
        }
        public void InitallizeAnotherRound()
        {
            m_PlayerIndex = 0;
            CreateBoard(m_Board.BoardSize);
        }
        public bool TryMove(int[] i_ActionPositions, out bool o_PlayerHasAnotherMove)
        {
            bool moveIsLegal = playOneMove(i_ActionPositions);

            if (moveIsLegal && m_ForcedPositionToStartFrom != null)
            {
                o_PlayerHasAnotherMove = true;
            }
            else
            {
                o_PlayerHasAnotherMove = false;
                if (moveIsLegal)
                {
                    m_PlayerIndex = (byte)((m_PlayerIndex + 1) % 2);
                }
            }

            return moveIsLegal;
        }

        public bool ApplyMoveInComputerModeIfPossible(out int[] o_ActionPositions)
        {
            bool nextMoveIsPossible = false;

            if (m_VersusMode == eVsComputerOrPlayer.Computer)
            {
                o_ActionPositions = playComputerMove();
                if (m_ForcedPositionToStartFrom != null)
                {
                    nextMoveIsPossible = true;
                }
                else
                {
                    m_PlayerIndex = (byte)((m_PlayerIndex + 1) % 2);
                }
            }
            else
            {
                o_ActionPositions = null;
            }

            return nextMoveIsPossible;
        }
        public bool CheckIfGameEnds(out byte? o_WinningPlayerIndex)
        {
            bool gameEnded = m_CheckersLogicManager.CheckIfGameEnds();

            if (gameEnded)
            {
                o_WinningPlayerIndex = m_CheckersLogicManager.UpdatePointsOfTeamsAndStateOfGame();
            }
            else
            {
                o_WinningPlayerIndex = null;
            }

            return gameEnded;
        }
        private bool playOneMove(int[] i_ActionPositions)
        {
            CheckersLogicManager.eMoveType eMoveTypeChosen, eCurrentLegalMoveType;
            bool moveSucceeded;

            bool canEat = m_CheckersLogicManager.CheckForMoveTypePossibilities(m_PlayerIndex, CheckersLogicManager.eMoveType.Eat, null);
            bool legalStartingPosition = m_ForcedPositionToStartFrom == null
                || (i_ActionPositions[0] == m_ForcedPositionToStartFrom[0] && i_ActionPositions[1] == m_ForcedPositionToStartFrom[1]);

            if (canEat)
            { // the current player has one or more eat moves available
                eCurrentLegalMoveType = CheckersLogicManager.eMoveType.Eat;
            }
            else
            {
                eCurrentLegalMoveType = CheckersLogicManager.eMoveType.Regular;
            }

            eMoveTypeChosen = m_CheckersLogicManager.CheckLegalActionAndReturnType(i_ActionPositions, m_PlayerIndex);
            if (legalStartingPosition && (eMoveTypeChosen == eCurrentLegalMoveType))
            {
                applyActionAndCheckForAnotherEat(eMoveTypeChosen, i_ActionPositions);
                moveSucceeded = true;
            }
            else
            {
                moveSucceeded = false;
            }

            return moveSucceeded;
        }

        private int[] playComputerMove()
        {
            List<int[]> legalIndexesToApplyMoveFrom;
            bool canEat = m_CheckersLogicManager.CheckForMoveTypePossibilities(1, CheckersLogicManager.eMoveType.Eat, null);
            CheckersLogicManager.eMoveType legalMoveType;

            if (canEat)
            {
                legalMoveType = CheckersLogicManager.eMoveType.Eat;
            }
            else
            {
                legalMoveType = CheckersLogicManager.eMoveType.Regular;
            }

            if (m_ForcedPositionToStartFrom == null)
            {
                legalIndexesToApplyMoveFrom = m_CheckersLogicManager.FindLegalComputerIndexesToStartMoveFrom(legalMoveType);
            }
            else
            {
                legalIndexesToApplyMoveFrom = new List<int[]>();
                legalIndexesToApplyMoveFrom.Add(m_ForcedPositionToStartFrom);
            }

            List<int[]> indexesOfLegalActions = m_CheckersLogicManager.FindLegalComputerActions(legalIndexesToApplyMoveFrom, legalMoveType);
            Random randomNumberGenerator = new Random();
            int[] randomChosenAction = indexesOfLegalActions[randomNumberGenerator.Next() % indexesOfLegalActions.Count];
            applyActionAndCheckForAnotherEat(legalMoveType, randomChosenAction);
            if (m_ForcedPositionToStartFrom != null)
            {
                playComputerMove();
            }

            return randomChosenAction;
        }

        private void applyActionAndCheckForAnotherEat(CheckersLogicManager.eMoveType i_MoveTypeChosen, int[] i_CurrentAction)
        {
            int[] forcedPosition = { i_CurrentAction[2], i_CurrentAction[3] };
            bool canEatOneMore = false;

            m_CheckersLogicManager.DoAction(i_CurrentAction, i_MoveTypeChosen, m_PlayerIndex);
            if (i_MoveTypeChosen == CheckersLogicManager.eMoveType.Eat)
            {
                canEatOneMore = m_CheckersLogicManager.CheckIfMoveTypeExists(m_PlayerIndex, forcedPosition, CheckersLogicManager.eMoveType.Eat, null);
            }
            if (canEatOneMore)
            {
                m_ForcedPositionToStartFrom = forcedPosition;
            }
            else
            {
                m_ForcedPositionToStartFrom = null;
            }
        }
    }
}
