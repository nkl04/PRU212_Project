using UnityEngine;

[CreateAssetMenu(fileName = "Card Info", menuName = "New Card")]
public class CardInfo : ScriptableObject
{
    public Suit suit;
    public Rank rank;
    public int score;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rank != null)
        {
            score = rank.GetHashCode();
        }
    }
#endif
}
public enum Suit
{
    Heart,
    Diamond,
    Club,
    Spade
}

public enum Rank
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
}
