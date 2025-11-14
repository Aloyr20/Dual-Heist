using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 30f;
    public GameObject gunBullets;
    public int maxAmmo = 100;
    public float autoReloadDelay = 1f;
    public float fireCooldown = 1f; // 1 second cooldown between shots
    public Vector3 rotationOffsetBullet = Vector3.zero;

    [Header("Laser Settings")]
    public LineRenderer laserLine;
    public float laserDistance = 50f;
    public Color activeLaserColor = Color.red;
    public Color reloadingLaserColor = Color.yellow;
    public float laserWidth = 0.005f;

    [Header("Input Actions")]
    public InputActionProperty fireAction;

    [Header("Controller Attachment")]
    public Transform rightHandController;

    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float cooldownTimer = 0f;
    private bool canShoot = true;

    void Start()
    {
        // Attach to controller
        if (rightHandController != null)
        {
            transform.SetParent(rightHandController);
        }

        // Set up input actions
        if (fireAction.action != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += OnFire;
        }

        // Initialize ammo
        currentAmmo = maxAmmo;

        // Set up laser
        InitializeLaser();
    }

    void InitializeLaser()
    {
        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.useWorldSpace = true;
            laserLine.startWidth = laserWidth;
            laserLine.endWidth = laserWidth;
            laserLine.startColor = activeLaserColor;
            laserLine.endColor = activeLaserColor;
            laserLine.enabled = true;
        }
    }

    void OnDestroy()
    {
        if (fireAction.action != null)
            fireAction.action.performed -= OnFire;
    }

    void Update()
    {
        UpdateLaser();
        UpdateCooldown();

        // Handle auto-reload
        if (currentAmmo <= 0 && !isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= autoReloadDelay)
            {
                ReloadGun();
            }
        }
    }

    void UpdateCooldown()
    {
        if (!canShoot)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= fireCooldown)
            {
                canShoot = true;
                cooldownTimer = 0f;
            }
        }
    }

    void OnFire(InputAction.CallbackContext context)
    {
        // Only shoot if trigger is pressed, cooldown is over, and gun is ready
        if (context.ReadValue<float>() > 0.5f && canShoot && !isReloading && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Start cooldown
        canShoot = false;
        cooldownTimer = 0f;

        currentAmmo--;

        if (gunBullets != null && bulletSpawnPoint != null)
        {
            GameObject bulletsSpawned = Instantiate(gunBullets, bulletSpawnPoint.position, bulletSpawnPoint.rotation * Quaternion.Euler(rotationOffsetBullet));

            // Set position and rotation
            bulletsSpawned.transform.position = bulletSpawnPoint.position;
            bulletsSpawned.transform.rotation = bulletSpawnPoint.rotation;

            Rigidbody bulletRb = bulletsSpawned.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
                bulletRb.angularVelocity = Vector3.zero;
                bulletRb.freezeRotation = true;
            }

            Destroy(bulletsSpawned, 5);
            Debug.Log($"Shot fired! Ammo left: {currentAmmo}");
        }

        if (currentAmmo <= 0)
        {
            isReloading = true;
            reloadTimer = 0f;
        }
    }

    void ReloadGun()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        reloadTimer = 0f;
        Debug.Log("Gun reloaded! Ammo: " + currentAmmo);
    }

    void UpdateLaser()
    {
        if (!laserLine || !laserLine.enabled) return;

        // Visual feedback for cooldown - blink laser when cooling down
        if (!canShoot)
        {
            float blink = Mathf.PingPong(Time.time * 10f, 1f);
            laserLine.startColor = Color.Lerp(activeLaserColor, Color.gray, blink);
            laserLine.endColor = Color.Lerp(activeLaserColor, Color.gray, blink);
        }
        else if (isReloading)
        {
            laserLine.startColor = reloadingLaserColor;
            laserLine.endColor = reloadingLaserColor;
        }
        else
        {
            laserLine.startColor = activeLaserColor;
            laserLine.endColor = activeLaserColor;
        }

        if (bulletSpawnPoint != null)
        {
            laserLine.SetPosition(0, bulletSpawnPoint.position);

            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnPoint.position, bulletSpawnPoint.forward, out hit, laserDistance))
            {
                laserLine.SetPosition(1, hit.point);
            }
            else
            {
                laserLine.SetPosition(1, bulletSpawnPoint.position + bulletSpawnPoint.forward * laserDistance);
            }
        }
    }

    // Optional: Method to manually reload if needed
    public void ManualReload()
    {
        if (isReloading) return;
        ReloadGun();
    }

    // Optional: Method to check current ammo
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    // Optional: Method to add ammo
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }

    // Optional: Get cooldown progress (0 to 1)
    public float GetCooldownProgress()
    {
        return canShoot ? 1f : (cooldownTimer / fireCooldown);
    }
}