using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }
    [Header("Lighting")]
    [SerializeField] private Light directionalLight;

    [Header("Default Lighting Values")]
    private float defaultLightIntensity;
    private Color defaultLightColor;
    private float defaultAmbientIntensity;
    private Color defaultAmbientSkyColor;
    private Color defaultAmbientEquatorColor;
    private Color defaultAmbientGroundColor;
    private float defaultReflectionIntensity;
    private AmbientMode defaultAmbientMode;
    private Color defaultSkyboxTint;
    private Material skyboxInstance;
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    
    private Coroutine activeFadeCoroutine;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (RenderSettings.skybox != null)
        {
            skyboxInstance = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyboxInstance;
        }
        CacheDefaultLightingValues();
    }
    private void CacheDefaultLightingValues()
    {
        if (directionalLight != null)
        {
            defaultLightIntensity = directionalLight.intensity;
            defaultLightColor = directionalLight.color;
        }

        defaultAmbientMode = RenderSettings.ambientMode;
        defaultAmbientIntensity = RenderSettings.ambientIntensity;
        defaultAmbientSkyColor = RenderSettings.ambientSkyColor;
        defaultAmbientEquatorColor = RenderSettings.ambientEquatorColor;
        defaultAmbientGroundColor = RenderSettings.ambientGroundColor;
        defaultReflectionIntensity = RenderSettings.reflectionIntensity;
        
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_SkyTint"))
            defaultSkyboxTint = RenderSettings.skybox.GetColor("_SkyTint");
        else
            defaultSkyboxTint = Color.white;
    } 
    public void SetBlackout()
    {
        if (directionalLight != null)
            directionalLight.intensity = 0f;
        if (skyboxInstance != null)
            skyboxInstance.SetColor("_SkyTint", Color.black);

        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.reflectionIntensity = 0f;
    }

    public void SetDefaults()
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = defaultLightIntensity;
            directionalLight.color = defaultLightColor;
        }
        
        if (skyboxInstance != null)
            skyboxInstance.SetColor("_SkyTint", defaultSkyboxTint);

        DynamicGI.UpdateEnvironment();

        RenderSettings.ambientMode = defaultAmbientMode;
        RenderSettings.ambientIntensity = defaultAmbientIntensity;
        RenderSettings.ambientSkyColor = defaultAmbientSkyColor;
        RenderSettings.ambientEquatorColor = defaultAmbientEquatorColor;
        RenderSettings.ambientGroundColor = defaultAmbientGroundColor;
        RenderSettings.reflectionIntensity = defaultReflectionIntensity;
        
        DynamicGI.UpdateEnvironment();
    }
    
    public void FadeToBlackout(float duration = -1f)
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        activeFadeCoroutine = StartCoroutine(FadeCoroutine(false, duration < 0 ? fadeDuration : duration));
    }
    
    public void FadeToDefaults(float duration = -1f)
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        activeFadeCoroutine = StartCoroutine(FadeCoroutine(true, duration < 0 ? fadeDuration : duration));
    }
    
    private IEnumerator FadeCoroutine(bool toDefaults, float duration)
    {
        float elapsed = 0f;

        // Snapshot current values as fade start points
        float startLightIntensity = directionalLight != null ? directionalLight.intensity : 0f;
        float startAmbientIntensity = RenderSettings.ambientIntensity;
        float startReflectionIntensity = RenderSettings.reflectionIntensity;
        Color startSkyboxTint = (skyboxInstance != null && RenderSettings.skybox.HasProperty("_SkyTint"))
            ? skyboxInstance.GetColor("_SkyTint")
            : Color.white;

        float targetLightIntensity     = toDefaults ? defaultLightIntensity : 0f;
        float targetAmbientIntensity   = toDefaults ? defaultAmbientIntensity : 0f;
        float targetReflectionIntensity = toDefaults ? defaultReflectionIntensity : 0f;
        Color targetSkyboxTint     = toDefaults ? defaultSkyboxTint : Color.black;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (directionalLight != null)
                directionalLight.intensity = Mathf.Lerp(startLightIntensity, targetLightIntensity, t);

            RenderSettings.ambientIntensity   = Mathf.Lerp(startAmbientIntensity, targetAmbientIntensity, t);
            RenderSettings.reflectionIntensity = Mathf.Lerp(startReflectionIntensity, targetReflectionIntensity, t);

            if (skyboxInstance != null && RenderSettings.skybox.HasProperty("_SkyTint"))
                skyboxInstance.SetColor("_SkyTint", Color.Lerp(startSkyboxTint, targetSkyboxTint, t));

            DynamicGI.UpdateEnvironment();

            yield return null;
        }

        // Snap to final values to avoid float imprecision
        if (toDefaults)
            SetDefaults();
        else
            SetBlackout();

        activeFadeCoroutine = null;
    }
    private void Start()
    {
        SetBlackout();
        FadeToDefaults();
    }
}
