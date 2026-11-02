using Gejsze.Engine.Cards;
using Gejsze.Engine.Actions;

namespace Gejsze.Engine.Game;

public sealed class PlayerState
{
    public PlayerId Id { get; }

    public List<Card> Hand { get; } = new();

    // Actions already used by this player during the current round.
    public HashSet<ActionType> UsedActions { get; } = new();

    public PlayerState(PlayerId id)
    {
        Id = id;
    }
}