using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private float targetPositionY;
    private Vector3 targetRotation;

    public float scrollEdgeProp = 0.1f;

    public float panSpeed = 10f;
    public float scrollSpeed = 15f;
    public float scrollSpeedMult = 2f;

    public Vector2 zoomRange = new Vector2(-10f, 10f);
    private float currentZoom = 0f;
    public float zoomSpeed = 100f;
    public float zoomXMultiplier = 1f;
    public float updateZoomRotationSpeed = 5f;
    public float updatePositionSpeed = 5f;

    private bool onFocus = true;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
        targetPositionY = transform.position.y;
        targetRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float currentScrollSpeed = scrollSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentScrollSpeed = currentScrollSpeed * scrollSpeedMult;
        }

        if (!Application.isEditor && onFocus)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height * (1 - scrollEdgeProp))
            {
                transform.Translate(Vector3.forward * currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= Screen.height * scrollEdgeProp)
            {
                transform.Translate(Vector3.forward * -currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width * (1 - scrollEdgeProp))
            {
                transform.Translate(Vector3.right * currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= Screen.width * scrollEdgeProp)
            {
                transform.Translate(Vector3.right * -currentScrollSpeed * Time.deltaTime, Space.World);
            }
        }
        else // control in the editor
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.forward * currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector3.forward * -currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * currentScrollSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.right * -currentScrollSpeed * Time.deltaTime, Space.World);
            }
        }

        // zoom in-out
        float zoomToApply = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;
        currentZoom -= zoomToApply;
        currentZoom = Mathf.Clamp(currentZoom, zoomRange.x, zoomRange.y);

        // Y position update
        targetPositionY = transform.position.y - (transform.position.y - (initialPosition.y + currentZoom));

        transform.position = new Vector3(
            transform.position.x,
            Mathf.Lerp(transform.position.y, targetPositionY, updatePositionSpeed * Time.deltaTime),
            transform.position.z
        );

        // X rotation update
        float zoomXRotation = transform.eulerAngles.x - (transform.eulerAngles.x - (initialRotation.x + currentZoom * zoomXMultiplier));
        zoomXRotation = Mathf.Clamp(zoomXRotation, 0f, 70f);
        targetRotation = new Vector3(
            zoomXRotation,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, updateZoomRotationSpeed * Time.deltaTime);
    }
    
    private void OnApplicationFocus(bool focus)
    {
        onFocus = focus;
    }
}
