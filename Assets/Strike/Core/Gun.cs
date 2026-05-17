using UnityEngine;

public class Gun : MonoBehaviour
{
    private AudioSource firingSound;

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
        firingSound.Play();
    }
}
