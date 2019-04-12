using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersLogic
{
    public class CheckersLogicManager
    {
        private readonly CheckersGame m_Game;
        private Board m_Board;

        public enum eMoveType { Eat, Regular, Illegal }
        public CheckersLogicManager(CheckersGame i_Game, Board i_Board)
        {
            m_Game = i_Game;
            m_Board = i_Board;
        }

        // returns the move type of the given action, in relation to the player.
        // assumes the format of the action is good(i.e Af>Bf), with positions inside bounds of array.
        internal eMoveType CheckLegalActionAndReturnType(int[] i_ActionPositions, byte i_PlayerIndex)
        {
            eMoveType actionType;
            bool playerIndexFitsSoldier, moveIsBackwards;
            Board.BoardCell coinAndPlayerTypeFrom = m_Board.GetCell(i_ActionPositions[0], i_ActionPositions[1]);
            Board.BoardCell coinAndPlayerTypeTo = m_Board.GetCell(i_ActionPositions[2], i_ActionPositions[3]);
            playerIndexFitsSoldier = CheckIfPlayerFitsState(coinAndPlayerTypeFrom.PositionState, i_PlayerIndex);
            moveIsBackwards = checkBackwardsMove(i_ActionPositions[0], i_ActionPositions[2], i_PlayerIndex);

            if ((playerIndexFitsSoldier && coinAndPlayerTypeTo.PositionState == Board.ePositionState.EmptyPosition)
                && (coinAndPlayerTypeFrom.IsKing() || !moveIsBackwards))
            {
                actionType = checkValidityDistanceAndTypeOfAction(i_ActionPositions, i_PlayerIndex);
            }
            else
            {
                actionType = eMoveType.Illegal;
            }

            return actionType;
        }

        // checks the action move type by its distance and the coins at the positions.
        private eMoveType checkValidityDistanceAndTypeOfAction(int[] i_ActionPositions, byte i_PlayerIndex)
        {
            eMoveType actionType = eMoveType.Illegal;
            bool moveIsDiagonal;
            byte distanceBetweenActions = checkDistanceOfAction(i_ActionPositions, out moveIsDiagonal);
            
            if (moveIsDiagonal)
            {
                if (distanceBetweenActions == 4)
                {
                    actionType = checkLegalEat(i_ActionPositions, i_PlayerIndex);
                }
                else if (distanceBetweenActions == 2)
                {
                    actionType = eMoveType.Regular;
                }
            }

            return actionType;
        }

        // checks whether the action's move is going backwards(i.e player one goes up / player two goes down)
        private bool checkBackwardsMove(int i_PositionFrom, int i_PositionTo, byte i_PlayerIndex)
        {
            bool playerGoingBackwards = (i_PositionFrom - i_PositionTo) * Math.Pow(-1, i_PlayerIndex) < 0;

            return playerGoingBackwards;
        }

        // checks if the current player fits the position of the coin given
        internal bool CheckIfPlayerFitsState(Board.ePositionState i_CoinAndPlayerTypeFrom, byte i_PlayerIndex)
        {
            bool actionIsLegal;

            if (i_CoinAndPlayerTypeFrom != (Board.ePositionState)(i_PlayerIndex + 1)
                && i_CoinAndPlayerTypeFrom != (Board.ePositionState)(i_PlayerIndex + 3))
            {
                actionIsLegal = false;
            }
            else
            {
                actionIsLegal = true;
            }

            return actionIsLegal;
        }

        // returns the distance between the positions of the action
        private byte checkDistanceOfAction(int[] i_ActionMoves, out bool o_MoveIsDiagonal)
        {
            byte distanceOne = (byte)(Math.Abs(i_ActionMoves[0] - i_ActionMoves[2]));
            byte distanceTwo = (byte)Math.Abs(i_ActionMoves[1] - i_ActionMoves[3]);
            o_MoveIsDiagonal = distanceOne == distanceTwo;
            byte distanceTotal = (byte)(distanceOne + distanceTwo);

            return distanceTotal;
        }

        // checks whether the action is a legal eat move in relation the the player,
        // and returns the move.
        private eMoveType checkLegalEat(int[] i_ActionPositions, byte i_PlayerIndex)
        {
            eMoveType eatOrIllegal;
            int[] positionOfCoinToEat = findCoinPositionToEat(i_ActionPositions);
            Board.BoardCell coinToEat = m_Board.GetCell(positionOfCoinToEat[0], positionOfCoinToEat[1]);

            if (!(CheckIfPlayerFitsState(coinToEat.PositionState, i_PlayerIndex))
                && coinToEat.PositionState != Board.ePositionState.EmptyPosition)
            {
                eatOrIllegal = eMoveType.Eat;
            }
            else
            {
                eatOrIllegal = eMoveType.Illegal;
            }

            return eatOrIllegal;
        }

        // returns the position of the coin to eat(i.e the position in between the two positions of the action)
        private int[] findCoinPositionToEat(int[] i_ActionPositions)
        {
            int[] coinToEatPosition = new int[2];
            coinToEatPosition[0] = i_ActionPositions[0] - ((i_ActionPositions[0] - i_ActionPositions[2]) / 2);
            coinToEatPosition[1] = i_ActionPositions[1] - ((i_ActionPositions[1] - i_ActionPositions[3]) / 2);

            return coinToEatPosition;
        }

        // updates the board with the given action and move type
        internal void DoAction(int[] i_ActionPositions, eMoveType i_MoveType, byte i_PlayerIndex)
        {
            bool newKing, eatMove, killedAKing = false;
            Board.BoardCell cellToMoveCoinFrom = m_Board.GetCell(i_ActionPositions[0], i_ActionPositions[1]);
            Board.BoardCell cellToReceiveCoin = m_Board.GetCell(i_ActionPositions[2], i_ActionPositions[3]);
            cellToReceiveCoin.PositionState = cellToMoveCoinFrom.PositionState;
            cellToMoveCoinFrom.PositionState = Board.ePositionState.EmptyPosition;
            eatMove = i_MoveType == eMoveType.Eat;

            if (eatMove)
            {
                int[] coinToEatPosition = findCoinPositionToEat(i_ActionPositions);
                Board.BoardCell cellToRemoveDeadCoin = m_Board.GetCell(coinToEatPosition[0], coinToEatPosition[1]);
                cellToRemoveDeadCoin.OnPositionChanged(Board.ePositionState.EmptyPosition);
                killedAKing = cellToRemoveDeadCoin.IsKing();
                cellToRemoveDeadCoin.PositionState = Board.ePositionState.EmptyPosition;
            }

            newKing = crownNewKingIfNeeded(cellToReceiveCoin);
            cellToReceiveCoin.OnPositionChanged((Board.ePositionState)(cellToReceiveCoin.PositionState));
            cellToMoveCoinFrom.OnPositionChanged(Board.ePositionState.EmptyPosition);
            updatePointsOfTeams(newKing, i_PlayerIndex, eatMove, killedAKing);
        }
        internal byte? UpdatePointsOfTeamsAndStateOfGame()
        {
            byte? indexOfWinningTeam;
            int firstTeamPoints, secondTeamPoints;

            if (m_Board.SecondTeamValueOfCoins > m_Board.FirstTeamValueOfCoins)
            {
                indexOfWinningTeam = 1;
                firstTeamPoints = 0;
                secondTeamPoints = m_Board.SecondTeamValueOfCoins - m_Board.FirstTeamValueOfCoins;
            }
            else if (m_Board.FirstTeamValueOfCoins > m_Board.SecondTeamValueOfCoins)
            {
                indexOfWinningTeam = 0;
                secondTeamPoints = 0;
                firstTeamPoints = m_Board.FirstTeamValueOfCoins - m_Board.SecondTeamValueOfCoins;
            }
            else
            {
                indexOfWinningTeam = null;
                secondTeamPoints = 0;
                firstTeamPoints = 0;
            }

            m_Game.SecondTeamPoints += secondTeamPoints;
            m_Game.FirstTeamPoints += firstTeamPoints;

            return indexOfWinningTeam;
        }

        private void updatePointsOfTeams(bool i_NewKing, byte i_PlayerIndex, bool i_EatMove, bool i_KilledAKing)
        {
            short[] pointsChange = { 0, 0 };
            if (i_NewKing)
            {
                pointsChange[i_PlayerIndex] += 3;
            }

            if (i_EatMove)
            {
                if (i_KilledAKing)
                {
                    pointsChange[1 - i_PlayerIndex] -= 4;
                }
                else
                {
                    pointsChange[1 - i_PlayerIndex] -= 1;
                }
            }

            m_Board.FirstTeamValueOfCoins += pointsChange[0];
            m_Board.SecondTeamValueOfCoins += pointsChange[1];
        }

        private bool crownNewKingIfNeeded(Board.BoardCell i_CellToReceiveCoin)
        {
            bool isNewKing = !i_CellToReceiveCoin.IsKing()
                && (i_CellToReceiveCoin.Row == 0 || i_CellToReceiveCoin.Row == m_Board.BoardSize - 1);

            if (isNewKing)
            {
                Board.ePositionState stateOfPlayerToCrown = i_CellToReceiveCoin.PositionState;
                i_CellToReceiveCoin.PositionState = (Board.ePositionState)(stateOfPlayerToCrown + 2);
            }

            return isNewKing;
        }

        // checks if the given player has any eat moves available
        internal bool CheckForMoveTypePossibilities(byte i_PlayerIndex, eMoveType i_FirstMoveType, eMoveType? i_SecondMoveType)
        {
            bool canDoMove = false;

            foreach (Board.BoardCell currentCell in m_Board.BoardArray)
            {
                if (CheckIfPlayerFitsState(currentCell.PositionState, i_PlayerIndex))
                {
                    int[] positionToCheck = { currentCell.Row, currentCell.Column };
                    canDoMove = CheckIfMoveTypeExists(i_PlayerIndex, positionToCheck, i_FirstMoveType, i_SecondMoveType);
                    if (canDoMove)
                    {
                        break;
                    }
                }
            }

            return canDoMove;
        }

        // checks if the given position has any eat action available
        internal bool CheckIfMoveTypeExists(byte i_PlayerIndex, int[] i_ActionFrom, eMoveType i_MoveToCheck, eMoveType? i_SecondMoveToCheck)
        {
            bool hasMoveOption = false;
            eMoveType eMoveType = CheckersLogicManager.eMoveType.Illegal;
            ushort currentActionIndex = 0;
            short[,] positionToGo = GenerateMovingPositions();

            while ((currentActionIndex < 8) && (eMoveType == CheckersLogicManager.eMoveType.Illegal))
            {
                int[] actionOfNewPositions = GenerateActionPosition(i_ActionFrom,
                    positionToGo[currentActionIndex, 0], positionToGo[currentActionIndex, 1]);
                bool inBoundsOfArray = checkIfActionInBoundsOfArray(actionOfNewPositions);
                if (inBoundsOfArray)
                {
                    eMoveType = CheckLegalActionAndReturnType(actionOfNewPositions, i_PlayerIndex);
                    if (eMoveType == i_MoveToCheck || eMoveType == i_SecondMoveToCheck)
                    {
                        hasMoveOption = true;
                        break;
                    }
                }
                currentActionIndex++;
            }

            return hasMoveOption;
        }

        private bool checkIfActionInBoundsOfArray(int[] i_ActionOfNewPositions)
        {
            bool actionInBoundsOfArray = true;

            foreach (int indexOfAction in i_ActionOfNewPositions)
            {
                actionInBoundsOfArray &= indexOfAction < m_Board.BoardSize
                    && indexOfAction >= 0;
            }

            return actionInBoundsOfArray;
        }

        internal int[] GenerateActionPosition(int[] i_ActionFrom, short i_FirstPositionIndex, short i_SecondPositionIndex)
        {
            int[] actionOfNewPositions = new int[4];
            actionOfNewPositions[0] = (i_ActionFrom[0]);
            actionOfNewPositions[1] = (i_ActionFrom[1]);
            actionOfNewPositions[2] = (i_ActionFrom[0] + i_FirstPositionIndex);
            actionOfNewPositions[3] = (i_ActionFrom[1] + i_SecondPositionIndex);

            return actionOfNewPositions;
        }

        internal short[,] GenerateMovingPositions()
        {
            short[,] availablePositionMovements = new short[,] {
                {2,2},
                {2,-2},
                {-2,2},
                {-2,-2},
                {1,1},
                {1,-1 },
                {-1,1 },
                {-1,-1 }
                };

            return availablePositionMovements;
        }

        internal bool CheckIfGameEnds()
        {
            bool gameEnded = m_Board.FirstTeamValueOfCoins == 0 || m_Board.SecondTeamValueOfCoins == 0
                || !CheckForMoveTypePossibilities(0, eMoveType.Eat, eMoveType.Regular)
                || !CheckForMoveTypePossibilities(1, eMoveType.Eat, eMoveType.Regular);

            return gameEnded;
        }

        internal List<int[]> FindLegalComputerActions(List<int[]> i_LegalIndexesToApplyMoveFrom, eMoveType i_MoveType)
        {
            short[,] positionChanges = GenerateMovingPositions();
            List<int[]> legalActionMoves = new List<int[]>();

            foreach (int[] currentLegalIndexToApplyMoveFrom in i_LegalIndexesToApplyMoveFrom)
            {
                for (int currentPositionChange = 0; currentPositionChange < 8; currentPositionChange++)
                {
                    int[] currentAction = GenerateActionPosition(currentLegalIndexToApplyMoveFrom, positionChanges[currentPositionChange, 0]
                        , positionChanges[currentPositionChange, 1]);
                    bool inBoundsOfArray = checkIfActionInBoundsOfArray(currentAction);
                    if (inBoundsOfArray && CheckLegalActionAndReturnType(currentAction, 1) == i_MoveType)
                    {
                        legalActionMoves.Add(currentAction);
                    }
                }
            }

            return legalActionMoves;
        }

        internal List<int[]> FindLegalComputerIndexesToStartMoveFrom(eMoveType i_MoveType)
        {
            List<int[]> legalIndexesToApplyMoveFrom = new List<int[]>();

            foreach (Board.BoardCell cellToCheck in m_Board.BoardArray)
            {
                if (CheckIfPlayerFitsState(cellToCheck.PositionState, 1))
                {
                    int[] coinIndex = { cellToCheck.Row, cellToCheck.Column };
                    if (CheckIfMoveTypeExists(1, coinIndex, i_MoveType, null))
                    {
                        legalIndexesToApplyMoveFrom.Add(coinIndex);
                    }
                }
            }

            return legalIndexesToApplyMoveFrom;
        }
    }
}
