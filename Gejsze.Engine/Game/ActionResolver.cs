
using Gejsze.Engine.Actions;
using Gejsze.Engine.Cards;
using Gejsze.Engine.Decisions;

namespace Gejsze.Engine.Game;

public enum ActionResolutionStatus
{
    Completed,
    WaitingForDecision
}

public sealed class ActionResolver
{
    private readonly GameState _state;

    public ActionResolver(GameState state)
    {
        _state = state;
    }

    public ActionResolutionStatus ResolveAction(ActionContext context)
    {
        switch (context.Type)
        {
            case ActionType.DiscardPair:
                ResolveDiscardPair(context);
                break;

            case ActionType.PairAndPair:
                ResolvePairAndPair(context);
                break;

            case ActionType.PairAndSingle:
                ResolvePairAndSingle(context);
                break;

            case ActionType.PlaySingle:
                ResolvePlaySingle(context);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(context.Type));
        }

        return ActionResolutionStatus.WaitingForDecision;
    }

    private void ResolveDiscardPair(ActionContext context)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        RequestCardSelection(
            player.Id,
            player.Hand.Select(card => card.Id),
            2,
            2);
    }

    private void ResolvePlaySingle(ActionContext context)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        RequestCardSelection(
            player.Id,
            player.Hand.Select(card => card.Id),
            1,
            1);
    }

    private void ResolvePairAndPair(ActionContext context)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        RequestCardSelection(
            player.Id,
            player.Hand.Select(card => card.Id),
            4,
            4);
    }

    private void ResolvePairAndSingle(ActionContext context)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        RequestCardSelection(
            player.Id,
            player.Hand.Select(card => card.Id),
            3,
            3);
    }

    public ActionResolutionStatus ResolveDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        switch (context.Type)
        {
            case ActionType.DiscardPair:
                ResolveDiscardPairDecision(context, decision);
                break;

            case ActionType.PlaySingle:
                ResolvePlaySingleDecision(context, decision);
                break;

            case ActionType.PairAndPair:
                if (context.DecisionStep == 0)
                {
                    ResolvePairAndPairDecision(context, decision);
                    return ActionResolutionStatus.WaitingForDecision;
                }

                ResolvePairAndPairOpponentDecision(context, decision);
                break;

            case ActionType.PairAndSingle:
                if (context.DecisionStep == 0)
                {
                    ResolvePairAndSingleDecision(context, decision);
                    return ActionResolutionStatus.WaitingForDecision;
                }

                ResolvePairAndSingleOpponentDecision(context, decision);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(context.Type));
        }

        return ActionResolutionStatus.Completed;
    }

    private void ResolveDiscardPairDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        foreach (int cardId in decision.SelectedCardIds)
        {
            Card card = player.Hand.First(c => c.Id == cardId);

            player.Hand.Remove(card);
            _state.DiscardPile.Add(card);
        }
    }

    private void ResolvePlaySingleDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        PlayerState player = _state.GetPlayer(context.InitiatingPlayer);

        int cardId = decision.SelectedCardIds[0];

        Card card = player.Hand.First(c => c.Id == cardId);

        player.Hand.Remove(card);

        _state.Played.Add(
            new PlayedCard(card, player.Id, _state.TurnNumber));
    }

    // Initiating player submits two pairs.
    private void ResolvePairAndPairDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        context.FirstPairIds.Clear();
        context.FirstPairIds.AddRange(decision.FirstPairIds);

        context.SecondPairIds.Clear();
        context.SecondPairIds.AddRange(decision.SecondPairIds);

        context.DecisionStep = 1;

        PlayerId opponent = _state.GetOpponent(
            context.InitiatingPlayer).Id;

        RequestPairChoice(
            opponent,
            context.FirstPairIds,
            context.SecondPairIds);
    }

    // Opponent chooses which pair goes to their tableau.
    private void ResolvePairAndPairOpponentDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        List<int> opponentPair = decision.ChosenPair == 1
            ? context.FirstPairIds
            : context.SecondPairIds;

        List<int> initiatorPair = decision.ChosenPair == 1
            ? context.SecondPairIds
            : context.FirstPairIds;

        PlayerState initiator = _state.GetPlayer(
            context.InitiatingPlayer);

        PlayerId opponentId = _state.GetOpponent(
            context.InitiatingPlayer).Id;

        // Move opponent's pair.
        foreach (int id in opponentPair)
        {
            Card card = initiator.Hand.First(c => c.Id == id);
            initiator.Hand.Remove(card);

            _state.Played.Add(
                new PlayedCard(card, opponentId, _state.TurnNumber));
        }

        // Move initiating player's pair.
        foreach (int id in initiatorPair)
        {
            Card card = initiator.Hand.First(c => c.Id == id);
            initiator.Hand.Remove(card);

            _state.Played.Add(
                new PlayedCard(card, initiator.Id, _state.TurnNumber));
        }
    }

    private void ResolvePairAndSingleDecision(
        ActionContext context,
        PlayerDecision decision)
    {
        context.SelectedCardIds.Clear();
        context.SelectedCardIds.AddRange(decision.SelectedCardIds);

        context.DecisionStep = 1;

        PlayerId opponent = _state.GetOpponent(
            context.InitiatingPlayer).Id;

        RequestCardSelection(
            opponent,
            context.SelectedCardIds,
            1,
            1);
    }

    private void ResolvePairAndSingleOpponentDecision(
    ActionContext context,
    PlayerDecision decision)
    {
        PlayerState initiator = _state.GetPlayer(
            context.InitiatingPlayer);

        PlayerId opponentId = _state.GetOpponent(
            context.InitiatingPlayer).Id;

        int opponentCardId = decision.SelectedCardIds[0];

        foreach (int id in context.SelectedCardIds)
        {
            Card card = initiator.Hand.First(c => c.Id == id);
            initiator.Hand.Remove(card);

            PlayerId destination = id == opponentCardId
                ? opponentId
                : initiator.Id;

            _state.Played.Add(
                new PlayedCard(card, destination, _state.TurnNumber));
        }
    }

    private void RequestCardSelection(
        PlayerId player,
        IEnumerable<int> availableCardIds,
        int minSelections,
        int maxSelections)
    {
        _state.PendingDecision = new PendingDecision(
            player,
            DecisionType.SelectCards,
            availableCardIds,
            minSelections,
            maxSelections);
    }

    private void RequestPairChoice(
        PlayerId player,
        IEnumerable<int> firstPairIds,
        IEnumerable<int> secondPairIds)
    {
        _state.PendingDecision = new PendingDecision(
            player,
            DecisionType.ChoosePair,
            firstPairIds.Concat(secondPairIds),
            0,
            0);
    }
}
