namespace GameOfLife.Domain.Interfaces
{
    public interface IBoardService
    {
        /// <summary>
        /// Return a board based on the id.
        /// </summary>
        /// <param name="id">Guid id of the board.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <returns>A Board object mapped to the DynamoDB table.</returns>
        Task<DAL.Models.Board> GetBoardByIdAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// Return the new created board.
        /// </summary>
        /// <param name="board">A Board object mapped to the DynamoDB table.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <returns>An object of the created board.</returns>
        Task<DAL.Models.Board> CreateBoardAsync(DAL.Models.Board board, CancellationToken cancellationToken);

        /// <summary>
        /// Get the next state from a board.
        /// </summary>
        /// <param name="id">Id of the board.</param>
        /// <param name="cancellationToken">Request's cancellation token.</param>
        /// <returns>A Board object</returns>
        Task<DAL.Models.Board> GetBoardNextStateAsync(string id, CancellationToken cancellationToken);
    }
}
