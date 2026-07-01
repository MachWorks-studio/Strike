using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    [SerializeField] private MeshFilter coverMeshFilter;
    [SerializeField] private MeshCollider coverMeshCollider;
    [SerializeField] private GameObject[] randomCoverPool;
    [SerializeField] private float riseDuration = 0.2f;
    [SerializeField] private float edgeWidth = 0.2f;
    [SerializeField] private float edgeOvershoot = 0.5f;
    
    // Shader Setup
    private Renderer coverRenderer;
    private MaterialPropertyBlock propBlock;
    private Coroutine activeEffectCoroutine;
    private float lastClipHeight;

    private static readonly int ClipHeightID = Shader.PropertyToID("_ClipHeight");
    private static readonly int EdgeWidthID = Shader.PropertyToID("_EdgeWidth");

    public static CoverManager Instance {  get; private set; }

    private void OnEnable()
    {
        GameManager.OnStartRound += EnableCover;
        GameManager.OnEndRound += DisableCover;
        GameManager.OnEndCircuit += DisableCover;
    }

    private void OnDisable()
    {
        GameManager.OnStartRound -= EnableCover;
        GameManager.OnEndRound -= DisableCover;
        GameManager.OnEndCircuit -= DisableCover;
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        coverMeshFilter = cover.GetComponent<MeshFilter>();
        coverMeshCollider = cover.GetComponent<MeshCollider>();
        coverRenderer = cover.GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        cover.gameObject.SetActive(false);
    }

    private void EnableCover()
    {
        cover.gameObject.SetActive(true);
        StartEffect(AnimateClip(rising: true));
    }
    private void DisableCover()
    {
        if (!cover.gameObject.activeSelf) return;
        StartEffect(AnimateClip(rising: false));
    }

    public void SetCoverMesh(RoundTargetData roundData)
    {
        GameObject chosen;

        if (roundData.coverSelectionMode == CoverSelectionMode.Specific)
        {
            chosen = roundData.coverMeshObject;
        }
        else
        {
            chosen = randomCoverPool[Random.Range(0, randomCoverPool.Length)];
        }
        Mesh mesh = chosen.GetComponent<MeshFilter>().sharedMesh;
        coverMeshFilter.mesh = mesh;
        coverMeshCollider.sharedMesh = mesh;
    }

    private void StartEffect(IEnumerator routine)
    {
        if (activeEffectCoroutine != null)
        {
            StopCoroutine(activeEffectCoroutine);
        }
        activeEffectCoroutine = StartCoroutine(routine);
    }
    
    private IEnumerator AnimateClip(bool rising)
    {
        Bounds bounds = coverRenderer.bounds;
        float startY = bounds.min.y - edgeOvershoot;
        float endY = bounds.max.y + edgeOvershoot;
        
        float fromHeight = rising ? startY : lastClipHeight;
        float toHeight = rising ? endY : startY;

        propBlock.SetFloat(EdgeWidthID, edgeWidth);
        
        if (rising)
        {
            SetClipHeight(fromHeight);
        }
        
        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseDuration);
            SetClipHeight(Mathf.Lerp(fromHeight, toHeight, t));
            yield return null;
        }
        
        SetClipHeight(toHeight);
        if (!rising)
        {
            cover.gameObject.SetActive(false);
        }

        activeEffectCoroutine = null;
    }
    
    private void SetClipHeight(float height)
    {
        coverRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(ClipHeightID, height);
        coverRenderer.SetPropertyBlock(propBlock);
        lastClipHeight = height;
    }
}
