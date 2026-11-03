using Gejsze.Engine.Game;

public enum DecisionType
{
    SelectCards,
    ChoosePair
}
public sealed class PlayerDecision
{
    public PlayerId Player { get; init; }

    public DecisionType Type { get; init; }

    // DiscardPair, PlaySingle
    public IReadOnlyList<int> SelectedCardIds { get; init; }
        = Array.Empty<int>();

    // PairAndPair
    public IReadOnlyList<int> FirstPairIds { get; init; }
        = Array.Empty<int>();

    public IReadOnlyList<int> SecondPairIds { get; init; }
        = Array.Empty<int>();

    // Opponent's choice: 1 or 2
    public int? ChosenPair { get; init; }
}