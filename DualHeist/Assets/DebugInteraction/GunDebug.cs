using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunDebug : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public GameObject gunBullets;
    public int maxAmmo = 10;
    public LineRenderer laserLine;
    public float laserDistance = 50f;
    public InputActionProperty triggerAction;

    private int currentAmmo;
    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;
    private bool canShoot = true;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        triggerAction.action.Enable();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        currentAmmo = maxAmmo;

        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.useWorldSpace = true;
        }
    }

    void Update()
    {
        UpdateLaser();

        if (isHeld && triggerAction.action.ReadValue<float>() > 0.1f && canShoot)
        {
            FireGun();
            canShoot = false;
        }

        if (triggerAction.action.ReadValue<float>() < 0.1f)
        {
            canShoot = true;
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    void FireGun()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        if (gunBullets != null && bulletSpawnPoint != null)
        {
            GameObject bulletsSpawned = Instantiate(gunBullets);
            bulletsSpawned.transform.position = bulletSpawnPoint.position;
            bulletsSpawned.transform.rotation = bulletSpawnPoint.rotation;

            Rigidbody bulletRb = bulletsSpawned.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            Destroy(bulletsSpawned, 5);
        }
    }

    void UpdateLaser()
    {
        if (!laserLine || !bulletSpawnPoint) return;

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