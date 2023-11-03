using BattleshipBoardGame.Models.Entities;
using BattleshipBoardGame.Services;
using FluentValidation;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Models.Api;

[UsedImplicitly]
public class PlayerInfo
{
    public string? GuessingStrategy { get; set; }

    public string? ShipsPlacementStrategy { get; set; }
}

[UsedImplicitly]
public class PlayerInfosValidator : AbstractValidator<PlayerInfo[]>
{
    public PlayerInfosValidator()
    {
        RuleFor(x => x).NotNull().WithMessage("You must provide players settings in request body.");
        RuleFor(x => x.Length).Equal(2).WithMessage("Expected settings for exactly two player.");
        RuleForEach(x => x).Where(info => info is not null).SetValidator(new PlayerInfoValidator());
    }
}

public class PlayerInfoValidator : AbstractValidator<PlayerInfo>
{
    public PlayerInfoValidator()
    {
        RuleFor(x => x.GuessingStrategy).IsEnumName(typeof(GuessingStrategy), caseSensitive: false);
        RuleFor(x => x.ShipsPlacementStrategy).IsEnumName(typeof(ShipsPlacementStrategy), caseSensitive: false);
    }
}
