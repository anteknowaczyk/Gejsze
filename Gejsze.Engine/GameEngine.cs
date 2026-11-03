using System.Runtime.InteropServices;
using Gejsze.Engine.Actions;
using Gejsze.Engine.Cards;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.Engine;

public sealed class GameEngine
{
    private readonly Random _random;

    private readonly ActionResolver _actionResolver;

    public int Seed { get; }

    public GameState State { get; }

    public bool HasPendingDecision =>
        State.PendingDecision != null;

    public GameEngine(int seed)
    {
        Seed = seed;
        _random = new Random(seed);
        State = new GameState();
        _actionResolver = new ActionResolver(State);
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
        _actionResolver.ResolveAction(State.CurrentAction);
    }

    public void SubmitDecision(PlayerDecision decision)
    {
        PendingDecision pending = State.PendingDecision
            ?? throw new InvalidOperationException("No decision is pending.");

        ActionContext context = State.CurrentAction
            ?? throw new InvalidOperationException("No action is in progress.");

        if (decision.Player != pending.Player)
            throw new InvalidOperationException("Wrong player.");

        if (decision.SelectedCardIds.Count < pending.MinimumSelections ||
            decision.SelectedCardIds.Count > pending.MaximumSelections)
            throw new InvalidOperationException("Invalid number of cards.");

        if (decision.SelectedCardIds.Distinct().Count() != decision.SelectedCardIds.Count)
            throw new InvalidOperationException("Duplicate card selections.");

        if (decision.SelectedCardIds.Any(id => !pending.AvailableCardIds.Contains(id)))
            throw new InvalidOperationException("Invalid card selection.");

        State.PendingDecision = null;

        _actionResolver.ResolveDecision(context, decision);

        FinishAction();
    }

    private void FinishAction()
    {
        State.CurrentAction = null;
        State.PendingDecision = null;

        // Check whether both players have completed all four actions.
        bool roundComplete =
            State.Player1.UsedActions.Count == 4 &&
            State.Player2.UsedActions.Count == 4;

        if (roundComplete)
        {
            State.Status = GameStatus.Finished;
            return;
        }

        // Switch to the other player.
        State.CurrentPlayer = State.GetOpponent(State.CurrentPlayer).Id;

        State.TurnNumber++;
    }

    private void StartNextRound()
    {
        State.Player1.UsedActions.Clear();
        State.Player2.UsedActions.Clear();

        State.CurrentPlayer = PlayerId.Player1;
        State.TurnNumber = 1;

        // Prepare the next round here:
        // 1. Clear the cards played this round.
        // 2. Shuffle and prepare the deck.
        // 3. Remove one random card.
        // 4. Deal six cards to each player.
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