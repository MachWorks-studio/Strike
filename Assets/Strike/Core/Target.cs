using UnityEngine;
using System;
using System.Collections;

public class Target : MonoBehaviour, IDamageable
{
    public event Action OnTargetHit;

    [SerializeField] private GameObject targetMesh;
    [SerializeField] private Renderer targetMeshRenderer;
    [SerializeField] private Renderer polePostRenderer;
    [SerializeField] private float endClipHeight = 0f;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float clipStartOffset = 0.1f;

    [Header("Hit Flash")]
    [SerializeField] private Color flashEmissionColor = Color.white;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float poleEffectLeadTime = 0.05f;
    
    private float startClipHeight;
    private MaterialPropertyBlock propBlock;
    private MaterialPropertyBlock meshPropBlock;
    private static readonly int ClipHeightID = Shader.PropertyToID("_ClipHeight");
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    
    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        meshPropBlock = new MaterialPropertyBlock();
        
        float topOfPost = polePostRenderer.bounds.max.y;
        startClipHeight = topOfPost + clipStartOffset;

        polePostRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(ClipHeightID, startClipHeight);
        polePostRenderer.SetPropertyBlock(propBlock);
    }
    public void TakeDamage(float damage)
    {
        TargetHit();
    }
    void TargetHit()
    {
        OnTargetHit?.Invoke();
        StartCoroutine(TargetHitSequence());
    }

    public ESurfaceType GetSurfaceType()
    {
        return ESurfaceType.Target;
    }

    public void SetSurfaceType(ESurfaceType type)
    {
        type = ESurfaceType.Target;
    }
    
    private IEnumerator TargetHitSequence()
    {
        StartCoroutine(HitFlashCoroutine());

        float waitTime = Mathf.Max(0f, flashDuration - poleEffectLeadTime);
        yield return new WaitForSeconds(waitTime);

        StartCoroutine(PolePostEffectCoroutine());
    }
    
    private IEnumerator PolePostEffectCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            float currentClipHeight = Mathf.Lerp(startClipHeight, endClipHeight, t);

            polePostRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(ClipHeightID, currentClipHeight);
            polePostRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }

        polePostRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(ClipHeightID, endClipHeight);
        polePostRenderer.SetPropertyBlock(propBlock);

        Destroy(gameObject);
    }
    private IEnumerator HitFlashCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;

            float intensity = Mathf.Sin(t * Mathf.PI);
            Color currentEmission = flashEmissionColor * intensity;

            targetMeshRenderer.GetPropertyBlock(meshPropBlock);
            meshPropBlock.SetColor(EmissionColorID, currentEmission);
            targetMeshRenderer.SetPropertyBlock(meshPropBlock);

            yield return null;
        }

        targetMesh.SetActive(false);
    }
}
