using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(RectTransform))]
public class ButtonCollider : MonoBehaviour
{
    private BoxCollider _boxCollider;
    private RectTransform _rectTransform;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _rectTransform = GetComponent<RectTransform>();
        SyncCollider();
    }

    void OnEnable()
    {
        StartCoroutine(SyncOnEnable());
    }

    IEnumerator SyncOnEnable()
    {
        yield return null;
        SyncCollider();
    }

    public void SyncCollider()
    {
        if (_boxCollider == null || _rectTransform == null) return;
        _boxCollider.size = new Vector3(_rectTransform.rect.width, _rectTransform.rect.height, 0.1f);
        _boxCollider.center = Vector3.zero;
    }
#if UNITY_EDITOR
    void OnValidate() => SyncCollider();
#endif
}
