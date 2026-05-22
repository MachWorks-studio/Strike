using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{
    [SerializeField] private AudioClip impactSound;
    private AudioSource audioSource;
    private ParticleSystem impactParticles;
    void Start()
    {
        impactParticles = GetComponentInChildren<ParticleSystem>();
        impactParticles.Play();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Destroy(gameObject, 3f);
    }
}
