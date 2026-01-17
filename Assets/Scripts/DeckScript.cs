using System.Xml.Linq;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
  public Sprite[] cardSprites;
  int[] cardValues = new int[53];
  int currentIndex = 0;

  void Start()
  {
    GetCardValues();
  }

   void GetCardValues()
  {
    int num = 0;
    // Loop to assign values to the cards
    for (int i = 0; i < cardSprites.Length; i++)
    {
      num = i;// Count up to the amout of cards, 52
      num %= 13;
      // if there is a remainder after x/13, then remainder
      // is used as the value, unless over 10, the use 10
      if (num > 10 || num == 0)
      {
        num = 10;
      }
      cardValues[i] = num++;
    }
    currentIndex = 1;
  }

  public void Shuffle()
  { //Fisher / Yastes Algorythm
    for (int i = cardSprites.Length - 1; i > 0; --i)
    {
      int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * cardSprites.Length - 1) + 1;
      //int rndNumber = Random.Range(1, 53); //alternativ

      //Sprite face = variable name.It will store one card image.
      Sprite face = cardSprites[i];
      cardSprites[i] = cardSprites[j];
      cardSprites[j] = face;
      //check if tuples work

      int value = cardValues[i];
      cardValues[i] = cardValues[j];
      cardValues[j] = value;
    }
    currentIndex = 1;
  }

  public void RestoreDeck() //restore like it was 1st init
  {
    for (int i = cardSprites.Length - 1; i > 0; --i)
    {
      cardSprites[i] = cardSprites[i]; //this will return the original array, deactivte later
    }
    currentIndex = 1;
  }
  public int DealCard(CardScript cardScript)
  {
    cardScript.SetSprite(cardSprites[currentIndex]);
    cardScript.SetValue(cardValues[currentIndex]);
    currentIndex++;
    return cardScript.GetValueOfCard();
  }

  public Sprite GetCardBack()
  {
    return cardSprites[0];
  }
}
