using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject flatPlayerPrefab;
    [SerializeField] private GameObject vrPlayerPrefab;
    [SerializeField] private Transform playerSpawnTransform;

    private void Start()
    {
        StartCoroutine(DetectAndSpawnPlayer());
    }
    private IEnumerator DetectAndSpawnPlayer()
    {
        yield return StartCoroutine(InitializeXR());

        bool vrDetected = IsVRHeadSetConnected();

        if (vrDetected)
        {
            Debug.Log("VR detected - spawning VR Player");
            SpawnPlayer(vrPlayerPrefab);
        }
        else
        {
            Debug.Log("No VR detected - spawning Flat Player");
            SpawnPlayer(flatPlayerPrefab);
        }
    }

    private IEnumerator InitializeXR()
    {
        if (XRGeneralSettings.Instance == null ||
            XRGeneralSettings.Instance.Manager == null)
        {
            yield break;
        }
        
        var manager = XRGeneralSettings.Instance.Manager;

        if (!manager.isInitializationComplete)
        {
            manager.InitializeLoaderSync();
        }

        if (manager.activeLoader != null)
        {
            manager.StartSubsystems();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool IsVRHeadSetConnected()
    {
        var displays = new System.Collections.Generic.List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(displays);

        foreach (var display in displays)
        {
            if (display.running)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is null");
            return;
        }
        
        Vector3 position = playerSpawnTransform != null ? playerSpawnTransform.position : Vector3.zero;
        Quaternion rotation = playerSpawnTransform != null ? playerSpawnTransform.rotation : Quaternion.identity;
        GameObject player = Instantiate(playerPrefab, position, rotation);
        GameManager.Instance.SetPlayerPrefab(player);
    }
}
