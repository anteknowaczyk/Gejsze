using Gejsze.Engine.Actions;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.AI.Agents;

public sealed class RandomAgent : IPlayerAgent
{
    private readonly Random _random;

    public RandomAgent(int seed)
    {
        _random = new Random(seed);
    }

    public ActionType ChooseAction(
        GameObservation observation,
        IReadOnlyList<ActionType> legalActions)
    {
        // TODO: Improve the strategy later.
        return legalActions[_random.Next(legalActions.Count)];
    }

    public PlayerDecision ChooseDecision(GameObservation observation)
    {
        // TODO: Implement decision selection based on DecisionType.
        // For now, randomly select a valid number of available cards.

        PendingDecision pending = observation.PendingDecision
            ?? throw new InvalidOperationException("No pending decision.");

        List<int> available = pending.AvailableCardIds
            .OrderBy(_ => _random.Next())
            .ToList();

        int count = _random.Next(
            pending.MinimumSelections,
            pending.MaximumSelections + 1);

        return new PlayerDecision
        {
            Player = observation.MyPlayer,
            SelectedCardIds = available.Take(count).ToList()
        };
    }
}