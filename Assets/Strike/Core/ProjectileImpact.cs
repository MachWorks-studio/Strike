using UnityEngine;

public class ProjectileImpact : MonoBehaviour
{ 
    public AudioClip targetImpactSound;
    public AudioClip[] defaultImpactSound;
    public AudioClip buttonImpactSound;
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
                audioSource.clip = GetRandomClip(defaultImpactSound);
                break;
            case ESurfaceType.Target:
                audioSource.clip = targetImpactSound;
                break;
            case ESurfaceType.Button:
                audioSource.clip = buttonImpactSound;
                break;
        }
        audioSource.Play();
    }
    
    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
