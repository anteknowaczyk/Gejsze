using Gejsze.AI.Agents;
using Gejsze.Engine;
using Gejsze.Console;

int appSeed = 12345;

var engine = new GameEngine(appSeed);

var player1 = new HumanAgent();

var player2 = new RandomAgent(
    appSeed + 1);

var runner = new GameRunner(
    engine,
    player1,
    player2);

runner.Run();