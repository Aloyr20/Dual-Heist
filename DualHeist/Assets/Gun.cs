using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public float bulletSpeed;
    public GameObject gunBullets;
    public int maxAmmo = 10;
    public LineRenderer laserLine;
    public float laserDistance = 50f;

    private int currentAmmo;
    private XRGrabInteractable canGrab;

    void Start()
    {
        canGrab = GetComponent<XRGrabInteractable>();
        canGrab.activated.AddListener(FireGun);
        canGrab.deactivated.AddListener(ReloadGun);
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        UpdateLaser();
    }

    public void FireGun(ActivateEventArgs args)
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        GameObject bulletsSpawned = Instantiate(gunBullets);
        bulletsSpawned.transform.position = bulletSpawnPoint.position;
        bulletsSpawned.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        Destroy(bulletsSpawned, 5);
    }

    public void ReloadGun(DeactivateEventArgs args)
    {
        currentAmmo = maxAmmo;
    }

    void UpdateLaser()
    {
        if (!laserLine) return;
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

