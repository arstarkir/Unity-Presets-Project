using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;

    [SerializeField] enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    [SerializeField] RotationAxes axes = RotationAxes.MouseXAndY;
    [SerializeField] float sensitivityX = 5f;
    [SerializeField] float sensitivityY = 5f;

    [SerializeField] float minimumX = -360;
    [SerializeField] float maximumX = 360;

    [SerializeField] float minimumY = -90;
    [SerializeField] float maximumY = 90;

    float rotationX = 0F;
    float rotationY = 0F;

    Quaternion originalRotation;
    void Update()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }
    void Start()
    {
        Cursor.lockState = cursorLockMode;
        originalRotation = transform.localRotation;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}