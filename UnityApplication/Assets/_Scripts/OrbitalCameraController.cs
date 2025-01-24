using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitalCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float orbitSpeed = 5f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationDamping = 0.1f;
    [SerializeField] private float initialYRotation = -45f; 

    [Header("Input Actions")]
    [Tooltip("Provides the mouse delta (change in mouse position). Used for both Orbit and Mouse Look.")]
    [SerializeField] private InputActionReference orbitActionReference;

    [Tooltip("Provides the mouse scroll wheel value. Used for zooming.")]
    [SerializeField] private InputActionReference zoomActionReference;

    [Tooltip("Detects if the right mouse button is held down. Acts as a modifier for Orbit/Mouse Look.")]
    [SerializeField] private InputActionReference lookActionReference;

    private Vector2 orbitInput;
    private float zoomInput; 
    private float currentDistance;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Quaternion currentRotation;
    private bool isLooking;

    private void Awake()
    {
        currentDistance = (minDistance + maxDistance) / 2f;
        yRotation = initialYRotation;
        currentRotation = transform.rotation;
    }

    private void OnEnable()
    {
        orbitActionReference.action.Enable();
        zoomActionReference.action.Enable();
        lookActionReference.action.Enable();
    }

    private void LateUpdate()
    {
        if (!target) return;

        isLooking = lookActionReference.action.ReadValue<float>() > 0;

        zoomActionReference.action.ReadValue<float>();

        if (zoomInput != 0)
        {
            currentDistance -= zoomInput * zoomSpeed * Time.deltaTime;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        if (isLooking)
        {
            orbitInput = orbitActionReference.action.ReadValue<Vector2>();

            xRotation += orbitInput.x * orbitSpeed * Time.deltaTime;
            yRotation -= orbitInput.y * orbitSpeed * Time.deltaTime;
            yRotation = Mathf.Clamp(yRotation, -89f, 89f);

            Quaternion targetRotation = Quaternion.Euler(yRotation, xRotation, 0);
            currentRotation = Quaternion.Lerp(currentRotation, targetRotation, rotationDamping);
        }

        transform.rotation = currentRotation;
        transform.position = target.position - transform.forward * currentDistance;
    }

    private void OnDisable()
    {
        orbitActionReference.action.Disable();
        zoomActionReference.action.Disable();
        lookActionReference.action.Disable();
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.transform;
        target.SetActive(true);
    }
}