using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] Transform camera;
    void Update()
    {
        camera.position = transform.position;
        transform.rotation = camera.rotation;
    }
}
