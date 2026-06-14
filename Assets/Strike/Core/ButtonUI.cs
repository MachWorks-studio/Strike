using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.black;
    private MenusUI menusUI;

    private void Awake()
    {
        menusUI = GetComponentInParent<MenusUI>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
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
        menusUI.PlayHoverSound();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}
