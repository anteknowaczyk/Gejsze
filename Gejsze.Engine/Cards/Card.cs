namespace Gejsze.Engine.Cards;

public class Card
{
    public CardType Type { get; }

    public int Id { get; }

    public Card(CardType type, int id)
    {
        Type = type;
        Id = id;
    }
}