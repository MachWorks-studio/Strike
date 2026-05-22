using UnityEngine;

public class TargetSpawnPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
