using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    [SerializeField] private MeshFilter coverMeshFilter;
    [SerializeField] private MeshCollider coverMeshCollider;
    [SerializeField] private GameObject[] randomCoverPool;

    public static CoverManager Instance {  get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DisableCover();
        coverMeshFilter = cover.GetComponent<MeshFilter>();
        coverMeshCollider = cover.GetComponent<MeshCollider>();
    }

    private void EnableCover()
    {
        cover.gameObject.SetActive(true);
    }
    private void DisableCover()
    {
        cover.gameObject.SetActive(false);
    }

    public void SetCoverMesh(RoundTargetData roundData)
    {
        GameObject chosen;

        if (roundData.coverSelectionMode == CoverSelectionMode.Specific)
        {
            chosen = roundData.coverMeshObject;
        }
        else
        {
            chosen = randomCoverPool[Random.Range(0, randomCoverPool.Length)];
        }
        Mesh mesh = chosen.GetComponent<MeshFilter>().sharedMesh;
        coverMeshFilter.mesh = mesh;
        coverMeshCollider.sharedMesh = mesh;
    }

    private void OnEnable()
    {
        GameManager.OnStartRound += EnableCover;
        GameManager.OnEndCircuit += DisableCover;
    }

    private void OnDisable()
    {
        GameManager.OnStartRound -= EnableCover;
        GameManager.OnEndCircuit -= DisableCover;
    }
}
