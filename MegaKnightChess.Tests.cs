using Microsoft.VisualStudio.TestTools.UnitTesting;
using MegaKnightChess.Domain;

namespace MegaKnightChess
{
    [TestClass]
    public class ChessTests
    {
        private const int Size = 8;

        public Board testBoard = new(Size);

        public static Figure whitePawn = new(Colour.White, FigureType.Pawn);
        public static Figure whiteKnight = new(Colour.White, FigureType.Knight);
        public static Figure whiteMegaknight = new(Colour.White, FigureType.Megaknight);
        public static Figure whiteKing = new(Colour.White, FigureType.King);
        public static Figure blackPawn = new(Colour.Black, FigureType.Pawn);
        public static Figure blackKnight = new(Colour.Black, FigureType.Knight);
        public static Figure blackMegaknight = new(Colour.Black, FigureType.Megaknight);
        public static Figure blackKing = new(Colour.Black, FigureType.King);

        public List<Figure> allFigureTypes = new()
        {
            whitePawn,
            whiteKnight,
            whiteMegaknight,
            whiteKing,
            blackPawn,
            blackKnight,
            blackMegaknight,
            blackKing
        };

        public void SetFigures(Dictionary<Figure, HashSet<Point>> figuresLocations)
        {
            foreach (var figure in allFigureTypes)
            {
                if (figuresLocations[figure] == null) continue;
                foreach (var location in figuresLocations[figure])
                {
                    testBoard.Cells[location.X, location.Y].Figure = figure;
                }
            }
        }

        [TestMethod]
        public void GameStatusCheckingShouldStalemate()
        {
            Dictionary<Figure, HashSet<Point>> figuresLocations = new()
            {
                { whiteKing, new HashSet<Point> {new Point() { X = 0, Y = 0 } } },
                { blackKing, new HashSet<Point> {new Point() { X = 6, Y = 6 } } },
                { blackKnight, new HashSet<Point> {new Point() { X = 2, Y = 2 }, new Point() { X = 2, Y = 3 } } },
            };
            SetFigures(figuresLocations);
            var expected = StatusType.Stalemate;
            var actual = testBoard.StatusChecking(Colour.White);
            Assert.AreEqual(expected, actual);
        }
    }
}