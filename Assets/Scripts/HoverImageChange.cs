using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{ //chatgpt script
  public Sprite normalSprite;
  public Sprite hoverSprite;
  private Image image;

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
}
