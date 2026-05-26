using System.Collections;
using UnityEngine;

public class BulletTracerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletTracerPrefab;
    [SerializeField] private Transform bulletTracerSpawnPoint;
    public float bulletSpeed = 500f;

    public void SpawnBulletTracer(Vector3 spawnPoint, Vector3 hitPoint)
    {
        GameObject bulletTracerObject = Instantiate(
            bulletTracerPrefab,
            bulletTracerSpawnPoint.transform.position,
            bulletTracerSpawnPoint.transform.rotation);
            StartCoroutine(MoveBulletTracer(bulletTracerObject, spawnPoint, hitPoint));
    }
    private IEnumerator MoveBulletTracer(GameObject bulletTracer, Vector3 startPos, Vector3 endPos)
    {
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / bulletSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float trailProgress = Mathf.Clamp01(elapsedTime / duration);
            bulletTracer.transform.position = Vector3.Lerp(startPos, endPos, trailProgress);
            yield return null;
        }

        bulletTracer.transform.position = endPos;
        
        // Fade out trail before destroying
        TrailRenderer trailRenderer = bulletTracer.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            yield return new WaitForSeconds(trailRenderer.time);
        }
        Destroy(bulletTracer);
    }
}
