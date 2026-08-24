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

    private bool isReloading;
    private GameObject bullet;
    private Rigidbody bulletRigidbody;
    private Animator animator;
    private Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        bullet = Instantiate(bulletPrefab);
        bullet.SetActive(false);

        bulletRigidbody = bullet.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (isReloading || bullet.activeSelf)
            return;

        animator.SetTrigger("Shoot");

        Ray aimRay = mainCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 aimPoint =
            aimRay.origin + aimRay.direction * maxAimDistance;

        bool hitSomething = Physics.Raycast(
            aimRay,
            out RaycastHit hit,
            maxAimDistance,
            aimLayers,
            QueryTriggerInteraction.Collide
        );

        if (hitSomething)
        {
            aimPoint = hit.point;

            Debug.DrawLine(
                aimRay.origin,
                hit.point,
                Color.red,
                reloadDuration
            );

            Debug.Log("Raycast touche : " + hit.collider.name);
        }
        else
        {
            Debug.DrawRay(
                aimRay.origin,
                aimRay.direction * maxAimDistance,
                Color.yellow,
                reloadDuration
            );
        }

        Vector3 shootDirection =
            (aimPoint - firePoint.position).normalized;

        Quaternion rotation =
            Quaternion.LookRotation(shootDirection) *
            Quaternion.Euler(90f, 0f, 180f);

        bullet.SetActive(true);

        bullet.transform.SetPositionAndRotation(
            firePoint.position,
            rotation
        );

        bulletRigidbody.linearVelocity =
            shootDirection * bulletSpeed;

        bulletRigidbody.angularVelocity = Vector3.zero;

        bullet.GetComponent<PooledBullet>().Launch(this);

        StartCoroutine(Reload());
    }

    public void ReturnBulletToPool()
    {
        bulletRigidbody.linearVelocity = Vector3.zero;
        bulletRigidbody.angularVelocity = Vector3.zero;
        bullet.SetActive(false);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadStartDelay);

        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(
            reloadDuration - reloadStartDelay
        );

        isReloading = false;
    }
}