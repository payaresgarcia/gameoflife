namespace GameOfLife.Domain.Core
{
    public class StateOperations
    {
        public static int[,] GetNextState(int[,] board)
        {
            Mutex nextStateMutex = new Mutex(false);

            if (board == null)
                return new int[0, 0];

            int board1DLength = board.GetLength(0);
            int board2DLength = board.GetLength(1);

            int[,] newState = new int[board1DLength, board2DLength];

            Parallel.For(0, board1DLength, i => {
                Parallel.For(0, board2DLength, j => {
                    int aliveNeighbours = 0;
                    int currentCellNextState = 0;

                    // Left upper adjacent neighbour
                    aliveNeighbours += GetLeftUpperAdjacentNeighbour(board, i, j);

                    // Upper neighbour
                    aliveNeighbours += GetUpperNeighbour(board, i, j);

                    // Right upper adjacent neighbour
                    aliveNeighbours += GetRightUpperAdjacentNeighbour(board, i, j, board2DLength);

                    // Left neighbour
                    aliveNeighbours += GetLeftNeighbour(board, i, j);

                    // Right neighbour
                    aliveNeighbours += GetRightNeighbour(board, i, j, board2DLength);

                    // Left bottom adjacent neighbour
                    aliveNeighbours += GetLeftBottomAdjacentNeighbour(board, i, j, board1DLength);

                    // Bottom neighbour
                    aliveNeighbours += GetBottomNeighbour(board, i, j, board1DLength);

                    // Right bottom adjacent neighbour
                    aliveNeighbours += GetRightBottomAdjacentNeighbour(board, i, j, board1DLength, board2DLength);

                    // Determine state for current cell
                    // If cell is alive
                    if (board[i, j] == 1)
                    {
                        if (aliveNeighbours == 2 || aliveNeighbours == 3)
                            currentCellNextState = 1;
                    }
                    else
                    {
                        if (aliveNeighbours == 3)
                            currentCellNextState = 1;
                    }

                    // Set next state
                    nextStateMutex.WaitOne();
                    newState[i, j] = currentCellNextState;
                    nextStateMutex.ReleaseMutex();
                });
            });

            return newState;
        }

        private static int GetLeftUpperAdjacentNeighbour(int[,] board, int i, int j)
        {
            return ((i - 1) >= 0 && (j - 1) >= 0) ? board[i - 1, j - 1] : 0;
        }

        private static int GetUpperNeighbour(int[,] board, int i, int j)
        {
            return (i - 1) >= 0 ? board[i - 1, j] : 0;
        }

        private static int GetRightUpperAdjacentNeighbour(int[,] board, int i, int j, int board2DLength)
        {
            return ((i - 1) >= 0 && (j + 1) < board2DLength) ? board[i - 1, j + 1] : 0;
        }

        private static int GetLeftNeighbour(int[,] board, int i, int j)
        {
            return (j - 1) >= 0 ? board[i, j - 1] : 0;
        }

        private static int GetRightNeighbour(int[,] board, int i, int j, int board2DLength)
        {
            return (j + 1) < board2DLength ? board[i, j + 1] : 0;
        }

        private static int GetLeftBottomAdjacentNeighbour(int[,] board, int i, int j, int board1DLength)
        {
            return ((i + 1) < board1DLength && (j - 1) >= 0) ? board[i + 1, j - 1] : 0;
        }

        private static int GetBottomNeighbour(int[,] board, int i, int j, int board1DLength)
        {
            return (i + 1) < board1DLength ? board[i + 1, j] : 0;
        }

        private static int GetRightBottomAdjacentNeighbour(int[,] board, int i, int j, int board1DLength, int board2DLength)
        {
            return ((i + 1) < board1DLength && (j + 1) < board2DLength) ? board[i + 1, j + 1] : 0;
        }
    }
}
