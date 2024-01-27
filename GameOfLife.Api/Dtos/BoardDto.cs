using System.ComponentModel.DataAnnotations;

namespace GameOfLife.Dtos
{
    public record BoardDto
    {
        [Required]
        public int[,] State { get; init; }
    }
}
