namespace MegaKnightChess.Domain
{
    public class Figure
    {
        public Figure(Colour colour, FigureType type)
        {
            Colour = colour;
            Type = type;
        }
        public Colour Colour { get; set; }
        public FigureType Type { get; set; }
    }
}
