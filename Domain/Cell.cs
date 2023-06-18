namespace MegaKnightChess.Domain
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            BlockedForMove = false; //на эту клетку выбранная на данный момент фигура не может ходить согласно правилам,
                                    //еще используется при нахождении незаблокированных клеток для хода короля выбранного цвета
            LegalNextMove = false; //клетка доступная для хода для данной фигуры и не заблокирована правилами
        }
        public int X { get; set; }
        public int Y { get; set; }
        public bool BlockedForMove { get; set; }
        public bool LegalNextMove { get; set; }
        public Figure? Figure { get; set; }

        //сахарные методы
        public FigureType? CheckFigureType()
        {
            if (Figure != null) return Figure.Type;
            else return null;
        }

        public Colour? CheckFigureColour()
        {
            if (Figure != null) return Figure.Colour;
            else return null;
        }

        public bool HasFigure()
        {
            return Figure == null ? false : true;
        }
    }
}
