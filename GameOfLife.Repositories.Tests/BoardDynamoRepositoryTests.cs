using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Moq;

namespace GameOfLife.Repositories.Tests
{
    [TestClass]
    public class BoardDynamoRepositoryTests
    {
        private readonly Mock<IDynamoDBContext> _mockDynamoContext;
        private readonly BoardDynamoRepository _sut;

        /// <summary>
        /// Initialize a new instance of <see cref="BoardDynamoRepositoryTests"/> class.
        /// </summary>
        public BoardDynamoRepositoryTests()
        {
            _mockDynamoContext = new Mock<IDynamoDBContext>();
            _sut = new BoardDynamoRepository(_mockDynamoContext.Object);
        }

        /// <summary>
        /// Test that GetBoardByIdAsync get a board record from dynamo.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetBoardByIdAsync_Throws_Exception_When_DynamoError()
        {
            // arrange
            _mockDynamoContext.Setup(e => e.LoadAsync(It.IsAny<string>(), CancellationToken.None));

            // act
            await _sut.GetBoardByIdAsync(String.Empty, CancellationToken.None);
        }

        /// <summary>
        /// Test that SaveBoardAsync throws an exception when there is an error in dynamo.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(AmazonDynamoDBException))]
        public async Task SaveBoardAsync_Throws_Exception_When_DynamoError()
        {
            // arrange
            DAL.Models.Board entry = new DAL.Models.Board();
            _mockDynamoContext.Setup(e => e.SaveAsync(It.IsAny<DAL.Models.Board>(), It.IsAny<DynamoDBOperationConfig>(), CancellationToken.None)).ThrowsAsync(new AmazonDynamoDBException("Error Message"));

            // act
            await _sut.SaveBoardAsync(entry, CancellationToken.None);
        }

        /// <summary>
        /// Test that SaveBoardAsync saves a board in dynamoDB.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveBoardAsync_Saves_Board_In_DynamoDB()
        {
            // arrange
            DAL.Models.Board entry = new DAL.Models.Board();
            _mockDynamoContext.Setup(e => e.SaveAsync(It.IsAny<DAL.Models.Board>(), It.IsAny<DynamoDBOperationConfig>(), CancellationToken.None));

            // act
            await _sut.SaveBoardAsync(entry, CancellationToken.None);
        }
    }
}