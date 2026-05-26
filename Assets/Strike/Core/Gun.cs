using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    private AudioSource firingSound;
    [Header("Gun Attributes")]
    [SerializeField] private GameObject projectileOriginLocation;
    [SerializeField] private GameObject projectileImpactPrefab;
    [SerializeField] private GameObject muzzleFlash;
    private ParticleSystem muzzleFlashParticles;
    private Animator gunAnimator;
    private Camera eventCamera;

    public InputActionProperty VRShoot;
    public float hapticAmplitude = 0.1f;
    public float hapticDuration = 0.02f;

    void OnEnable() => VRShoot.action.performed += OnVRShootPerformed;
    void OnDisable() => VRShoot.action.performed -= OnVRShootPerformed;

    private void Awake()
    {
        firingSound = GetComponent<AudioSource>();
        gunAnimator = GetComponentInChildren<Animator>();
        muzzleFlashParticles = muzzleFlash.GetComponent<ParticleSystem>();
    }
    void Start()
    {
    }
    void Update()
    {
        
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
            // Raycast Success
            Debug.DrawRay
            (projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward) * hitInfo.distance,
                Color.green,
                2f);
            
            IDamageable damageable = hitInfo.collider.GetComponentInParent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(1f);
                targetSurfaceType = damageable.GetSurfaceType();
            }
            
            //Play projectile impact effects
            GameObject projectileImpact = Instantiate(projectileImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            ProjectileImpact projectileImpactScript = projectileImpact.GetComponent<ProjectileImpact>();
            projectileImpactScript.PlayImpactSound(targetSurfaceType);
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
        }
        ShootUIRaycast(shootRay);
    }

    private void ShootUIRaycast(Ray shootRayUI)
    {
        if (eventCamera == null) return;
        
        Vector2 screenPosition = eventCamera.WorldToScreenPoint(shootRayUI.origin + shootRayUI.direction);

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition,
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
                return;
            }
        }
    }
    public void SetEventCamera(Camera cam) => eventCamera = cam;
}
