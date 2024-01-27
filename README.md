# Game of Life Project

The Game of Life is a cellular automaton devised by the British mathematician John Horton Conway in 1970.
It is a zero-player game, meaning that its evolution is determined by its initial state, requiring no further input.

## Description

The universe of the Game of Life is an infinite, two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, live or dead (or populated and unpopulated, respectively). Every cell interacts with its eight neighbors, which are the cells that are horizontally, vertically, or diagonally adjacent. At each step in time, the following transitions occur:

### Rules:
* Any live cell with fewer than two live neighbors dies, as if by underpopulation.
* Any live cell with two or three live neighbors lives on to the next generation.
* Any live cell with more than three live neighbors dies, as if by overpopulation.
* Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.

The initial pattern constitutes the seed of the system. The first generation is created by applying the above rules simultaneously to every cell in the seed, live or dead; births and deaths occur simultaneously, and the discrete moment at which this happens is sometimes called a tick. Each generation is a pure function of the preceding one. The rules continue to be applied repeatedly to create further generations


## Dependencies

* Net 7
* ASP NetCore 7
* AWSDynamoDB


### Executing program

* Create an AWS DynamoDB database [name: game-of-life]
* In root project folder build the source code
```
dotnet build
```

* Enter to GameOfLife.Api folder
```
cd GameOfLife.Api
```

* Run the application from command line
```
dotnet run
```

* Check the url prompted in console
* Paste the url in your browser
[url]/swagger/index.html


## Authors

Pedro Payares
[payares7@gmail.com](payares7@gmail.com)

## Version History


* 1.0.0
    * Initial Release

## License

This project is licensed under the Academic Free License v3.0 - see the LICENSE.md file for details

## Acknowledgments

Links:
[Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)