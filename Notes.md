### Some loose thoughts

Let's assume, that simulation can take some time, as simulation often do.
Battleship simulation is rather easy, but we may extend app with other simulations
With that assumption is mind we need to separate 'create simulation' logic from 'get simulation result' logic.
So I can create endpoints like these:

| method | url                          | description                                    |
|--------|------------------------------|------------------------------------------------|
| POST   | /simulations/battleship/new  | starts new simulation and return simulation id |
| GET    | /simulations/battleship/{id} | get information about given simulation         |

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

### Simulation schema
When I was modeling schema for simulation dto a question arose:
if `Board` should be stored as a part of a simulation model, or should it be contained in Player model.

Option 1:
```csharp
Simulation 
{
    Board BoardOfPlayer1 { get; set; };
    Board BoardOfPlayer2 { get; set; };
    Guess[] Guesses { get; set; }
}
```

Option 2:
```csharp
Simulation 
{
    Player Player1 { get; set; }
    Player Player1 { get; set; }
}

Player
{
    Board { get; set; }
    Guess[] Guesses { get; set; } // different than guesses from Option 1 (contains only guesses of the player)
}

```

Note: Guesses can be stored as string of comma-separated values. It will reduce model complexity and number of joins
and improve data readability.

I like the Option 2 better. It seems more natural.

### Player model
Some properties of a Player class are useful only during simulation, but there is no point is storing them.
For example:
* hit counter
* guessing board

I am considering splitting Player class into a Player and a PlayerDto classes. One would contain a logic
and a second would be just for storing simulation result in db.
