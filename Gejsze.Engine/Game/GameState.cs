using Gejsze.Engine.Cards;
using Gejsze.Engine.Actions;
using Gejsze.Engine.Decisions;

namespace Gejsze.Engine.Game;

public enum GameStatus
{
    NotStarted,
    InProgress,
    Finished
}

public sealed class GameState
{
    public PlayerState Player1 { get; }
    public PlayerState Player2 { get; }

    public PlayerId CurrentPlayer { get; set; }

    public int TurnNumber { get; internal set; }

    public GameStatus Status { get; internal set; }

    public List<Card> DrawPile { get; } = new();
    public List<Card> DiscardPile { get; } = new();

    public List<PlayedCard> Played { get; } = new();

    // Non-null while an action is being resolved.
    public ActionContext? CurrentAction { get; internal set; }

    // Non-null when a player must make a decision.
    public PendingDecision? PendingDecision { get; internal set; }

    public GameState()
    {
        Player1 = new PlayerState(PlayerId.Player1);
        Player2 = new PlayerState(PlayerId.Player2);

        CurrentPlayer = PlayerId.Player1;
        TurnNumber = 1;
        Status = GameStatus.NotStarted;
    }

    public PlayerState GetPlayer(PlayerId id)
    {
        return id == PlayerId.Player1 ? Player1 : Player2;
    }

    public PlayerState GetOpponent(PlayerId id)
    {
        return GetPlayer(
            id == PlayerId.Player1
                ? PlayerId.Player2
                : PlayerId.Player1);
    }
}