using Gejsze.Engine.Game;

namespace Gejsze.Engine.Cards;

public sealed class PlayedCard
{
    public Card Card { get; }
    public PlayerId PlayedBy { get; }
    public int TurnNumber { get; }

    public PlayedCard(Card card, PlayerId playedBy, int turnNumber)
    {
        Card = card;
        PlayedBy = playedBy;
        TurnNumber = turnNumber;
    }
}