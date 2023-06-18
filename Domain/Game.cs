using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Text;

namespace MegaKnightChess.Domain
{
    public class Game
    {
        public Game(int boardSize)
        {
            MyBoard = new Board(boardSize);
            EncodedBoardStatements.Add(EncodeBoard(MyBoard));
            ColourOfCurrentPlayer = Colour.White;
            WhiteStatus = StatusType.Non;
            BlackStatus = StatusType.Non;
        }

        private bool StateChanged;

        public Board MyBoard { get; private set; }

        public int CurrentTurn = 0;

        public Colour ColourOfCurrentPlayer { get; private set; }

        public List<List<Dictionary<FigureType, List<Point>>>> EncodedBoardStatements = new();

        private int DecodeCounts = 0;

        public Cell? CurrentChoosenCell { get; private set; }

        public Cell? PreviousChoosenCell { get; private set; }

        public StatusType WhiteStatus { get; private set; }

        public StatusType BlackStatus { get; private set; }

        private List<Dictionary<FigureType, List<Point>>> StartEncodedBoardStatement = new();

        public void ClearGame()
        {
            MyBoard = new Board(MyBoard.Size);
            ColourOfCurrentPlayer = Colour.White;
            WhiteStatus = StatusType.Non;
            BlackStatus = StatusType.Non;
            CurrentTurn = 0;
            EncodedBoardStatements.Clear();
            CurrentChoosenCell = null;
            PreviousChoosenCell = null;
        }

        public void StartDefaultGame()
        {
            MyBoard.FillStandartStarterBoard();
            CurrentTurn = 0;
            StartEncodedBoardStatement = EncodeBoard(MyBoard);
        }

        public void StartKnightMadnessGame()
        {
            MyBoard.FillKnightMadnessStarterBoard();
            CurrentTurn = 0;
            StartEncodedBoardStatement = EncodeBoard(MyBoard);
        }

        public void StartKnightInfectionGame()
        {
            MyBoard.FillKnightInfectionBoard();
            CurrentTurn = 0;
            StartEncodedBoardStatement = EncodeBoard(MyBoard);
        }

        private void UpdateColourOfCurrentPlayer()
        {
            if (ColourOfCurrentPlayer == Colour.White) ColourOfCurrentPlayer = Colour.Black;
            else ColourOfCurrentPlayer = Colour.White;
        }

        private void NewTurn()
        {
            if (CurrentTurn + 1 < EncodedBoardStatements.Count)
            {
                while(CurrentTurn + 1 < EncodedBoardStatements.Count)
                {
                    EncodedBoardStatements.RemoveAt(EncodedBoardStatements.Count - 1);
                }
                DecodeCounts = 0;
            }
            AddEncodedBoardStatement(EncodeBoard(MyBoard));
            CurrentTurn++;
            UpdateColourOfCurrentPlayer();
            BlackStatus = MyBoard.StatusChecking(Colour.Black);
            WhiteStatus = MyBoard.StatusChecking(Colour.White);
        }

        public void BackTurn()
        {
            if (CurrentTurn <= 0) return;
            CurrentChoosenCell = null;
            MyBoard = CurrentTurn == 1? DecodeBoard(StartEncodedBoardStatement) : DecodeBoard(EncodedBoardStatements[CurrentTurn - 1]);
            CurrentTurn--;
            DecodeCounts++;
            UpdateColourOfCurrentPlayer();
            BlackStatus = MyBoard.StatusChecking(Colour.Black);
            WhiteStatus = MyBoard.StatusChecking(Colour.White);
        }

        public void NextTurn()
        {
            if (DecodeCounts == 0) return;
            CurrentChoosenCell = null;
            MyBoard = DecodeBoard(EncodedBoardStatements[CurrentTurn + 1]);
            CurrentTurn++;
            DecodeCounts--;
            UpdateColourOfCurrentPlayer();
            BlackStatus = MyBoard.StatusChecking(Colour.Black);
            WhiteStatus = MyBoard.StatusChecking(Colour.White);
        }

        private void TakeCurrentCell(Action instructions)
        {
            if (CurrentChoosenCell == null) throw new ArgumentNullException();
            MyBoard.MakeNextLegalCells(CurrentChoosenCell, ColourOfCurrentPlayer);
            StateChanged = true;
            instructions();
        }

