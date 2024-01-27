using GameOfLife.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Services.Tests
{
    [TestClass]
    public class BoardServiceTests
    {
        private readonly Mock<IBoardRepository> _mockBoardRepository;
        private readonly Mock<ILogger<BoardService>> _mockLogger;
        private readonly BoardService _sut;

        /// <summary>
        /// Initialize a new instance of <see cref="BoardServiceTests"/> class.
        /// </summary>
        public BoardServiceTests()
        {
            _mockBoardRepository = new Mock<IBoardRepository>();
            _mockLogger = new Mock<ILogger<BoardService>>();
            _sut = new BoardService(_mockLogger.Object, _mockBoardRepository.Object);
        }

        /// <summary>
        /// Test that CreateBoardAsync returns saved board when successful.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateBoardAsync_Returns_Saved_Board_When_Successful()
        {
            // arrange
            var boardRequest = BuildDALBoard();
            _mockBoardRepository.Setup(e => e.SaveBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None));

            // act
            var actual = await _sut.CreateBoardAsync(boardRequest, CancellationToken.None);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(boardRequest.Id, actual.Id);
        }

        /// <summary>
        /// Test that CreateBoardAsync returns empty board when an exception occurs.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateBoardAsync_Returns_Empty_Board_When_Exception()
        {
            // arrange
            var boardRequest = BuildDALBoard();
            _mockBoardRepository.Setup(e => e.SaveBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None)).ThrowsAsync(new Exception("Error Message"));

            // act
            var actual = await _sut.CreateBoardAsync(boardRequest, CancellationToken.None);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(String.IsNullOrWhiteSpace(actual.Id));
        }

        /// <summary>
        /// Test that GetBoardNextStateAsync returns an empty state board when no board to process.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetBoardNextStateAsync_Returns_Empty_Board_When_NoBoard()
        {
            // arrange
            _mockBoardRepository.Setup(e => e.SaveBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None));

            // act
            var actual = await _sut.GetBoardNextStateAsync("1", CancellationToken.None);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(String.IsNullOrWhiteSpace(actual.Id));
        }

        /// <summary>
        /// Test that GetBoardNextStateAsync returns next state board when successful.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetBoardNextStateAsync_Returns_NextState_Board_When_Successful()
        {
            // arrange
            var boardResponse = BuildDALBoard();
            _mockBoardRepository.Setup(e => e.SaveBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None));
            var mockBoardService = new BoardServiceMock(boardResponse, _mockBoardRepository);

            // act
            var actual = await mockBoardService.GetBoardNextStateAsync("1", CancellationToken.None);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(boardResponse.Id, actual.Id);
        }

        /// <summary>
        /// Test that GetBoardNextStateAsync returns an empty state board when any exception is thrown.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetBoardNextStateAsync_Returns_Empty_Board_When_Exception()
        {
            // arrange
            var boardResponse = BuildDALBoard();
            _mockBoardRepository.Setup(e => e.SaveBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None)).ThrowsAsync(new Exception("Error Message"));
            var mockBoardService = new BoardServiceMock(boardResponse, _mockBoardRepository);

            // act
            var actual = await mockBoardService.GetBoardNextStateAsync("1", CancellationToken.None);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(String.IsNullOrWhiteSpace(actual.Id));
        }

        private DAL.Models.Board BuildDALBoard()
        {
            string previousState = "[[1,1],[1,1]]";
            string currentState = "[[1,0],[0,0]]";

            return new DAL.Models.Board
            {
                Id = "Test Id",
                CurrentState = currentState,
                PreviousState = previousState
            };
        }

        /// <summary>
        /// BoardService mock for testing.
        /// </summary>
        public class BoardServiceMock : BoardService
        {
            private readonly DAL.Models.Board _sampleBoard;

            /// <summary>
            /// Initialize a new instance <see cref="BoardServiceMock"/> of class.
            /// </summary>
            public BoardServiceMock(DAL.Models.Board sampleBoard, Mock<IBoardRepository> mockBoardRepository) : base(new Mock<ILogger<BoardService>>().Object, mockBoardRepository.Object)
            {
                _sampleBoard = sampleBoard;
            }

            /// <summary>
            /// Override GetBoardByIdAsync method behavior for having a board response based on needs.
            /// </summary>
            /// <param name="id"></param>
            /// <param name="cancellationToken"></param>
            /// <returns>A Board object</returns>
            public override async Task<DAL.Models.Board> GetBoardByIdAsync(string id, CancellationToken cancellationToken)
            {
                return await Task.FromResult(_sampleBoard ?? new DAL.Models.Board());
            }
        }
    }
}