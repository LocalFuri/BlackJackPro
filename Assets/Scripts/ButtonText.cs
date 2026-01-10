using UnityEngine;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour
{
  public Text Textfield;  //Textfield = var name
  public void SetText(string text)
  {
    //Textfield.text = text;

    //int[] cardValues = new int[53];

    //for (int i = cardSprites.Length - 1; i > 0; --i)
    //for (int i = 0; i < 1; i++)
    {
      int rndNumber = Random.Range(51, 53);
      Textfield.text = rndNumber.ToString();
      Debug.Log(rndNumber) ;

      //int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * cardSprites.Length - 1) + 1;
    }
  }
}
