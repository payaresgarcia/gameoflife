using AutoMapper;
using GameOfLife.Controllers;
using GameOfLife.Domain.Interfaces;
using GameOfLife.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace GameOfLife.Api.Tests
{
    [TestClass]
    public class BoardControllerTests
    {
        //Mock instances for services.
        private readonly IMapper _mockMapper;
        private readonly Mock<IBoardService> _mockBoardService;

        // System under test.
        private readonly BoardController _sut;

        public BoardControllerTests()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<Mappers.MappingProfile>();
            });
            _mockMapper = mappingConfig.CreateMapper();
            _mockBoardService = new Mock<IBoardService>();
            _sut = new BoardController(_mockMapper, _mockBoardService.Object);
        }

        /// <summary>
        /// Test that CreateBoard returns a Bad Request response when no input.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateBoard_Returns_BadRequest_When_NoInput()
        {
            // arrange
            BoardDto newBoardRequest = null;

            // act
            var response = await _sut.CreateBoard(newBoardRequest, CancellationToken.None);

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(BadRequestResult));
        }

        /// <summary>
        /// Test that CreateBoard returns a No Content response when no board created.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateBoard_Returns_NoContent_When_NoBoardCreatedt()
        {
            // arrange
            var newBoardRequest = BuildBoardDto();
            _mockBoardService.Setup(e => e.CreateBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None)).ReturnsAsync(new DAL.Models.Board());

            // act
            var response = await _sut.CreateBoard(newBoardRequest, CancellationToken.None);

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
        }

        /// <summary>
        /// Test that CreateBoard returns an Ok response when successfull.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateBoard_Returns_Ok_When_Successfull()
        {
            // arrange
            var newBoardRequest = BuildBoardDto();
            var dalBoardResponse = BuildDALBoard();
            _mockBoardService.Setup(e => e.CreateBoardAsync(It.IsAny<DAL.Models.Board>(), CancellationToken.None)).ReturnsAsync(dalBoardResponse);

            // act
            var response = await _sut.CreateBoard(newBoardRequest, CancellationToken.None);
            var result = response as OkObjectResult;
            var data = result.Value as string;

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.AreEqual(dalBoardResponse.Id, data);
        }

        /// <summary>
        /// Test that GetNextState returns a No Found response when no board state.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetNextState_Returns_NoFound_When_NoBoardState()
        {
            // arrange
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(new DAL.Models.Board());

            // act
            var response = await _sut.GetNextState(String.Empty, CancellationToken.None);

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        /// <summary>
        /// Test that GetNextState returns a Next State response when successfull.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetNextState_Returns_NextState_When_Successfull()
        {
            // arrange
            var dalBoardResponse = BuildDALBoard();
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(dalBoardResponse);

            // act
            var response = await _sut.GetNextState(String.Empty, CancellationToken.None);
            var result = response as OkObjectResult;
            var data = result.Value as int[,];

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));

            var expected = JsonConvert.DeserializeObject<int[,]>(dalBoardResponse.CurrentState);
            AssertStateContent(expected, data);
        }

        /// <summary>
        /// Test that GetStates returns a Empty Collection response when no board state.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStates_Returns_EmptyCollection_When_NoBoardState()
        {
            // arrange
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(new DAL.Models.Board());

            // act
            var response = await _sut.GetStates(0, String.Empty, CancellationToken.None);
            var result = response as OkObjectResult;
            var data = result.Value as List<int[,]>;

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.IsTrue(data.Count == 0);
        }

        /// <summary>
        /// Test that GetStates returns a States Collection response when successful.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetStates_Returns_StatesCollectionn_When_Successful()
        {
            // arrange
            var dalBoardResponse = BuildDALBoard();
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(dalBoardResponse);

            // act
            var response = await _sut.GetStates(2, String.Empty, CancellationToken.None);
            var result = response as OkObjectResult;
            var data = result.Value as List<int[,]>;

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.IsTrue(data.Count == 2);

            var expected = JsonConvert.DeserializeObject<int[,]>(dalBoardResponse.CurrentState);
            AssertStateContent(expected, data[0]);
            AssertStateContent(expected, data[1]);
        }

        /// <summary>
        /// Test that GetFinalState returns an Error when the numbers of iterations is too large.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetFinalState_Returns_Error_When_TooLongStates()
        {
            // arrange
            var dalBoardResponse = BuildDALBoard(isPreviousSameThanCurrent: false);
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(dalBoardResponse);

            // act
            var response = await _sut.GetFinalState(String.Empty, CancellationToken.None);
            var result = response as StatusCodeResult;

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(StatusCodeResult));
            Assert.AreEqual(new StatusCodeResult(413).StatusCode, result.StatusCode);
        }

        /// <summary>
        /// Test that GetFinalState returns Not Found when no board state.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetFinalState_Returns_NotFound_When_No_BordState_Exists()
        {
            // arrange
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(new DAL.Models.Board());

            // act
            var response = await _sut.GetFinalState(String.Empty, CancellationToken.None);

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        /// <summary>
        /// Test that GetFinalState returns an state when final state exists.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetFinalState_Returns_State_When_FinalStateExists()
        {
            // arrange
            var dalBoardResponse = BuildDALBoard(isPreviousSameThanCurrent: true);
            _mockBoardService.Setup(e => e.GetBoardNextStateAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(dalBoardResponse);

            // act
            var response = await _sut.GetFinalState(String.Empty, CancellationToken.None);
            var result = response as OkObjectResult;
            var data = result.Value as int[,];

            // assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));

            var expected = JsonConvert.DeserializeObject<int[,]>(dalBoardResponse.CurrentState);
            AssertStateContent(expected, data);
        }

        private BoardDto BuildBoardDto()
        {
            return new BoardDto {
                State = new int[,] {
                    { 1, 0, 1 },
                    { 1, 1, 0 },
                    { 0, 1, 1 }
                }
            };
        }

        private DAL.Models.Board BuildDALBoard(bool isPreviousSameThanCurrent = false)
        {
            string previousState = "[[1,1],[1,1]]";
            string currentState = "[[1,0],[0,0]]";

            return new DAL.Models.Board
            {
                Id = "Test Id",
                CurrentState = currentState,
                PreviousState = isPreviousSameThanCurrent ? currentState : previousState
            };
        }

        private void AssertStateContent(int[,] expected, int[,] actual)
        {
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.AreEqual(expected[i, j], actual[i, j]);
                }
            }
        }
    }
}