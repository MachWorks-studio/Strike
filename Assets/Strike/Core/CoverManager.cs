using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    [SerializeField] private MeshFilter coverMeshFilter;
    [SerializeField] private MeshCollider coverMeshCollider;
    [SerializeField] private Mesh[] coverMeshes;

    private HashSet<int> usedIndices = new HashSet<int>();
    
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

    public void SetCoverMesh(GameObject meshObject)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
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

    Mesh GetRandomUnusedCoverMesh()
    {
        // Reset if all meshes have been used
        if(usedIndices.Count >= coverMeshes.Length)
            usedIndices.Clear();
        
        // Builds a list of unused mesh indices
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < coverMeshes.Length; i++)
        {
            if (!usedIndices.Contains(i))
                availableIndices.Add(i);
        }
        
        // Pck a random unused mesh index
        int chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        usedIndices.Add(chosenIndex);
        return coverMeshes[chosenIndex];
    }
}
