namespace Gejsze.Engine.Actions;

using Gejsze.Engine.Game;

public enum ActionResolutionStatus
{
    Completed,
    WaitingForDecision
}

public sealed class ActionContext
{
    public PlayerId InitiatingPlayer { get; }
    public ActionType Type { get; }

    public List<int> SelectedCardIds { get; } = new();

    public List<int> FirstPairIds { get; } = new();
    public List<int> SecondPairIds { get; } = new();

    public int DecisionStep { get; set; }

    public ActionContext(PlayerId initiatingPlayer, ActionType type)
    {
        InitiatingPlayer = initiatingPlayer;
        Type = type;
    }
}