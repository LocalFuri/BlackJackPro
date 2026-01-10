using UnityEngine;

public class CardScript : MonoBehaviour
{


  // Value of card, 2 of clubs = 2, etc
  public int value = 0; //Stores the card’s Blackjack value
  public int GetValueOfCard() //Get ard Value //k
  {
    return value; 
  }

  public void SetValue(int newValue)  //This introduces encapsulation
  { //Later, rules can be enforced inside SetValue
    value = newValue;
  }

  public string GetSpriteName()
  { //Reads the current sprite name
    return GetComponent<SpriteRenderer>().sprite.name; //Useful for debugging or matching card data
  }

  public void SetSprite(Sprite newSprite)
  { //Visually changes the card
    gameObject.GetComponent<SpriteRenderer>().sprite = newSprite; //Keeps visual logic inside the card
  }

  public void ResetCard()
  { //Finds the Deck
    //Gets the card back sprite
    //Resets the card visually and logically

    Sprite back = GameObject.Find("Deck")
      .GetComponent<DeckScript>()
      .GetCardBack();

    gameObject.GetComponent<SpriteRenderer>().sprite = back;
    value = 0;
  }
}
