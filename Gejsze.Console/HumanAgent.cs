using Gejsze.AI.Agents;
using Gejsze.Engine.Actions;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.Console;

public sealed class HumanAgent : IPlayerAgent
{
    public ActionType ChooseAction(
        GameObservation observation,
        IReadOnlyList<ActionType> legalActions)
    {
        System.Console.WriteLine("Choose an action:");

        for (int i = 0; i < legalActions.Count; i++)
        {
            System.Console.WriteLine($"{i + 1}. {legalActions[i]}");
        }

        // TODO: Validate input and handle invalid choices.
        int choice = int.Parse(System.Console.ReadLine()!);

        return legalActions[choice - 1];
    }

    public PlayerDecision ChooseDecision(GameObservation observation)
    {
        PendingDecision pending = observation.PendingDecision
            ?? throw new InvalidOperationException("No pending decision.");

        System.Console.WriteLine("Make your decision:");

        foreach (int cardId in pending.AvailableCardIds)
        {
            System.Console.WriteLine($"Card ID: {cardId}");
        }

        // TODO: Ask the player to make a decision appropriate
        // for pending.Type, and validate their input.

        throw new NotImplementedException();
    }
}