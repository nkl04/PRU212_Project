using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Info", menuName = "New Card")]
public class CardInfo : ScriptableObject
{
    public Suit suit;
    public Rank rank;
    public int score;
    public Sprite sprite;

#if UNITY_EDITOR
    private void OnValidate()
    {
        score = rank.GetHashCode();

        int rankInt = rank == Rank.Ace ? 1 : rank.GetHashCode();

        string spriteName = suit.ToString() + " " + rankInt.ToString();

        Sprite sprite = AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Deck/{spriteName}.png", typeof(Sprite)) as Sprite;

        this.sprite = sprite;
    }
#endif
}
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
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
