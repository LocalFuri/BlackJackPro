using UnityEngine;
using UnityEngine.EventSystems;

public class HelpYou : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
  public AudioSource audioSource;

  void Awake()
  {
    if (audioSource == null)
      audioSource = GetComponent<AudioSource>();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!audioSource.isPlaying)
      audioSource.Play();
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    audioSource.Stop();
  }
}
