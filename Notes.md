### Some loose thoughts

Let's assume, that simulation can take some time, as simulation often do.
Battleship simulation is rather easy, but we may extend app with other simulations
With that assumption is mind we need to separate 'create simulation' logic from 'get simulation result' logic.
So I can create endpoints like these:

| method | url                                    | description                                    |
|--------|----------------------------------------|------------------------------------------------|
| POST   | /simulations/battleship/new            | starts new simulation and return simulation id |
| GET    | /simulations/battleship/get?id={$guid} | get information about given simulation         |

### Thoughts about simulation

Model elements:

* players
* boards
* rounds
  * guess and hit/miss info for both players

```csharp
public class Player
{
    private Board _ownBoard;
    private Board _guessingBoard;
    
    // make a guess based on available information (_guessingBoard) 
    public Point Guess() { ... }
    
    public Answer Answer(Point p) { ... }
}

public enum Answer 
{
    Hit = 0,
    Miss = 1
}

// OwnBoard:
// 0 -> ocean
// 1 -> ship
// GuessingBoard:
// -1-> unknown
// 0 -> (miss)ocean
// 1 -> (hit)ship
public class Board
{
    private sbyte[][] _board;
}
```

Pre simulation stages:

* randomly place ships for both players

Simulation stages

1. player 1 makes a guess, player 2 answers
2. if remaining ships of player 2 == 0 player 1 won
3. player 2 makes a guess, player 1 answers
4. if remaining ships of player 1 == 0 player 2 won
5. repeat till game is over
