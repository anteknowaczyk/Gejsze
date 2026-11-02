using Gejsze.AI.Agents;
using Gejsze.Engine;
using Gejsze.Engine.Decisions;
using Gejsze.Engine.Game;

namespace Gejsze.Console;

public sealed class GameRunner
{
    private readonly GameEngine _engine;

    private readonly Dictionary<PlayerId, IPlayerAgent> _agents;

    public GameRunner(
        GameEngine engine,
        IPlayerAgent player1,
        IPlayerAgent player2)
    {
        _engine = engine;

        _agents = new Dictionary<PlayerId, IPlayerAgent>
        {
            { PlayerId.Player1, player1 },
            { PlayerId.Player2, player2 }
        };
    }

    public void Run()
    {
        _engine.StartGame();

        while (_engine.State.Status == GameStatus.InProgress)
        {
            if (_engine.HasPendingDecision)
            {
                HandlePendingDecision();
            }
            else
            {
                HandlePlayerTurn();
            }
        }

        DisplayFinalResults();
    }

    private void HandlePlayerTurn()
    {
        PlayerId player = _engine.State.CurrentPlayer;

        GameObservation observation =
            _engine.GetObservation(player);

        IReadOnlyList<Engine.Actions.ActionType> legalActions =
            _engine.GetLegalActions(player);

        var action = _agents[player].ChooseAction(
            observation,
            legalActions);

        _engine.StartAction(player, action);
    }

    private void HandlePendingDecision()
    {
        PendingDecision pending =
            _engine.State.PendingDecision!;

        PlayerId player = pending.Player;

        GameObservation observation =
            _engine.GetObservation(player);

        PlayerDecision decision =
            _agents[player].ChooseDecision(observation);

        _engine.SubmitDecision(decision);
    }

    private void DisplayFinalResults()
    {
        // TODO: Display scores and the winner.
    }
}