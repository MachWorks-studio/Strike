using UnityEngine;

public class VRController : MonoBehaviour
{
    [SerializeField] private Gun gun;
    [SerializeField] private Camera vrCamera;

    void Start()
    {
        gun.SetEventCamera(vrCamera);
    }
}
