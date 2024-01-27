using GameOfLife.DAL.Models;

namespace GameOfLife.Domain.Interfaces
{
    public interface IBoardRepository
    {
        /// <summary>
        /// Return a board based on the id.
        /// </summary>
        /// <param name="id">Id of the board.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <returns>A Board object mapped to the DynamoDB table.</returns>
        Task<Board> GetBoardByIdAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Save a new board.
        /// This can be used to create and update an item.
        /// </summary>
        /// <param name="board">New board data</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <returns>The task object.</returns>
        Task SaveBoardAsync(Board board, CancellationToken cancellationToken);
    }
}
