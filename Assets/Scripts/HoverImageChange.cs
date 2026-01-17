using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Collections;

public class HoverImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ //chatgpt script
  public Sprite normalSprite;
  public Sprite hoverSprite;
  private Image image;
  public Button dealBtn;


  void Awake()
  {
    image = GetComponent<Image>();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    image.sprite = hoverSprite;
    // GetComponent<AudioSource>().Play();
  }

  public void OnPointerExit(PointerEventData eventData)
  { 
    image.sprite = normalSprite;
  }


  public void SetNormal(Button btn)
  {
    btn.image.sprite = normalSprite;
  }

}
