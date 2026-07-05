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
    [SerializeField] private GameObject intersectionShaderPlane;
    
    // Shader Setup
    private Renderer coverRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Coroutine activeCoverEffectCoroutine;
    private Coroutine activeFieldEffectCoroutine;
    private float lastClipHeight;

    private static readonly int ClipHeightID = Shader.PropertyToID("_ClipHeight");
    private static readonly int EdgeWidthID = Shader.PropertyToID("_EdgeWidth");
    
    [SerializeField] private float fieldTravelDistance = 50f;
    [SerializeField] private float fieldEffectDuration = 0.08f;

    private Vector3 fieldStartLocalPos;
    private Vector3 fieldEndLocalPos;
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
        materialPropertyBlock = new MaterialPropertyBlock();
        cover.gameObject.SetActive(false);
        intersectionShaderPlane.SetActive(false);
        
        fieldStartLocalPos = intersectionShaderPlane.transform.localPosition;
        fieldEndLocalPos = fieldStartLocalPos + Vector3.forward * fieldTravelDistance;
    }

    private void EnableCover()
    {
        cover.gameObject.SetActive(true);
        StartCoverShaderEffect(AnimateCoverEffect(rising: true));
        StartFieldShaderEffect(AnimateFieldEffect(goingForward: true));
    }
    private void DisableCover()
    {
        if (!cover.gameObject.activeSelf) return;
        StartCoverShaderEffect(AnimateCoverEffect(rising: false));
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

    private void StartCoverShaderEffect(IEnumerator routine)
    {
        if (activeCoverEffectCoroutine != null)
        {
            StopCoroutine(activeCoverEffectCoroutine);
        }
        activeCoverEffectCoroutine = StartCoroutine(routine);
    }

    
    private IEnumerator AnimateCoverEffect(bool rising)
    {
        Bounds bounds = coverRenderer.bounds;
        float startY = bounds.min.y - edgeOvershoot;
        float endY = bounds.max.y + edgeOvershoot;
        
        float fromHeight = rising ? startY : lastClipHeight;
        float toHeight = rising ? endY : startY;

        materialPropertyBlock.SetFloat(EdgeWidthID, edgeWidth);
        
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

        activeCoverEffectCoroutine = null;
    }
    
    private void SetClipHeight(float height)
    {
        coverRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(ClipHeightID, height);
        coverRenderer.SetPropertyBlock(materialPropertyBlock);
        lastClipHeight = height;
    }
    
    private void StartFieldShaderEffect(IEnumerator routine)
    {
        if (activeFieldEffectCoroutine != null)
        {
            StopCoroutine(activeFieldEffectCoroutine);
        }
        activeFieldEffectCoroutine = StartCoroutine(routine);
    }

    private IEnumerator AnimateFieldEffect(bool goingForward)
    {
        Transform planeTransform = intersectionShaderPlane.transform;
        planeTransform.localPosition = fieldStartLocalPos;
        intersectionShaderPlane.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fieldEffectDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fieldEffectDuration);
            planeTransform.localPosition = Vector3.Lerp(fieldStartLocalPos, fieldEndLocalPos, t);
            yield return null;
        }

        planeTransform.localPosition = fieldEndLocalPos;
        intersectionShaderPlane.SetActive(false);
        activeFieldEffectCoroutine = null;
    }
}
