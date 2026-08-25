using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float reloadDuration = 0.5f;
    [SerializeField] private float reloadStartDelay = 0.2f;
    [SerializeField] private float maxAimDistance = 100f;
    [SerializeField] private LayerMask aimLayers;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GunVFXController gunVFXController;

    private bool isReloading;
    private GameObject bullet;
    private Rigidbody bulletRigidbody;
    private Animator animator;
    private Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        if (gunVFXController == null)
            gunVFXController = GetComponent<GunVFXController>();

        bullet = Instantiate(bulletPrefab);
        bullet.SetActive(false);

        bulletRigidbody =
            bullet.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    private Vector3 GetAimPoint(out RaycastHit hit)
    {
        Ray aimRay = mainCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 aimPoint =
            aimRay.origin + aimRay.direction * maxAimDistance;

        bool hitSomething = Physics.Raycast(
            aimRay,
            out hit,
            maxAimDistance,
            aimLayers,
            QueryTriggerInteraction.Collide
        );

        if (hitSomething)
        {
            aimPoint = hit.point;
        }

        return aimPoint;
    }

    private void Shoot()
    {
        if (isReloading)
            return;

        animator.SetTrigger("Shoot");

        Vector3 aimPoint =
            GetAimPoint(out RaycastHit hit);

        Vector3 shootDirection =
            (aimPoint - firePoint.position).normalized;

        if (gunVFXController != null)
        {
            gunVFXController.PlayMuzzleFlash();

            gunVFXController.PlayTracer(
                firePoint.position,
                aimPoint
            );

            if (hit.collider != null)
            {
                BystanderScript bystander = hit.collider.GetComponentInParent<BystanderScript>();
                bool isBystander = bystander != null;

                gunVFXController.PlayHitVFX(hit, isBystander);

                if (isBystander)
                {
                    Vector3 forceDirection = shootDirection;
                    forceDirection.y = 1f;
                    bystander.TriggerRagdoll(forceDirection * bulletSpeed, hit.point);
                }
            }
        }

        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(
            reloadStartDelay
        );

        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(
            reloadDuration - reloadStartDelay
        );

        isReloading = false;
    }
}