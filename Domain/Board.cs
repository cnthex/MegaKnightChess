

using Microsoft.VisualBasic.Devices;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;

namespace MegaKnightChess.Domain
{
    public class Board
    {
        public int Size { get; set; }

        public Cell[,] Cells { get; set; }

        public Cell? WhiteKingLocation { get; set; }

        public Cell? BlackKingLocation { get; set; }

        private List<Cell> AttackFigures = new();

        public Board(int size)
        {
            if (size % 2 == 1 || size < 6) throw new ImpossibleBoardException();
            Size = size;
            Cells = new Cell[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Cell cell = new(i, j);
                    cell.BlockedForMove = false;
                    Cells[i, j] = cell;
                }
            }
            WhiteKingLocation = null;
            BlackKingLocation = null;
        }

        public void FillStandartStarterBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                Cells[i, 1].Figure = new Figure(Colour.Black, FigureType.Pawn);
                Cells[i, Size - 2].Figure = new Figure(Colour.White, FigureType.Pawn);
                Cells[i, 0].Figure = new Figure(Colour.Black, FigureType.Knight);
                Cells[i, Size - 1].Figure = new Figure(Colour.White, FigureType.Knight);
            }
            Cells[Size / 2, 0].Figure = new Figure(Colour.Black, FigureType.King);
            BlackKingLocation = Cells[Size / 2, 0];
            Cells[Size / 2, Size - 1].Figure = new Figure(Colour.White, FigureType.King);
            WhiteKingLocation = Cells[Size / 2, Size - 1];
            Cells[Size / 2 - 1, 0].Figure = new Figure(Colour.Black, FigureType.Megaknight);
            Cells[Size / 2 - 1, Size - 1].Figure = new Figure(Colour.White, FigureType.Megaknight);
            Cells[Size / 2 - 2, 0].Figure = new Figure(Colour.Black, FigureType.EliteKnight);
            Cells[Size / 2 - 2, Size - 1].Figure = new Figure(Colour.White, FigureType.EliteKnight);
            Cells[Size / 2 + 1, 0].Figure = new Figure(Colour.Black, FigureType.EliteKnight);
            Cells[Size / 2 + 1, Size - 1].Figure = new Figure(Colour.White, FigureType.EliteKnight);
        }

        public void FillKnightMadnessStarterBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                Cells[i, Size - 2].Figure = new Figure(Colour.White, FigureType.Knight);
                Cells[i, 1].Figure = new Figure(Colour.Black, FigureType.Knight);
                Cells[i, Size - 1].Figure = new Figure(Colour.White, FigureType.EliteKnight);
                Cells[i, 0].Figure = new Figure(Colour.Black, FigureType.EliteKnight);
            }
            Cells[Size / 2, 0].Figure = new Figure(Colour.Black, FigureType.King);
            BlackKingLocation = Cells[Size / 2, 0];
            Cells[Size / 2, Size - 1].Figure = new Figure(Colour.White, FigureType.King);
            WhiteKingLocation = Cells[Size / 2, Size - 1];
            Cells[0, Size - 1].Figure = new Figure(Colour.White, FigureType.Megaknight);
            Cells[0, 0].Figure = new Figure(Colour.Black, FigureType.Megaknight);
            Cells[Size - 1, Size - 1].Figure = new Figure(Colour.White, FigureType.Megaknight);
            Cells[Size - 1, 0].Figure = new Figure(Colour.Black, FigureType.Megaknight);
        }

        public void FillKnightInfectionBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                Cells[i, Size - 2].Figure = new Figure(Colour.White, FigureType.Knight);
                Cells[i, Size - 1].Figure = new Figure(Colour.White, FigureType.Knight);
            }
            Cells[Size / 2, 0].Figure = new Figure(Colour.Black, FigureType.King);
            BlackKingLocation = Cells[Size / 2, 0];
            Cells[Size / 2, Size - 1].Figure = new Figure(Colour.White, FigureType.King);
            WhiteKingLocation = Cells[Size / 2, Size - 1];
            Cells[Size - 1, 0].Figure = new Figure(Colour.Black, FigureType.Megaknight);
            Cells[0, 0].Figure = new Figure(Colour.Black, FigureType.Megaknight);
        }

        private bool IsPointOnTheBoard(int x, int y)
        {
            return (x >= 0 && x < Size && y >= 0 && y < Size);
        }

        private void PoachEnemyKnightsAfterMegaknightMove(Cell currentCell)
        {
            for(int i = currentCell.X - 1; i <= currentCell.X + 1; i++)
            {
                for(int j = currentCell.Y - 1; j <= currentCell.Y + 1; j++)
                {
                    if(IsPointOnTheBoard(i,j))
                    {
                        if (Cells[i, j].CheckFigureType() == FigureType.Knight)
                            Cells[i, j].Figure.Colour = currentCell.Figure.Colour;
                    }
                }
            }
        }

        //методы, работающие с NextLegalMove
        private void MakeAllNotLegalNextMove()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Cells[x, y].LegalNextMove = false;
                }
            }
        }

        public void MakeNextLegalCells(Cell currentCell, Colour colourOfCurrentPlayer)
        {
            GetAttackFigures(colourOfCurrentPlayer);
            MakeAllNotLegalNextMove();
            MakeAllNotBlocked();
            MakeBlockedCellsForFigure(currentCell, colourOfCurrentPlayer);
            switch (currentCell.CheckFigureType())
            {
                case FigureType.Pawn:
                    if (IsDoubleCheck()) break;
                    switch (currentCell.CheckFigureColour())
                    {
                        case Colour.Black:
                            MakeNextLegalBlackPawnCase(currentCell);
                            break;
                        case Colour.White:
                            MakeNextLegalWhitePawnCase(currentCell);
                            break;
                    }
                    break;

                case FigureType.King:
                    MakeNextLegalKingCase(currentCell);
                    break;

                case FigureType.Knight:
                    if (IsDoubleCheck()) break;
                    MakeNextLegalKnightCase(currentCell);
                    break;

                case FigureType.Megaknight:
                    MakeNextLegalMegaknightCase(currentCell);
                    break;
                case FigureType.EliteKnight:
                    MakeNextLegalEliteKnightCase(currentCell);
                    break;
                case null:
                    break;
            }
            AttackFigures.Clear();
        }

        private void MakeNextLegalWhitePawnCase(Cell currentCell)
        {
            if (!Cells[currentCell.X, currentCell.Y - 1].BlockedForMove &&
                        IsPointOnTheBoard(currentCell.X, currentCell.Y - 1))
            {
                Cells[currentCell.X, currentCell.Y - 1].LegalNextMove = true;
                if (currentCell.Y == Size - 2)
                {
                    if (!Cells[currentCell.X, currentCell.Y - 2].BlockedForMove)
                        Cells[currentCell.X, currentCell.Y - 2].LegalNextMove = true;
                }
            }
            if (IsPointOnTheBoard(currentCell.X + 1, currentCell.Y - 1))
            {
                if (Cells[currentCell.X + 1, currentCell.Y - 1].HasFigure())
                {
                    if (Cells[currentCell.X + 1, currentCell.Y - 1].CheckFigureColour() != Colour.White)
                        Cells[currentCell.X + 1, currentCell.Y - 1].LegalNextMove = true;
                }
            }
            if (IsPointOnTheBoard(currentCell.X - 1, currentCell.Y - 1))
            {
                if (Cells[currentCell.X - 1, currentCell.Y - 1].HasFigure())
                {
                    if (Cells[currentCell.X - 1, currentCell.Y - 1].CheckFigureColour() != Colour.White)
                        Cells[currentCell.X - 1, currentCell.Y - 1].LegalNextMove = true;
                }
            }
            RemakeLegalIfCheck(currentCell);
        }

        private void MakeNextLegalBlackPawnCase(Cell currentCell)
        {
            if (!Cells[currentCell.X, currentCell.Y + 1].BlockedForMove &&
                        IsPointOnTheBoard(currentCell.X, currentCell.Y + 1))
            {
                Cells[currentCell.X, currentCell.Y + 1].LegalNextMove = true;
                if (currentCell.Y == 1)
                    if (!Cells[currentCell.X, currentCell.Y + 2].BlockedForMove &&
                        IsPointOnTheBoard(currentCell.X, currentCell.Y + 2))
                        Cells[currentCell.X, currentCell.Y + 2].LegalNextMove = true;
            }
            if (IsPointOnTheBoard(currentCell.X + 1, currentCell.Y + 1))
            {
                if (Cells[currentCell.X + 1, currentCell.Y + 1].Figure != null)
                {
                    if (Cells[currentCell.X + 1, currentCell.Y + 1].Figure.Colour != Colour.Black)
                        Cells[currentCell.X + 1, currentCell.Y + 1].LegalNextMove = true;
                }
            }
            if (IsPointOnTheBoard(currentCell.X - 1, currentCell.Y + 1))
            {
                if (Cells[currentCell.X - 1, currentCell.Y + 1].Figure != null)
                {
                    if (Cells[currentCell.X - 1, currentCell.Y + 1].Figure.Colour != Colour.Black)
                        Cells[currentCell.X - 1, currentCell.Y + 1].LegalNextMove = true;
                }
            }
            RemakeLegalIfCheck(currentCell);
        }

        private void MakeNextLegalKnightCase(Cell currentCell)
        {
            var stepPoints = new int[] { -2, -1, 1, 2 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if ((i + j) % 2 != 0 &&
                        IsPointOnTheBoard(currentCell.X + i, currentCell.Y + j) &&
                        !Cells[currentCell.X + i, currentCell.Y + j].BlockedForMove)
                        Cells[currentCell.X + i, currentCell.Y + j].LegalNextMove = true;
                }
            }
            RemakeLegalIfCheck(currentCell);
        }

        private void MakeNextLegalEliteKnightCase(Cell currentCell)
        {
            var stepPoints = new int[] { -1, 1 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if (IsPointOnTheBoard(currentCell.X + 2 * i, currentCell.Y + 2 * j) &&
                        !Cells[currentCell.X + 2 * i, currentCell.Y + 2 * j].BlockedForMove)
                        Cells[currentCell.X + 2 * i, currentCell.Y + 2 * j].LegalNextMove = true;
                }
                if (IsPointOnTheBoard(currentCell.X + 3 * i, currentCell.Y) &&
                        !Cells[currentCell.X + 3 * i, currentCell.Y].BlockedForMove)
                    Cells[currentCell.X + 3 * i, currentCell.Y].LegalNextMove = true;
                if (IsPointOnTheBoard(currentCell.X, currentCell.Y + 3 * i) &&
                        !Cells[currentCell.X, currentCell.Y + 3 * i].BlockedForMove)
                    Cells[currentCell.X, currentCell.Y + 3 * i].LegalNextMove = true;
            }
            RemakeLegalIfCheck(currentCell);
        }

        private void MakeNextLegalMegaknightCase(Cell currentCell)
        {
            var stepPoints = new int[] { -2, -1, 1, 2 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if ((i + j) % 2 != 0 &&
                        IsPointOnTheBoard(currentCell.X + i, currentCell.Y + j) &&
                        !Cells[currentCell.X + i, currentCell.Y + j].BlockedForMove)
                        Cells[currentCell.X + i, currentCell.Y + j].LegalNextMove = true;
                }
            }
            RemakeLegalIfCheck(currentCell);
        }

        private void MakeNextLegalKingCase(Cell currentCell)
        {
            var stepPoints = new int[] { -1, 0, 1 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if (IsPointOnTheBoard(currentCell.X + i, currentCell.Y + j) &&
                        !Cells[currentCell.X + i, currentCell.Y + j].BlockedForMove &&
                        !(i == 0 && j == 0))
                        Cells[currentCell.X + i, currentCell.Y + j].LegalNextMove = true;
                }
            }
        }

        //методы, обрабатывающие BlockedForMove
        private void MakeAllNotBlocked()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Cells[x, y].BlockedForMove = false;
                }
            }
        }

        private void MakeBlockedCellsForFigure(Cell currentCell, Colour colourOfCurrentPlayer)
        {
            if (currentCell.Figure != null)
            {
                var isCurrentFigureKing = false;
                var oppositeColour = colourOfCurrentPlayer == Colour.White ? Colour.Black : Colour.White;
                if (currentCell.Figure.Type == FigureType.King && currentCell.Figure.Colour == colourOfCurrentPlayer)
                {
                    MakeBlockedAttackCells(oppositeColour);
                    isCurrentFigureKing = true;
                }
                for (int x = 0; x < Size; x++)
                {
                    for (int y = 0; y < Size; y++)
                    {
                        Cells[x, y].LegalNextMove = false;
                        if (!isCurrentFigureKing) Cells[x, y].BlockedForMove = false;
                        MakeBlockedIfFriendlyFigure(currentCell, x, y);
                        if (currentCell.CheckFigureType() == FigureType.Pawn)
                            MakeBlockedForPawn(currentCell, x, y);
                    }
                }
            }
        }

        private void MakeBlockedIfFriendlyFigure(Cell currentCell, int x, int y)
        {
            if (Cells[x, y].CheckFigureColour() == currentCell.CheckFigureColour())
            {
                Cells[x, y].BlockedForMove = true;
                return;
            }
        }

        private void MakeBlockedForPawn(Cell currentCell, int x, int y)
        {
            if (!Cells[x, y].HasFigure()) return;
            if (currentCell.CheckFigureColour() == Colour.White && currentCell.CheckFigureType() == FigureType.Pawn)
                if (y == currentCell.Y - 1 || y == currentCell.Y - 2) Cells[x, y].BlockedForMove = true;
            if (currentCell.CheckFigureColour() == Colour.Black && currentCell.CheckFigureType() == FigureType.Pawn)
                if (y == currentCell.Y + 1 || y == currentCell.Y + 2) Cells[x, y].BlockedForMove = true;
        }


        private void MakeBlockedAttackCells(Colour oppositeColour)
        {
            MakeAllNotBlocked();
            for (int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    if (Cells[i,j].CheckFigureColour() == oppositeColour)
                        BlockAttackCellsByFigure(Cells[i, j]);
                }
            }
        }

        private void BlockAttackCellsByFigure(Cell currentCell)
        {
            switch (currentCell.CheckFigureType())
            {
                case FigureType.Pawn:
                    switch (currentCell.CheckFigureColour())
                    {
                        case Colour.Black:
                            BlockAttackCellsBlackPawnCase(currentCell);
                            break;
                        case Colour.White:
                            BlockAttackCellsWhitePawnCase(currentCell);
                            break;
                    }
                    break;

                case FigureType.King:
                    BlockAttackCellsKingCase(currentCell);
                    break;

                case FigureType.Knight:
                    BlockAttackCellsKnightAndMegaknightCase(currentCell);
                    break;

                case FigureType.EliteKnight:
                    BlockAttackCellsEliteKnightCase(currentCell);
                    break;

                case FigureType.Megaknight:
                    BlockAttackCellsKnightAndMegaknightCase(currentCell);
                    break;
            }
        }

        private void BlockAttackCellsBlackPawnCase(Cell currentCell)
        {
            if (IsPointOnTheBoard(currentCell.X - 1, currentCell.Y + 1)) 
                Cells[currentCell.X - 1, currentCell.Y + 1].BlockedForMove = true;
            if (IsPointOnTheBoard(currentCell.X + 1, currentCell.Y + 1))
                Cells[currentCell.X + 1, currentCell.Y + 1].BlockedForMove = true;
        }

        private void BlockAttackCellsWhitePawnCase(Cell currentCell)
        {
            if (IsPointOnTheBoard(currentCell.X - 1, currentCell.Y - 1))
                Cells[currentCell.X - 1, currentCell.Y - 1].BlockedForMove = true;
            if (IsPointOnTheBoard(currentCell.X + 1, currentCell.Y - 1))
                Cells[currentCell.X + 1, currentCell.Y - 1].BlockedForMove = true;
        }

        private void BlockAttackCellsKnightAndMegaknightCase(Cell currentCell)
        {
            var stepPoints = new int[] { -2, -1, 1, 2 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if ((i + j) % 2 != 0 &&
                    IsPointOnTheBoard(currentCell.X + i, currentCell.Y + j))
                        Cells[currentCell.X + i, currentCell.Y + j].BlockedForMove = true;
                }
            }
        }

        private void BlockAttackCellsEliteKnightCase(Cell currentCell)
        {
            var stepPoints = new int[] { -1, 1 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if (IsPointOnTheBoard(currentCell.X + 2 * i, currentCell.Y + 2 * j))
                        Cells[currentCell.X + 2 * i, currentCell.Y + 2 * j].BlockedForMove = true;
                }
                if (IsPointOnTheBoard(currentCell.X + 3 * i, currentCell.Y))
                    Cells[currentCell.X + 3 * i, currentCell.Y].BlockedForMove = true;
                if (IsPointOnTheBoard(currentCell.X, currentCell.Y + 3 * i))
                    Cells[currentCell.X, currentCell.Y + 3 * i].BlockedForMove = true;
            }
        }

        private void BlockAttackCellsKingCase(Cell currentCell)
        {
            var stepPoints = new int[] { -1, 0, 1 };
            foreach (int i in stepPoints)
            {
                foreach (int j in stepPoints)
                {
                    if (IsPointOnTheBoard(currentCell.X + i, currentCell.Y + j) &&
                        !(i == 0 && j == 0))
                    {
                        Cells[currentCell.X + i, currentCell.Y + j].BlockedForMove = true;
                    }
                }     
            }      
        }

        //методы движения фигур:
        public void PutFigure(Figure figure, int x, int y)
        {
            if (x >= Size || y >= Size) throw new OutOfBoardException();
            Cells[x, y].Figure = figure;
        }

        public void DeleteFigure(int x, int y)
        {
            if (x >= Size || y >= Size) throw new OutOfBoardException();
            Cells[x, y].Figure = null;
        }

        public void MakeMove(int currentX, int currentY, int moveX, int moveY)
        {
            var currentFigure = Cells[currentX, currentY].Figure;
            if (currentFigure != null)
            {
                if (currentFigure.Type == FigureType.Pawn && (moveY == 0 || moveY == Size - 1))
                    PutFigure(new Figure(currentFigure.Colour, FigureType.Megaknight), moveX, moveY);
                else
                {
                    PutFigure(currentFigure, moveX, moveY);
                    if (Cells[moveX, moveY].Figure.Type == FigureType.Megaknight)
                        PoachEnemyKnightsAfterMegaknightMove(Cells[moveX, moveY]);
                }
                DeleteFigure(currentX, currentY);
                if (Cells[moveX,moveY].Figure.Type == FigureType.King)
                {
                    if (Cells[moveX, moveY].Figure.Colour == Colour.White)
                        WhiteKingLocation = Cells[moveX, moveY];
                    else BlackKingLocation = Cells[moveX, moveY];
                }
            }
            var oppositeColour = Cells[moveX, moveY].CheckFigureColour() == Colour.White ? Colour.Black : Colour.White;
            MakeBlockedCellsForFigure(Cells[moveX, moveY], oppositeColour);
            MakeAllNotLegalNextMove();
            AttackFigures.Clear();
        }

        //методы, обеспечивающие правильность работы инфо методов при шахе/мате/пате 
        private void ReamkeLegalIfDoubleCheckMegaknightCase()
        {
            if (AttackFigures[0].CheckFigureType() != FigureType.Knight &&
                AttackFigures[1].CheckFigureType() != FigureType.Knight)
                return;
            var knight1 = AttackFigures[0];
            var knight2 = AttackFigures[1];
            if (!(Math.Abs(knight1.X - knight2.X) <= 2 && Math.Abs(knight1.X - knight2.X) <= 2))
                return;
            var legal = new List<Cell>();
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (Math.Abs(x - knight2.X) <= 1 && Math.Abs(x - knight1.X) <= 1 &&
                        Math.Abs(y - knight2.Y) <= 1 && Math.Abs(y - knight1.Y) <= 1 &&
                        Cells[x, y].LegalNextMove)
                        legal.Add(Cells[x, y]);
                }
            }
            MakeAllNotLegalNextMove();
            foreach (var cell in legal)
            {
                Cells[cell.X, cell.Y].LegalNextMove = true;
            }
        }

        private void RemakeLegalIfCheckMegaknightCase()
        {
            if (AttackFigures.Count > 1)
            {
                ReamkeLegalIfDoubleCheckMegaknightCase();
                return;
            }
            var legal = new List<Cell>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (IsPointOnTheBoard(AttackFigures[0].X + x, AttackFigures[0].Y + y) &&
                        Cells[AttackFigures[0].X + x, AttackFigures[0].Y + y].LegalNextMove)
                        legal.Add(Cells[AttackFigures[0].X + x, AttackFigures[0].Y + y]);
                }
            }
            MakeAllNotLegalNextMove();
            foreach (var cell in legal)
            {
                Cells[cell.X, cell.Y].LegalNextMove = true;
            }
        }

        private void RemakeLegalIfCheck(Cell currentCell)
        {
            if (AttackFigures.Count > 0)
            {
                if (AttackFigures[0].CheckFigureType() == FigureType.Knight &&
                    currentCell.CheckFigureType() == FigureType.Megaknight)
                {
                    RemakeLegalIfCheckMegaknightCase();
                }
                else if (Cells[AttackFigures[0].X, AttackFigures[0].Y].LegalNextMove)
                {
                    MakeAllNotLegalNextMove();
                    Cells[AttackFigures[0].X, AttackFigures[0].Y].LegalNextMove = true;
                }
                else MakeAllNotLegalNextMove();
            }
        }

        private void MakeOccupiedForKingBecauseCheck(Colour colourOfCurrentPlayer)
        {
            MakeAllNotBlocked();
            var oppositeColour = colourOfCurrentPlayer == Colour.White ? Colour.Black : Colour.White;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Cells[i, j].CheckFigureColour() == oppositeColour)
                        BlockAttackCellsByFigure(Cells[i, j]);
                }
            }
        }

        private void GetAttackFigures(Colour currentColour)
        {
            var currentKing = currentColour == Colour.White ? WhiteKingLocation : BlackKingLocation;
            for(int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    if (Cells[i,j].HasFigure() && Cells[i, j].CheckFigureColour() != currentColour)
                    {
                        MakeAllNotBlocked();
                        BlockAttackCellsByFigure(Cells[i, j]);
                        if (Cells[currentKing.X, currentKing.Y].BlockedForMove) AttackFigures.Add(Cells[i, j]);
                        if (AttackFigures.Count >= 2) return;
                    }
                }
            }
            MakeAllNotBlocked();
        }

        private bool IsDoubleCheck()
        {
            if (AttackFigures.Count == 2) return true;
            return false;
        }

        private bool CanKingMove(Cell currentKing)
        {
            MakeOccupiedForKingBecauseCheck(currentKing.Figure.Colour);
            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    if (IsPointOnTheBoard(currentKing.X + x, currentKing.Y + y) && (x != 0 || y != 0))
                    {
                        if (!Cells[currentKing.X + x, currentKing.Y + y].BlockedForMove &&
                            Cells[currentKing.X + x, currentKing.Y + y].CheckFigureColour() != currentKing.Figure.Colour) 
                            return true;
                    }
                }
            }
            return false;
        }

        private bool CanFigureMove(Cell currentCell)
        {
            if (!currentCell.HasFigure()) return false;
            MakeNextLegalCells(currentCell, currentCell.Figure.Colour);
            //if (currentCell.Figure.Type == FigureType.King) return CanKingMove(currentCell);
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Cells[i, j].LegalNextMove) return true;
                }
            }
            return false;
        }

        private bool AnyFigureCanMove(Colour currentColour)
        {
            for(int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    if (Cells[i, j].CheckFigureColour() == currentColour)
                        if(CanFigureMove(Cells[i, j])) return true;
                }
            }
            return false;
        }

        public StatusType StatusChecking(Colour colourOfCurrentPlayer)
        {
            StatusType status;
            var oppositeColour = colourOfCurrentPlayer == Colour.White ? Colour.Black : Colour.White;
            var currentKing = colourOfCurrentPlayer == Colour.White ? WhiteKingLocation : BlackKingLocation;
            var kingCanMove = CanKingMove(currentKing);
            GetAttackFigures(colourOfCurrentPlayer);
            if (IsDoubleCheck())
            {
                if (!kingCanMove)
                    status = StatusType.Mate;
                else status = StatusType.Check;
            }
            else
            {
                if (AttackFigures.Count > 0)
                {
                    if (!AnyFigureCanMove(colourOfCurrentPlayer)) status = StatusType.Mate;
                    else status = StatusType.Check;
                }
                else
                {
                    if (!AnyFigureCanMove(colourOfCurrentPlayer))
                        status = StatusType.Stalemate;
                    else status = StatusType.Non;
                }
            }
            AttackFigures.Clear();
            return status;
        }
    }
}
