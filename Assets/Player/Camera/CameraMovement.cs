using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private InputController mouseScript;

    private void Awake()
    {

        mouseScript = GetComponentInParent<InputController>();
    }

    public float mouseSensitivity;

    float xRotation = 0f;

    private void FixedUpdate()
    {

        Vector2 mouseMove = mouseScript.GetMouseMove();

        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = mouseMove.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseMove.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);

    }

}
