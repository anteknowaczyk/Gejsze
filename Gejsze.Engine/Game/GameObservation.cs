using Gejsze.Engine.Cards;
using Gejsze.Engine.Actions;
using Gejsze.Engine.Decisions;

namespace Gejsze.Engine.Game;

public sealed class GameObservation
{
    public PlayerId MyPlayer { get; init; }

    public PlayerStateView Me { get; init; } = new();

    public PlayerStateView Opponent { get; init; } = new();

    public IReadOnlyList<Card> DiscardPile { get; init; }
        = Array.Empty<Card>();

    public IReadOnlyList<PlayedCard> PlayedCards { get; init; }
        = Array.Empty<PlayedCard>();

    public IReadOnlyList<ActionType> AvailableActions { get; init; }
        = Array.Empty<ActionType>();

    public PendingDecision? PendingDecision { get; init; }

    public PlayerId CurrentPlayer { get; init; }

    public int TurnNumber { get; init; }
}

public sealed class PlayerStateView
{
    public IReadOnlyList<Card> Hand { get; init; }
        = Array.Empty<Card>();

    public int CardCount { get; init; }
}