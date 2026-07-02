using UnityEngine;

public class VRController : MonoBehaviour, IPlayerController
{
    [SerializeField] private Gun gun;
    [SerializeField] private Camera vrCamera;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private bool isRightHanded = true;

    void Start()
    {
        gun.SetEventCamera(vrCamera);
    }

    public void SwitchHand()
    {
        isRightHanded = !isRightHanded;
        gun.isRightHanded = isRightHanded;
        Transform newParent = isRightHanded ? rightHandTransform : leftHandTransform;
        gunTransform.SetParent(newParent, false);
    }
}
