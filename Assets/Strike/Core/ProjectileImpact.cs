using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{ 
    public AudioClip targetImpactSound;
    public AudioClip defaultImpactSound;
    private AudioSource audioSource;
    private ParticleSystem impactParticles;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        impactParticles = GetComponentInChildren<ParticleSystem>();
        impactParticles.Play();
        Destroy(gameObject, 3f);
    }
    public void PlayImpactSound(ESurfaceType type)
    {
        switch (type)
        {
            case ESurfaceType.Default:
                audioSource.clip = defaultImpactSound;
                break;
            case ESurfaceType.Target:
                audioSource.clip = targetImpactSound;
                break;
        }
        audioSource.Play();
    }
}
