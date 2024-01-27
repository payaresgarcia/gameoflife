using AutoMapper;
using GameOfLife.Api.Dtos;
using GameOfLife.Domain.Constants;
using GameOfLife.Domain.Interfaces;
using GameOfLife.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameOfLife.Controllers
{
    /// <summary>
    /// Endpoint for Boards management.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/boards")]
    [ApiVersion("1.0")]
    public class BoardController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBoardService _boardService;

        /// <summary>
        /// Creates a new instance of <see cref="BoardController"/>
        /// </summary>
        /// <param name="mapper">Mapper service to handle models convertion and assigment.</param>
        /// <param name="boardService">Board service to perform CRUD operations.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BoardController(IMapper mapper, IBoardService boardService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
        }

        /// <summary>
        /// Return a Board with its last state.
        /// </summary>
        /// <param name="id">The id of the board.</param>
        /// <param name="cancellationToken">The request cancelation token if any.</param>
        /// <returns>A Board object</returns>
        [ProducesResponseType(typeof(Board), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetBoardById")]
        public async Task<IActionResult> GetBoardById(string id, CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();

            var response = await _boardService.GetBoardByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (String.IsNullOrWhiteSpace(response?.Id))
                return NotFound();

            return Ok(_mapper.Map<Board>(response));
        }

        /// <summary>
        /// Return the id of the new created board.
        /// </summary>
        /// <param name="newBoard">The new board to be persisted.</param>
        /// <param name="cancellationToken">The request's cancelation token if any.</param>
        /// <returns>The id string of the new board.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost()]
        public async Task<IActionResult> CreateBoard([FromBody] BoardDto newBoard, CancellationToken cancellationToken)
        {
            if (newBoard == null || !ModelState.IsValid)
                return BadRequest();

            var board = _mapper.Map<DAL.Models.Board>(newBoard);
            var response = await _boardService.CreateBoardAsync(board, cancellationToken).ConfigureAwait(false);
            
            if (String.IsNullOrWhiteSpace(response?.Id))
                return NoContent();

            return Ok(response.Id);
        }

        /// <summary>
        /// Return the next state of a board.
        /// </summary>
        /// <param name="id">Id of the board</param>
        /// <param name="cancellationToken">The request cancelation token if any.</param>
        /// <returns>The next state from a board</returns>
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/next-state")]
        public async Task<IActionResult> GetNextState(string id, CancellationToken cancellationToken)
        {
            var response = await _boardService.GetBoardNextStateAsync(id, cancellationToken).ConfigureAwait(false);
            if (String.IsNullOrWhiteSpace(response?.Id))
                return NotFound();

            var board = _mapper.Map<Board>(response);
            return Ok(board.State);
        }

        /// <summary>
        /// Return the desired number of states indicated in the request.
        /// </summary>
        /// <param name="numberOfStates">Number of states to calculate and return</param>
        /// <param name="id">Id of the board</param>
        /// <param name="cancellationToken">The request's cancelation token if any.</param>
        /// <returns>An array of states</returns>
        [ProducesResponseType(typeof(string[][]), StatusCodes.Status200OK)]
        [HttpGet("{id}/states")]
        public async Task<IActionResult> GetStates([FromQuery(Name = "numberOfStates")] int numberOfStates, string id, CancellationToken cancellationToken)
        {
            if (numberOfStates > BoardConstants.MAX_STATES_REQUEST_LIMIT)
            {
                numberOfStates = BoardConstants.MAX_STATES_REQUEST_LIMIT;
            }

            var states = new List<int[,]>();

            for (int i = 0; i < numberOfStates; i++)
            {
                var response = await _boardService.GetBoardNextStateAsync(id, cancellationToken).ConfigureAwait(false);
                if (!String.IsNullOrWhiteSpace(response?.CurrentState))
                {
                    states.Add(JsonConvert.DeserializeObject<int[,]>(response.CurrentState)!);

                    if (response.CurrentState.Equals(response.PreviousState))
                    {
                        break;
                    }
                }
            }

            return Ok(states);
        }

        /// <summary>
        /// Return the final state fo a board.
        /// </summary>
        /// <param name="id">Id of the board</param>
        /// <param name="cancellationToken">The request's cancelation token if any</param>
        /// <returns>The final state from a board</returns>
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [HttpGet("{id}/final-state")]
        public async Task<IActionResult> GetFinalState(string id, CancellationToken cancellationToken)
        {
            int[,] state;

            int i = 0;
            while (true)
            {
                if (i == BoardConstants.MAX_STATES_REQUEST_LIMIT)
                {
                    return new StatusCodeResult(413);
                }

                var response = await _boardService.GetBoardNextStateAsync(id, cancellationToken).ConfigureAwait(false);
                if (!String.IsNullOrWhiteSpace(response?.CurrentState))
                {
                    state = JsonConvert.DeserializeObject<int[,]>(response.CurrentState)!;

                    if (response.CurrentState.Equals(response.PreviousState))
                        break;
                }
                else
                {
                    return NotFound();
                }

                i++;
            }

            return Ok(state);
        }
    }
}
