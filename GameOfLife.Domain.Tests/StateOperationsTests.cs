using GameOfLife.Domain.Core;

namespace GameOfLife.Domain.Tests
{
    [TestClass]
    public class StateOperationsTests
    {
        /// <summary>
        /// Test that GetNextState returns an empty matrix when no data to process
        /// </summary>
        [TestMethod]
        public void GetNextState_Returns_Empty_Matrix_When_No_Data()
        {
            // arrange
            int[,] matrix = null;

            // act
            var actual = StateOperations.GetNextState(matrix);

            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.GetLength(0));
            Assert.AreEqual(0, actual.GetLength(1));
        }

        /// <summary>
        /// Test that GetNextState returns next state board
        /// </summary>
        [TestMethod]
        public void GetNextState_Returns_Next_State_Board()
        {
            // arrange
            int[,] initialState = GetKnownInitialState();
            int[,] expected = GetKnownSecondState();

            // act
            var actual = StateOperations.GetNextState(initialState);

            // assert
            Assert.IsNotNull(actual);
            AssertStateContent(expected, actual);
        }

        /// <summary>
        /// Get known initial state to test.
        /// </summary>
        /// <returns></returns>
        private int[,] GetKnownInitialState()
        {
            return new int[,] {
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 }
            };
        }

        /// <summary>
        /// Get known second state from the initial one.
        /// </summary>
        /// <returns></returns>
        private int[,] GetKnownSecondState()
        {
            return new int[,] {
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 }
            };
        }

        /// <summary>
        /// Asserts each cell in the boards (expected and actual) are the same.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
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