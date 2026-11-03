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

        while (true)
        {
            if (int.TryParse(System.Console.ReadLine(), out int choice) &&
                choice >= 1 &&
                choice <= legalActions.Count)
            {
                return legalActions[choice - 1];
            }

            System.Console.WriteLine("Invalid choice. Try again:");
        }
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

        switch (pending.Type)
        {
            case DecisionType.SelectCards:
                return ChooseCards(pending);

            case DecisionType.ChoosePair:
                return ChoosePair();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private PlayerDecision ChooseCards(PendingDecision pending)
    {
        if (pending.MinimumSelections == 3 &&
            pending.MaximumSelections == 3)
        {
            // PairAndSingle: initiator selects three cards.
            // The same SelectCards decision is also used by the opponent
            // to select one card, so the selection count determines the case.
        }

        if (pending.MinimumSelections == 4 &&
            pending.MaximumSelections == 4)
        {
            // PairAndPair: initiator selects four cards and splits them
            // into two pairs.
            List<int> firstPair = ReadCardIds(2, pending);
            List<int> secondPair = ReadCardIds(2, pending, firstPair);

            return new PlayerDecision
            {
                Player = pending.Player,
                FirstPairIds = firstPair,
                SecondPairIds = secondPair
            };
        }

        List<int> selectedCards = ReadCardIds(
            pending.MinimumSelections,
            pending);

        return new PlayerDecision
        {
            Player = pending.Player,
            SelectedCardIds = selectedCards
        };
    }

    private PlayerDecision ChoosePair()
    {
        while (true)
        {
            System.Console.WriteLine("Choose a pair:");
            System.Console.WriteLine("1. First pair");
            System.Console.WriteLine("2. Second pair");

            if (int.TryParse(System.Console.ReadLine(), out int choice) &&
                (choice == 1 || choice == 2))
            {
                return new PlayerDecision
                {
                    ChosenPair = choice
                };
            }

            System.Console.WriteLine("Invalid choice. Enter 1 or 2.");
        }
    }

    private List<int> ReadCardIds(
        int count,
        PendingDecision pending,
        IReadOnlyCollection<int>? alreadySelected = null)
    {
        while (true)
        {
            System.Console.WriteLine(
                $"Enter {count} card ID(s), separated by spaces:");

            string? input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                System.Console.WriteLine("Invalid input.");
                continue;
            }

            string[] parts = input.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != count ||
                !parts.All(part => int.TryParse(part, out _)))
            {
                System.Console.WriteLine(
                    $"You must enter exactly {count} card ID(s).");
                continue;
            }

            List<int> ids = parts.Select(int.Parse).ToList();

            if (ids.Distinct().Count() != ids.Count)
            {
                System.Console.WriteLine("Duplicate card IDs are not allowed.");
                continue;
            }

            if (ids.Any(id => !pending.AvailableCardIds.Contains(id)))
            {
                System.Console.WriteLine("Invalid card ID.");
                continue;
            }

            if (alreadySelected != null &&
                ids.Any(alreadySelected.Contains))
            {
                System.Console.WriteLine(
                    "These cards have already been selected.");
                continue;
            }

            return ids;
        }
    }
}