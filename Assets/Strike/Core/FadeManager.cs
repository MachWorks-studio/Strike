using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private Canvas fadeCanvas;
    [SerializeField] private float defaultDuration = 1f;

    [Header("VR Settings")]
    [SerializeField] private float vrDistance = 0.15f;
    [SerializeField] private Vector2 vrCanvasSize = new Vector2(0.28f, 0.16f);

    private Camera _mainCam;
    private bool _isVR;

    public event Action OnFadeToBlackComplete;
    public event Action OnFadeFromBlackComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _mainCam = Camera.main;
        _isVR = XRSettings.isDeviceActive;

        SetupCanvas();
    }

    private void SetupCanvas()
    {
        if (_isVR)
        {
            // World space, parented to camera
            fadeCanvas.renderMode = RenderMode.WorldSpace;
            fadeCanvas.transform.SetParent(_mainCam.transform, false);
            fadeCanvas.transform.localPosition = new Vector3(0f, 0f, vrDistance);
            fadeCanvas.transform.localRotation = Quaternion.identity;

            RectTransform rt = fadeCanvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1f, 1f);
            fadeCanvas.transform.localScale = new Vector3(vrCanvasSize.x, vrCanvasSize.y, 1f);
        }
        else
        {
            // Flat: Screen Space Overlay, unparent from camera
            fadeCanvas.transform.SetParent(null, false);
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 999;
        }

        // Start fully transparent
        SetAlpha(0f);
    }

    public void FadeToBlack(float duration = -1f, System.Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration < 0 ? defaultDuration : duration, onComplete));
    }

    public void FadeFromBlack(float duration = -1f, System.Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration < 0 ? defaultDuration : duration, onComplete));
    }

    public void FadeTransition(System.Action midpointAction, float duration = -1f)
    {
        float d = duration < 0 ? defaultDuration : duration;
        FadeToBlack(d, () => {
            midpointAction?.Invoke();
            FadeFromBlack(d);
        });
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
        onComplete?.Invoke();
        
        if (to >= 1f) OnFadeToBlackComplete?.Invoke();
        else OnFadeFromBlackComplete?.Invoke();
    }

    private void SetAlpha(float a)
    {
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}