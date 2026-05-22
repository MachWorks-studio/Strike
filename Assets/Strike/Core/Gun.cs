using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private AudioSource firingSound;
    [Header("Gun Attributes")]
    [SerializeField] private GameObject projectileOriginLocation;
    [SerializeField] private GameObject projectileImpactPrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private ParticleSystem muzzleFlashParticles;

    private void Awake()
    {
        firingSound = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        muzzleFlashParticles = muzzleFlash.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShoot()
    {
        ESurfaceType targetSurfaceType = ESurfaceType.Default;
        muzzleFlashParticles.Play();
        firingSound.Play();
        if  (Physics.Raycast
                (projectileOriginLocation.transform.position,
                 projectileOriginLocation.transform.TransformDirection(Vector3.forward),
                 out RaycastHit hitInfo,
                 1000f)
            )
        {
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
            
            GameObject projectileImpact = Instantiate(projectileImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            ProjectileImpact projectileImpactScript = projectileImpact.GetComponent<ProjectileImpact>();
            projectileImpactScript.PlayImpactSound(targetSurfaceType);
        }
        else
        {
            Debug.DrawRay(
                projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward) * 10f,
                Color.red,
                2f
                );
        }
    }
}
