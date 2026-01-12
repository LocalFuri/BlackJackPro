using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
  // --- This script is for BOTH player and dealer

  // Get other scripts
  public CardScript cardScript;
  public DeckScript deckScript;

  // Total value of player/dealer's hand
  public int handValue = 0;

  // Betting money
  private int money = 1000;

  // Array of card objects on table
  public GameObject[] hand;
  // Index of next card to be turned over
  public int cardIndex = 0;
  // Tracking aces for 1 to 11 conversions
  List<CardScript> aceList = new List<CardScript>();

  public void StartHand() //mark2
  {      
    GetCard();  //1st card for player and dealer
    //GetCard();  //2nd card for player and dealer
  }

  // Add a hand to the player/dealer's hand
  public int GetCard()
  {
    // Get a card, use deal card to assign sprite and value to card on table
    //int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
    int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
    // Show card on game screen
    hand[cardIndex].GetComponent<Renderer>().enabled = true;

    // Add card value to running total of the hand
    handValue += cardValue;

    // If value is 1, it is an ace
    if (cardValue == 1)
    {
      aceList.Add(hand[cardIndex].GetComponent<CardScript>());
    }
    
    //Check if we should use an 11 instead of a 1
    AceCheck();
    cardIndex++ ;
    Debug.Log(cardIndex);
    return handValue;
  }

  // Search for needed ace conversions, 1 to 11 or vice versa
  public void AceCheck()
  {
    //for each ace in the list check
    foreach (CardScript ace in aceList)
    {
      if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
        {
        // if converting, adjust card object value and hand
        ace.SetValue(11);
        handValue += 10;
      }
      else if (handValue > 21 && ace.GetValueOfCard() == 11)
      {
        // if converting, adjust gameobject value and hand value
        ace.SetValue(1);
        handValue -= 10;
      }
    }
  }

  // Add or subtract from money, for bets
  public void AdjustMoney(int amount)
  {
    money += amount;
  }

  // Output players current money amount
  public int GetMoney()
  {
    return money;
  }

  // Hides all cards, resets the needed variables
 
  public void ResetHand()
  {
    //UnityEditor.EditorApplication.isPlaying = false;
    //return;
    for (int i = 0; i < hand.Length; i++) //=11
    {
     //hand[i].GetComponent<CardScript>().ResetCard(); //what good for????????
     
      //if false you will not see any card!
      hand[i].GetComponent<Renderer>().enabled = false; //will hide: Card1 to Hit9 =11 cards
      //mark5

    }
    cardIndex = 0;
    handValue = 0;
    aceList = new List<CardScript>();
  }
}
 