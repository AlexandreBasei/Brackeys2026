using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 5f;

    private GunController gunController;
    private float lifetime;

    public void Launch(GunController owner)
    {
        gunController = owner;
        lifetime = 0f;
    }

    private void Update()
    {
        lifetime += Time.deltaTime;

        if (lifetime >= maxLifetime)
            ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (gunController != null)
            gunController.ReturnBulletToPool();
    }
}