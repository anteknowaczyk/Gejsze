using Gejsze.Engine.Game;

namespace Gejsze.Engine.Decisions;

public sealed class PlayerDecision
{
    public PlayerId Player { get; init; }

    public IReadOnlyList<int> SelectedCardIds { get; init; }
        = Array.Empty<int>();
}