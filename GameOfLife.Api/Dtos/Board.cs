namespace GameOfLife.Api.Dtos
{
    public class Board
    {
        public string Id { get; set; }

        public int[,] State { get; set; }
    }
}
