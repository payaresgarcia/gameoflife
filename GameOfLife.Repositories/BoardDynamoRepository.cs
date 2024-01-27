using Amazon.DynamoDBv2.DataModel;
using GameOfLife.DAL.Models;
using GameOfLife.Domain.Interfaces;

namespace GameOfLife.Repositories
{
    public class BoardDynamoRepository : IBoardRepository
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        /// <summary>
        /// Creates a new instance of <see cref="BoardDynamoRepository"/>
        /// </summary>
        /// <param name="dynamoDbContext">DynamoDB context instance</param>
        /// <exception cref="ArgumentNullException">If there is no dynamoDbContext instance</exception>
        public BoardDynamoRepository(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext ?? throw new ArgumentNullException(nameof(dynamoDbContext));
        }

        /// <inheritdoc/>
        public async Task<Board> GetBoardByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _dynamoDbContext.LoadAsync<Board>(id, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SaveBoardAsync(Board board, CancellationToken cancellationToken)
        {
            if (board != null)
            {
                await _dynamoDbContext
                    .SaveAsync(board, new DynamoDBOperationConfig { IgnoreNullValues = false }, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
