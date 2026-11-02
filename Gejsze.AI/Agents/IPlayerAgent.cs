using Gejsze.Engine.Actions;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.AI.Agents;

public interface IPlayerAgent
{
    ActionType ChooseAction(
        GameObservation observation,
        IReadOnlyList<ActionType> legalActions);

    PlayerDecision ChooseDecision(GameObservation observation);
}