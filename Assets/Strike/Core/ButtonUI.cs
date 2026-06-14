using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.black;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDisable()
    {
        buttonText.color = normalColor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        audioSource.PlayOneShot(hoverSound);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}
