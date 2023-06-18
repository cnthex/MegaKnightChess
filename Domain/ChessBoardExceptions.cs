namespace MegaKnightChess.Domain
{
    public class ImpossibleBoardException : Exception
    {
        public ImpossibleBoardException() : base() { }
    }
    public class OutOfBoardException : Exception
    {
        public OutOfBoardException() : base() { }
    }
}
