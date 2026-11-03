using Gejsze.Engine.Game;

namespace Gejsze.Engine.Decisions;

public sealed class PendingDecision
{
    public PlayerId Player { get; }

    public DecisionType Type { get; }

    public IReadOnlyList<int> AvailableCardIds { get; }

    public int MinimumSelections { get; }

    public int MaximumSelections { get; }

    public PendingDecision(
        PlayerId player,
        DecisionType type,
        IEnumerable<int> availableCardIds,
        int minimumSelections,
        int maximumSelections)
    {
        Player = player;
        Type = type;
        AvailableCardIds = availableCardIds.ToList();
        MinimumSelections = minimumSelections;
        MaximumSelections = maximumSelections;
    }
}