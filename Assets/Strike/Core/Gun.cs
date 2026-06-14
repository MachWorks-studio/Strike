using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class Gun : MonoBehaviour
{
    private AudioSource firingSound;
    [Header("Gun Attributes")]
    [SerializeField] private GameObject projectileOriginLocation;
    [SerializeField] private GameObject projectileImpactPrefab;
    [SerializeField] private GameObject muzzleFlash;
    [Header("Laser Properties")]
    [SerializeField] private Transform LaserOrigin;
    [SerializeField] private float maxLaserDistance = 1000f;
    [SerializeField] private LayerMask laserLayerMask;
    [SerializeField] private GameObject laserImpactPrefab;
    [SerializeField] private Material laserDefaultMaterial;
    [SerializeField] private Material laserHoverMaterial;
    private bool laserEnabled = true;
    private bool isHoveringUI;
    private GameObject laserDecal;
    private LineRenderer laserLineRenderer;
    
    private Button currentHoverButton;
    private ParticleSystem muzzleFlashParticles;
    private Animator gunAnimator;
    private Camera eventCamera;

    [SerializeField] private InputActionProperty VRShoot;
    [SerializeField] private float hapticAmplitude = 0.1f;
    [SerializeField] private float hapticDuration = 0.02f;
    
    [SerializeField] private BulletTracerSpawner bulletTracerSpawner;

    void OnEnable()
    {
        VRShoot.action.performed += OnVRShootPerformed;
        GameManager.OnStartCircuit += DisableLaser;
        GameManager.OnEndCircuit += EnableLaser;
    }

    void OnDisable()
    {
        VRShoot.action.performed -= OnVRShootPerformed;
        GameManager.OnStartCircuit -= DisableLaser;
        GameManager.OnEndCircuit -= EnableLaser;
    }

    private void Awake()
    {
        firingSound = GetComponent<AudioSource>();
        gunAnimator = GetComponentInChildren<Animator>();
        muzzleFlashParticles = muzzleFlash.GetComponent<ParticleSystem>();
        
        laserLineRenderer = GetComponent<LineRenderer>();
        laserLineRenderer.positionCount = 2;
        laserLineRenderer.useWorldSpace = true;
        if (laserImpactPrefab != null)
            laserDecal = Instantiate(laserImpactPrefab);
    }
    void Start()
    {
    }
    void Update()
    {
        if (laserEnabled)
            UpdateLaser();
    }
    void OnVRShootPerformed(InputAction.CallbackContext context)
    {
        Shoot();
    }
    //Shoot Input from FlatPlayer
    public void OnShoot()
    {
        Shoot();
    }

    private void Shoot()
    {
        // Haptic feedback for right VR controller on shoot
        var vrController = VRShoot.action.activeControl;
        if (vrController?.device is UnityEngine.InputSystem.XR.XRControllerWithRumble rumbleController)
        {
            rumbleController.SendImpulse(hapticAmplitude, hapticDuration);
        }
        
        // Gun shoot feedback
        ESurfaceType targetSurfaceType = ESurfaceType.Default;
        gunAnimator.Play("Shoot", 0, 0f);
        muzzleFlashParticles.Play();
        firingSound.Play();
        
        // Shoot Physics Raycast
        Ray shootRay = new Ray(projectileOriginLocation.transform.position, projectileOriginLocation.transform.TransformDirection(Vector3.forward));
        if  (Physics.Raycast(shootRay, out RaycastHit hitInfo, 1000f))
        {
            Button button = hitInfo.collider.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
                return;
            }
            // Raycast Success
            IDamageable damageable = hitInfo.collider.GetComponentInParent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(1f);
                targetSurfaceType = damageable.GetSurfaceType();
            }
            
            // Play projectile impact effects
            GameObject projectileImpact = Instantiate(projectileImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            ProjectileImpact projectileImpactScript = projectileImpact.GetComponent<ProjectileImpact>();
            projectileImpactScript.PlayImpactSound(targetSurfaceType);
            
            // Spawn bullet tracer
            bulletTracerSpawner.SpawnBulletTracer(projectileOriginLocation.transform.position, hitInfo.point);
        }
        else
        {
            //Raycast Failed
            Debug.DrawRay(
                projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward) * 10f,
                Color.red,
                2f
            );
            // Spawn bullet tracer going to empty space here
        }
    }
    private void UpdateLaser()
    {
        Vector3 laserOrigin = LaserOrigin.position;
        Vector3 laserEnd;
        Button hoveredButton = null;
        
        if (Physics.Raycast(laserOrigin, LaserOrigin.forward, out RaycastHit laserHitInfo, maxLaserDistance,laserLayerMask))
        {
            laserEnd = laserHitInfo.point;
            hoveredButton = laserHitInfo.collider.GetComponentInParent<Button>();
            isHoveringUI = hoveredButton != null;
            
            laserDecal.SetActive(true);
            laserDecal.transform.position = laserHitInfo.point;
            laserDecal.transform.rotation = Quaternion.LookRotation(laserHitInfo.normal);
        }
        else
        {
            isHoveringUI = false;
            laserEnd = laserOrigin + transform.forward * maxLaserDistance;
        }
        
        laserLineRenderer.enabled = true;
        laserLineRenderer.material = isHoveringUI ? laserHoverMaterial : laserDefaultMaterial;
        laserLineRenderer.SetPosition(0, laserOrigin);
        laserLineRenderer.SetPosition(1, laserEnd);
        HandleButtonHover(hoveredButton);
    }
    private void EnableLaser() => laserEnabled = true;
    private void DisableLaser()
    {
        laserEnabled = false;
        laserLineRenderer.enabled = false;
        laserDecal.SetActive(false);
        laserLineRenderer.material = laserDefaultMaterial;
    }

    private void HandleButtonHover(Button button)
    {
        if (button == currentHoverButton) return;
        if (currentHoverButton != null)
            ExecuteEvents.Execute(currentHoverButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        currentHoverButton = button;
        if (currentHoverButton != null)
            ExecuteEvents.Execute(currentHoverButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
    }
    public void SetEventCamera(Camera cam) => eventCamera = cam;
}