        private void TryMove(Action instructions)
        {
            if (CurrentChoosenCell == null || PreviousChoosenCell == null) throw new ArgumentNullException();
            if (CurrentChoosenCell.LegalNextMove)
            {
                MyBoard.MakeMove(PreviousChoosenCell.X, PreviousChoosenCell.Y, CurrentChoosenCell.X, CurrentChoosenCell.Y);
                NewTurn();
            }
            else TakeCurrentCell(instructions);
        }

        public void ClickCell(Button pressedButton, Action<Button> instructions1, Action instructions2)
        {
            var x = pressedButton.Location.X / 75;
            var y = pressedButton.Location.Y / 75;
            PreviousChoosenCell = CurrentChoosenCell;
            CurrentChoosenCell = MyBoard.Cells[x, y];
            instructions1(pressedButton);
            if (PreviousChoosenCell == CurrentChoosenCell)
            {
                TakeCurrentCell(instructions2);
                return;
            }
            if (PreviousChoosenCell == null)
            { 
                TakeCurrentCell(instructions2);
                return;
            }
            if (PreviousChoosenCell.Figure == null)
            {
                TakeCurrentCell(instructions2);
                return;
            }
            if (PreviousChoosenCell.Figure.Colour != ColourOfCurrentPlayer)
            {
                TakeCurrentCell(instructions2);
                return;
            }
            TryMove(instructions2);
            StateChanged = true;
            instructions2();
        }

        public Board DecodeBoard(List<Dictionary<FigureType, List<Point>>> allFiguresLocations)
        {
            var decodeBoard = new Board(MyBoard.Size);
            for(int i = 0; i < 2; i++)
            {
                foreach(var figureLocations in allFiguresLocations[i])
                {
                    foreach(var location in figureLocations.Value)
                    {
                        decodeBoard.PutFigure(new Figure(i == 0? Colour.White : Colour.Black, figureLocations.Key), location.X, location.Y);
                        if (figureLocations.Key == FigureType.King)
                        {
                            if (i == 0) decodeBoard.WhiteKingLocation = decodeBoard.Cells[location.X, location.Y];
                            else decodeBoard.BlackKingLocation = decodeBoard.Cells[location.X, location.Y];
                        }
                    }
                }
            }
            return decodeBoard;
        }

        public int? GetTypeNumberOrNull(FigureType type)
        {
            switch (type)
            {
                case FigureType.Pawn:
                    return 0;
                case FigureType.Knight:
                    return 1;
                case FigureType.EliteKnight:
                    return 2;
                case FigureType.Megaknight:
                    return 3;
                case FigureType.King:
                    return 4;
            }
            return null;
        }

        public List<Dictionary<FigureType, List<Point>>> EncodeBoard(Board board)
        {
            var whiteFiguresLocations = new Dictionary<FigureType, List<Point>>();
            var blackFiguresLocations = new Dictionary<FigureType, List<Point>>();
            for (int x = 0; x < board.Size; x++)
            {
                for(int y = 0; y < board.Size; y++)
                {
                    if (board.Cells[x, y].HasFigure())
                    {
                        if (board.Cells[x, y].Figure.Colour == Colour.Black)
                        {
                            if (blackFiguresLocations.Keys.Contains(board.Cells[x, y].Figure.Type))
                                blackFiguresLocations[board.Cells[x, y].Figure.Type].Add(new Point(x, y));
                            else blackFiguresLocations.Add(board.Cells[x, y].Figure.Type, new List<Point> { new Point(x, y) } );
                        }  
                        else
                        {
                            if (whiteFiguresLocations.Keys.Contains(board.Cells[x, y].Figure.Type))
                                whiteFiguresLocations[board.Cells[x, y].Figure.Type].Add(new Point(x, y));
                            else whiteFiguresLocations.Add(board.Cells[x, y].Figure.Type, new List<Point> { new Point(x, y) });
                        }
                    }
                }
            }
            var AllFiguresLocations = new List<Dictionary<FigureType, List<Point>>>
            { whiteFiguresLocations, blackFiguresLocations};
            return AllFiguresLocations;
        }

        private void AddEncodedBoardStatement(List<Dictionary<FigureType, List<Point>>> state)
        {
            EncodedBoardStatements.Add(state);
        }
    }
}
