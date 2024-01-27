using Amazon.DynamoDBv2.DataModel;

namespace GameOfLife.DAL.Models
{
    /// <summary>
    /// DynamoDB table to persist board and states.
    /// The state is a 2D array stored as a dash separated string.
    /// The "000-010-000" represents a 3X3 2D array.
    /// </summary>
    [DynamoDBTable("game-of-life", lowerCamelCaseProperties: true)]
    public class Board
    {
        /// <summary>
        /// Guid string used as hash key.
        /// </summary>
        [DynamoDBHashKey]
        public string Id { get; set; }

        /// <summary>
        /// Represents the state associated to the board prior the last execution.
        /// </summary>
        [DynamoDBProperty]
        public string PreviousState { get; set; } = String.Empty;

        /// <summary>
        /// Represents the state associated to the board after the last execution.
        /// </summary>
        [DynamoDBProperty]
        public string CurrentState { get; set; } = String.Empty;
    }
}
