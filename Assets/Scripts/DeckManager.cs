using UnityEngine;

public class DeckManager : MonoBehaviour
{
  public CardScript[] deck; // array of 52 cards

  void Start()
  {
    Debug.Log("DeckManager not activated!");
    return;
    
    
    InitializeDeck();
  }

  public void InitializeDeck()
  {
    string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
    int index = 0;

    for (int s = 0; s < suits.Length; s++)
    {
      for (int value = 1; value <= 13; value++)
      {
        //deck[index].cardSuit = suits[s];
        //deck[index].cardValue = value;

        // Optional: assign sprite if you have one array of 52 sprites
        // deck[index].frontSprite = cardSprites[index];

        index++;
      }
    }

    Debug.Log("Deck initialized with custom values!");
  }
}
