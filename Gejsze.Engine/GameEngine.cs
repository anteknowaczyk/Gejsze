using System.Runtime.InteropServices;
using Gejsze.Engine.Actions;
using Gejsze.Engine.Cards;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.Engine;

public sealed class GameEngine
{
    private readonly Random _random;

    public int Seed { get; }

    public GameState State { get; }

    public bool HasPendingDecision =>
        State.PendingDecision != null;

    public GameEngine(int seed)
    {
        Seed = seed;
        _random = new Random(seed);
        State = new GameState();
    }

    public void StartGame()
    {
        // Initialize the deck.
        InitDeck();
        // Deal the initial cards.
        DealStartingCards();
        // Set the starting player.
        State.CurrentPlayer = _random.Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
        // Set the game status to InProgress.
        State.Status = GameStatus.InProgress;
    }

    public GameObservation GetObservation(PlayerId playerId)
    {
        // TODO:
        // Create an observation containing only information
        // that this particular player is allowed to see.
        PlayerState me = State.GetPlayer(playerId);
        PlayerState opponent = State.GetOpponent(playerId);

        return new GameObservation
        {
            MyPlayer = playerId,

            Me = new PlayerStateView
            {
                Hand = me.Hand.ToList(),
                CardCount = me.Hand.Count,
            },

            Opponent = new PlayerStateView
            {
                Hand = Array.Empty<Card>(),
                CardCount = opponent.Hand.Count
            },

            DiscardPile = State.DiscardPile.ToList(),

            PlayedCards = State.Played.ToList(),

            AvailableActions = me.UsedActions.Count == 4
                ? Array.Empty<ActionType>()
                : Enum.GetValues<ActionType>()
                .Where(action => !me.UsedActions.Contains(action))
                .ToList(),

            PendingDecision = State.PendingDecision?.Player == playerId
                ? State.PendingDecision
                : null,

            CurrentPlayer = State.CurrentPlayer,

            TurnNumber = State.TurnNumber
        };
    }

    public IReadOnlyList<ActionType> GetLegalActions(PlayerId playerId)
    {
        // Return actions that:have not yet been used by this player this round
        return Enum.GetValues<ActionType>()
            .Where(action => !State.GetPlayer(playerId).UsedActions.Contains(action)).ToList();
    }

    public void StartAction(PlayerId playerId, ActionType actionType)
    {
        // Verify that the game is in progress.
        if (State.Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("The game is not in progress.");
        }

        // Verify that playerId is the current player.
        if (State.CurrentPlayer != playerId )
        {
            throw new InvalidOperationException("It is not the player's turn.");
        }

        // Verify that actionType is legal.
        if (State.GetPlayer(playerId).UsedActions.Contains(actionType))
        {
            throw new InvalidOperationException("The action is already used.");
        }

        // Mark the action as used.
        State.GetPlayer(playerId).UsedActions.Add(actionType);

        // Create the ActionContext.
        ActionContext context = new ActionContext(playerId, actionType);

        // Start resolving the action.
        State.CurrentAction = context;
        ResolveCurrentAction();
    }

    public void SubmitDecision(PlayerDecision decision)
    {
        // TODO:
        // 1. Verify that a decision is pending.
        // 2. Verify that decision.Player is the player
        //    expected to make the decision.
        // 3. Validate the submitted card IDs.
        // 4. Apply the decision to the current action.
        // 5. Either request another decision or finish the action.

        throw new NotImplementedException();
    }

    private void ResolveCurrentAction()
    {
        // TODO:
        // Resolve the effects of the current action.
        // This may include changing hands, discarding cards,
        // playing cards, and changing scores.
    }

    private void FinishAction()
    {
        // TODO:
        // 1. Clear CurrentAction and PendingDecision.
        // 2. Check whether the round is complete.
        // 3. If not, switch to the other player.
        // 4. If the round is complete, start the next round
        //    or finish the game.
    }

    private void StartNextRound()
    {
        // TODO:
        // Reset each player's UsedActions.
        // Prepare the next round according to your rules.
    }

    private void InitDeck()
    {
        State.DrawPile.Clear();
        State.DiscardPile.Clear();

        int id = 1;

        var cardDistribution = new[]
        {
            (CardType.Red, 2),
            (CardType.Yellow, 2),
            (CardType.Purple, 2),
            (CardType.Blue, 3),
            (CardType.Orange, 3),
            (CardType.Green, 4),
            (CardType.Pink, 5)
        };

        foreach (var (type, quantity) in cardDistribution)
        {
            for (int i = 0; i < quantity; i++)
            {
                State.DrawPile.Add(new Card(type, id++));
            }
        }

        // Randomly delete one card
        State.DrawPile.RemoveAt(_random.Next(State.DrawPile.Count));
        
        // Shuffle
        _random.Shuffle(CollectionsMarshal.AsSpan(State.DrawPile));
    }

    private void DealStartingCards()
    {
        for (int i = 0; i < 6; i++)
        {
            State.Player1.Hand.Add(DrawCard());
            State.Player2.Hand.Add(DrawCard());
        }
    }
    private Card DrawCard()
    {
        if (State.DrawPile.Count == 0)
        {
            throw new InvalidOperationException("Draw pile is empty.");
        }

        int lastIndex = State.DrawPile.Count - 1;

        Card card = State.DrawPile[lastIndex];
        State.DrawPile.RemoveAt(lastIndex);

        return card;
    }
}