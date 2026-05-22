using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem impactParticles;
    void Start()
    {
        impactParticles = GetComponentInChildren<ParticleSystem>();
        impactParticles.Play();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Destroy(gameObject, 3f);
    }
}
