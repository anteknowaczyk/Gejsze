namespace Gejsze.Engine.Actions;

using Gejsze.Engine.Game;

public sealed class ActionContext
{
    public PlayerId InitiatingPlayer { get; }

    public ActionType Type { get; }

    // Cards selected during this action.
    public List<int> SelectedCardIds { get; } = new();

    public ActionContext(PlayerId initiatingPlayer, ActionType type)
    {
        InitiatingPlayer = initiatingPlayer;
        Type = type;
    }
}