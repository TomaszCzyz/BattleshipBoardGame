using System.Diagnostics;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models;
using BattleshipBoardGame.Models.Entities;
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
    public async Task Run(Simulation simulation)
    {
        _logger.LogInformation("Starting new simulation with id {SimulationId}", simulation.Id);

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

        var winner = await Task.Run(() => RunSimulation(player1, player2));

        var playerDto1 = new PlayerDto
        {
            Id = Guid.NewGuid(),
            PlayerInfo = playerInfo1,
            Ships = player1.Ships.ToList(),
            Guesses = player1.Guesses.ToList()
        };

        var playerDto2 = new PlayerDto
        {
            Id = Guid.NewGuid(),
            PlayerInfo = playerInfo2,
            Ships = player2.Ships.ToList(),
            Guesses = player2.Guesses.ToList()
        };

        simulation.IsFinished = true;
        simulation.Player1 = playerDto1;
        simulation.Player2 = playerDto2;
        simulation.Winner = player1 == winner ? playerDto1 : playerDto2;

        _dbContext.Simulations.Add(simulation);
        _dbContext.SaveChanges();
    }

    private (Player Player1, Player Player2) CreatePlayers(PlayerInfo playerInfo1, PlayerInfo playerInfo2)
    {
        var generateShips1 = _boardGenerator.GenerateShips(playerInfo1.ShipsPlacementStrategy);
        var generateShips2 = _boardGenerator.GenerateShips(playerInfo2.ShipsPlacementStrategy);

        return (new Player(generateShips1), new Player(generateShips2));
    }

    private Player? RunSimulation(Player player1, Player player2)
    {
        PlayerAnswer answerOfPlayer1, answerOfPlayer2;
        var counter = 0;
        do
        {
            counter++;
            answerOfPlayer2 = PlayTurn(player1, player2);
            answerOfPlayer1 = PlayTurn(player2, player1);
        } while (answerOfPlayer1 != PlayerAnswer.HitAndWholeFleetSunk && answerOfPlayer2 != PlayerAnswer.HitAndWholeFleetSunk);

        var (message, winner) = (answerOfPlayer1, answerOfPlayer2) switch
        {
            (PlayerAnswer.HitAndWholeFleetSunk, PlayerAnswer.HitAndWholeFleetSunk) => ("It is a draw!!!", null),
            (PlayerAnswer.HitAndWholeFleetSunk, _) => ("Player 2 wins!", player2),
            (_, PlayerAnswer.HitAndWholeFleetSunk) => ("Player 1 wins!", player1),
            _ => throw new UnreachableException()
        };

        _logger.LogInformation("Simulation ended in {RoundsNumber} with result: {Message}", counter, message);

        return winner;
    }

    private PlayerAnswer PlayTurn(Player guessingPlayer, Player answeringPlayer)
    {
        var guess = _guessingEngine.Guess(guessingPlayer.GuessingBoard, guessingPlayer.GuessingStrategy);
        _logger.LogInformation("Guessing: {Coords}", guess);

        var answer = answeringPlayer.Answer(guess, out _);
        _logger.LogInformation("Answer: {Answer}", answer);

        guessingPlayer.ApplyAnswerInfo(guess, answer);

        return answer;
    }
}
