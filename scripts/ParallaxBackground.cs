using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxSpeed = 0.1f; 
    private Vector3 lastMousePosition;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        float parallaxX = mouseDelta.x * parallaxSpeed * Time.deltaTime;
        float parallaxY = mouseDelta.y * parallaxSpeed * Time.deltaTime;
        transform.position += new Vector3(parallaxX, parallaxY, 0);
        lastMousePosition = Input.mousePosition;
    }
}
