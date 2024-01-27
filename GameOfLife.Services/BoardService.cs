using GameOfLife.Domain.Core;
using GameOfLife.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GameOfLife.Services
{
    public class BoardService : IBoardService
    {
        private readonly ILogger<BoardService> _logger;
        private readonly IBoardRepository _boardRepository;

        /// <summary>
        /// Creates a new instance of <see cref="BoardService"/>
        /// </summary>
        /// <param name="logger">The logger service.</param>
        /// <param name="boardRepository">The board repository where data is stored.</param>
        /// <exception cref="ArgumentNullException">If there is no boardRepository instance</exception>
        public BoardService(ILogger<BoardService> logger, IBoardRepository boardRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _boardRepository = boardRepository ?? throw new ArgumentNullException(nameof(boardRepository));
        }

        /// <summary>
        /// Return a board based on the id.
        /// </summary>
        /// <param name="id">Guid id of the board.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <returns>A Board object mapped to the DynamoDB table.</returns>
        public virtual async Task<DAL.Models.Board> GetBoardByIdAsync(string id, CancellationToken cancellationToken)
        {
            var item = await _boardRepository.GetBoardByIdAsync(id, cancellationToken).ConfigureAwait(false);
            
            return item ?? new DAL.Models.Board();
        }

        /// <summary>
        /// Return the new created board.
        /// </summary>
        /// <param name="board">A Board object mapped to the DynamoDB table.</param>
        /// <param name="cancellationToken">Request's cancellation token.</param>
        /// <returns>A Board object.</returns>
        public async Task<DAL.Models.Board> CreateBoardAsync(DAL.Models.Board board, CancellationToken cancellationToken)
        {
            try
            {
                await _boardRepository.SaveBoardAsync(board, cancellationToken).ConfigureAwait(false);
                return board;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, board);

                return new DAL.Models.Board();
            }
        }

        /// <summary>
        /// Get the next state from a board.
        /// </summary>
        /// <param name="id">Id of the board.</param>
        /// <param name="cancellationToken">Request's cancellation token.</param>
        /// <returns>A Board object</returns>
        public async Task<DAL.Models.Board> GetBoardNextStateAsync(string id, CancellationToken cancellationToken)
        {
            var board = await GetBoardByIdAsync(id, cancellationToken);
            if (!String.IsNullOrWhiteSpace(board?.CurrentState))
            {
                try
                {
                    var state = JsonConvert.DeserializeObject<int[,]>(board.CurrentState);
                    var nextStateResult = StateOperations.GetNextState(state!);

                    board.PreviousState = board.CurrentState;
                    board.CurrentState = JsonConvert.SerializeObject(nextStateResult);

                    await _boardRepository.SaveBoardAsync(board, cancellationToken).ConfigureAwait(false);
                    return board;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, board);
                }
            }

            return new DAL.Models.Board();
        }
    }
}
