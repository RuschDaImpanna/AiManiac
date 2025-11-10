using UnityEngine;

public class CameraWeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilSpeed = 10f;
    [SerializeField] private float returnSpeed = 5f;
    
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Quaternion quaternionedCurrentRotation;

    void Update()
    {
        // Smoothly interpolate back to original position
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilSpeed * Time.deltaTime);
        quaternionedCurrentRotation = Quaternion.Euler(currentRotation);

        // Apply the rotation to the camera
        transform.localRotation = new Quaternion(quaternionedCurrentRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
    }

    public void ApplyRecoil()
    {
        // Add upward rotation (negative X axis rotates camera up)
        targetRotation += new Vector3(-recoilAmount, 0, 0);
    }
}
