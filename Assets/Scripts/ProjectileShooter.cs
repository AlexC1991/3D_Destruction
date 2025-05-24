using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;
    public float projectileMass = 5f;
    public float fireRate = 0.25f;
    public bool autoFire = false;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public bool spawnAtCamera = true;
    public float spawnDistance = 1f;
    
    [Header("Forces")]
    public float impactForce = 2000f;
    public bool applyTorque = true;
    public float torqueAmount = 10f;
    
    [Header("Mouse Controls")]
    public float mouseSensitivity = 2.0f;
    public bool invertY = false;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    public bool lockCursor = true;
    
    private float nextFireTime;
    private Camera mainCamera;
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        if (spawnPoint == null && !spawnAtCamera)
        {
            spawnPoint = transform;
        }
        
        nextFireTime = 0f;
        
        // Initialize rotation to match current orientation
        if (spawnAtCamera && mainCamera != null)
        {
            Vector3 angles = mainCamera.transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
        }
        
        // Lock and hide cursor if enabled
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void Update()
    {
        // Handle mouse rotation
        HandleMouseRotation();
        
        // Fire on left mouse button or continuously if autoFire is enabled
        if ((Input.GetMouseButton(0) || autoFire) && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
        
        // Toggle auto-fire with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            autoFire = !autoFire;
            Debug.Log("Auto-fire: " + (autoFire ? "ON" : "OFF"));
        }
        
        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }
    
    void HandleMouseRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);
        
        // Apply rotation
        rotationX += mouseX;
        rotationY = Mathf.Clamp(rotationY + mouseY, minVerticalAngle, maxVerticalAngle);
        
        // Apply rotation to camera if using camera spawning
        if (spawnAtCamera && mainCamera != null)
        {
            mainCamera.transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);
        }
        // Otherwise apply to this object or spawn point
        else
        {
            if (spawnPoint != null)
            {
                spawnPoint.rotation = Quaternion.Euler(rotationY, rotationX, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
            }
        }
    }
    
    void FireProjectile()
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        
        if (spawnAtCamera)
        {
            // Spawn from camera position and orientation
            spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;
            spawnRotation = mainCamera.transform.rotation;
        }
        else
        {
            // Spawn from the specified spawn point
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        
        // Create the projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        
        // Add a rigidbody if it doesn't have one
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody>();
        }
        
        // Configure rigidbody
        rb.mass = projectileMass;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Add forward force
        rb.AddForce(projectile.transform.forward * impactForce, ForceMode.Impulse);
        
        // Add random rotation if enabled
        if (applyTorque)
        {
            rb.AddTorque(
                Random.Range(-torqueAmount, torqueAmount),
                Random.Range(-torqueAmount, torqueAmount),
                Random.Range(-torqueAmount, torqueAmount),
                ForceMode.Impulse
            );
        }
        
        // Destroy projectile after 10 seconds to avoid cluttering the scene
        Destroy(projectile, 10f);
    }
    
    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void OnDestroy()
    {
        // Ensure cursor is visible when script is destroyed
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
