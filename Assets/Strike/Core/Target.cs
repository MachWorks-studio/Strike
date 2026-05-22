using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    private AudioSource impactAudioSource;

    void Start()
    {
        impactAudioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(float damage)
    {
        OnTargetHit();
    }
    void OnTargetHit()
    {
        impactAudioSource.Play();
        Debug.Log("TargetHit");
    }
}
