using System.Diagnostics;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Extensions;
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

        // todo: get player's settings from POST's body
        var playerInfo1 = new PlayerInfo
        {
            Name = "Player 1",
            GuessingStrategy = GuessingStrategy.Random,
            ShipsPlacementStrategy = ShipsPlacementStrategy.Simple
        };
        var playerInfo2 = new PlayerInfo
        {
            Name = "Player 2",
            GuessingStrategy = GuessingStrategy.Random,
            ShipsPlacementStrategy = ShipsPlacementStrategy.Simple
        };

        var (player1, player2) = CreatePlayers(playerInfo1, playerInfo2);

        RunSimulation(player1, player2);

        var playerDto1 = new PlayerDto
        {
            Id = id,
            PlayerInfo = playerInfo1,
            Ships = player1.Ships.ToList(),
            Guesses = player1.Guesses.ToList()
        };

        var playerDto2 = new PlayerDto
        {
            Id = id,
            PlayerInfo = playerInfo2,
            Ships = player2.Ships.ToList(),
            Guesses = player2.Guesses.ToList()
        };

        await UpdateResults(id, playerDto1, playerDto2);
    }

    private (Player Player1, Player Player2) CreatePlayers(PlayerInfo playerInfo1, PlayerInfo playerInfo2)
    {
        var generateShips1 = _boardGenerator.GenerateShips(playerInfo1.ShipsPlacementStrategy);
        var generateShips2 = _boardGenerator.GenerateShips(playerInfo2.ShipsPlacementStrategy);

        return (new Player(generateShips1), new Player(generateShips2));
    }

    private void RunSimulation(Player player1, Player player2)
    {
        BattleAnswer answerOfPlayer1, answerOfPlayer2;
        var counter = 0;
        do
        {
            counter++;
            answerOfPlayer2 = PlayTurn(player1, player2);
            answerOfPlayer1 = PlayTurn(player2, player1);
        } while (answerOfPlayer1 != BattleAnswer.HitAndWholeFleetSunk && answerOfPlayer2 != BattleAnswer.HitAndWholeFleetSunk);

        var message = (answerOfPlayer1, answerOfPlayer2) switch
        {
            (BattleAnswer.HitAndWholeFleetSunk, BattleAnswer.HitAndWholeFleetSunk) => "It is a draw!!!",
            (BattleAnswer.HitAndWholeFleetSunk, _) => "Player 2 wins!",
            (_, BattleAnswer.HitAndWholeFleetSunk) => "Player 1 wins!",
            _ => throw new UnreachableException()
        };

        _logger.LogInformation("Simulation ended in {RoundsNumber} with result: {Message}", counter, message);
    }

    private BattleAnswer PlayTurn(Player guessingPlayer, Player answeringPlayer)
    {
        var guess = _guessingEngine.Guess(guessingPlayer);
        _logger.LogInformation("Guessing: {Coords}", guess);

        var answer = answeringPlayer.Answer(guess, out _);
        _logger.LogInformation("Answer: {Answer}", answer);

        guessingPlayer.ApplyAnswerInfo(guess, answer);

        return answer;
    }

    private async Task UpdateResults(Guid id, PlayerDto player1, PlayerDto player2)
    {
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
}
