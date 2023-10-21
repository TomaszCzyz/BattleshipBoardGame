using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Services;

[UsedImplicitly]
public class BattleshipGameSimulator : IBattleshipGameSimulator
{
    private readonly ILogger<BattleshipGameSimulator> _logger;
    private readonly ISimulationsDbContext _dbContext;
    private readonly IBoardGenerator _boardGenerator;
    private readonly IGuessingEngine _guessingEngine;

    public BattleshipGameSimulator(
        ILogger<BattleshipGameSimulator> logger,
        ISimulationsDbContext dbContext,
        IBoardGenerator boardGenerator,
        IGuessingEngine guessingEngine)
    {
        _logger = logger;
        _dbContext = dbContext;
        _boardGenerator = boardGenerator;
        _guessingEngine = guessingEngine;
    }

    /// <summary>
    ///     Creates and runs new Battleship game simulation.
    ///     After finishing the database is updated with results.
    /// </summary>
    /// <remarks>
    ///     Currently there is no cancellation support, which can make app susceptible for DoS attack
    ///     (spamming new simulations). It can be solve by rate limiting the 'create new simulation' endpoint,
    ///     by cancelling long running simulations or by allowing only given number of simulations running
    ///     at the same time.
    /// </remarks>
    public async Task Create(Guid id)
    {
        _logger.LogInformation("Starting new simulation with id {SimulationId}", id);

        var player1 = new Player
        {
            Name = "Player 1",
            Board = _boardGenerator.Generate(),
            Guesses = new List<(uint, uint)>(),
            GuessingStrategy = GuessingStrategy.Random
        };
        var player2 = new Player
        {
            Name = "Player 2",
            Board = _boardGenerator.Generate(),
            Guesses = new List<(uint, uint)>(),
            GuessingStrategy = GuessingStrategy.Random
        };

        while (true)
        {
            if (PlayPlayerTurn(player1, player2))
            {
                break;
            }

            if (PlayPlayerTurn(player2, player1))
            {
                break;
            }
        }

        var simulation = await _dbContext.Simulations.FindAsync(id);
        if (simulation is not null)
        {
            simulation.IsFinished = true;
            simulation.Player1 = player1;
            simulation.Player2 = player2;
        }
        else
        {
            await _dbContext.Simulations.AddAsync(
                new Simulation
                {
                    Id = id,
                    IsFinished = true,
                    Player1 = player1,
                    Player2 = player2
                });
        }

        _dbContext.SaveChanges();
    }

    private bool PlayPlayerTurn(Player guessingPlayer, Player answeringPlayer)
    {
        var guess = _guessingEngine.Guess(guessingPlayer);
        if (answeringPlayer.Answer(guess))
        {
            guessingPlayer.GuessingBoard[guess.X, guess.Y] = 1;
            _logger.LogInformation("{PlayerName} guessed: {Point} and it was a hit", guessingPlayer.Name, guess);
            answeringPlayer.HitCounter++;

            // todo: move this logic to Player as it is player's responsibility to know when all ships have sunk
            if (answeringPlayer.HitCounter == 18)
            {
                _logger.LogInformation("{PlayerName} lost!", guessingPlayer.Name);
                return true;
            }
        }
        else
        {
            guessingPlayer.GuessingBoard[guess.X, guess.Y] = 0;
            _logger.LogInformation("{PlayerName} guessed: {Point} and it was a miss", guessingPlayer.Name, guess);
        }

        return false;
    }
}
