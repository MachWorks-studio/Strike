using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private AudioSource firingSound;
    [Header("Gun Attributes")]
    [SerializeField] private GameObject projectileOriginLocation;
    [SerializeField] private GameObject projectileImpactPrefab;

    private void Awake()
    {
        firingSound = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShoot()
    {
        if  (Physics.Raycast(
                projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward),
                out RaycastHit hitInfo,
                1000f
                )
            )
        {
            Debug.DrawRay(
                projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward) * hitInfo.distance,
                Color.green,
                2f
                );
            Debug.Log("Hit Something");
            Instantiate(projectileImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        }
        else
        {
            Debug.DrawRay(
                projectileOriginLocation.transform.position,
                projectileOriginLocation.transform.TransformDirection(Vector3.forward) * 10f,
                Color.red,
                2f
                );
            Debug.Log("Hit Nothing");
        }
        firingSound.Play();
    }
}
