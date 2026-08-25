using UnityEngine;
using System.Collections;

public class GunVFXController : MonoBehaviour
{
    [Header("Tracer")]
    [SerializeField] private LineRenderer tracer;
    [SerializeField] private float pointsPerUnit = 8f;
    [SerializeField] private float noiseScale = 2f;
    [SerializeField] private float noisePower = 0.08f;
    [SerializeField] private float dissolveDuration = 1f;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject bystanderHitVFX;

    [Header("Muzzle Flash")]
    [SerializeField] private ParticleSystem muzzleFlash;

    private Material tracerMaterial;
    private Coroutine dissolveCoroutine;

    private static readonly int PathDissolveProperty =
        Shader.PropertyToID("_Path_dissolve");

    private static readonly int DissolveProperty =
        Shader.PropertyToID("_Dissolve");

    private void Awake()
    {
        if (tracer == null)
            return;

        tracer.useWorldSpace = true;

        // Crée une instance du matériau du LineRenderer.
        tracerMaterial = tracer.material;

        tracer.positionCount = 0;
        tracer.gameObject.SetActive(false);
    }

    public void PlayMuzzleFlash()
    {
        if (muzzleFlash == null)
            return;

        muzzleFlash.Play(true);
    }

    public void PlayHitVFX(RaycastHit hit, bool isBystander)
    {
        if (hitVFX == null)
            return;

        Quaternion rotation =
            Quaternion.LookRotation(hit.normal);

        GameObject effect = Instantiate(
            isBystander ? bystanderHitVFX : hitVFX,
            hit.point + hit.normal * 0.01f,
            rotation
        );

        ParticleSystem particleSystem =
            effect.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            particleSystem.Play(true);
        }

        Destroy(effect, 0.3f);
    }

    public void PlayTracer(Vector3 startPoint, Vector3 endPoint)
    {
        if (tracer == null || tracerMaterial == null)
            return;

        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        float distance = Vector3.Distance(startPoint, endPoint);

        int pointCount = Mathf.Max(
            2,
            Mathf.CeilToInt(distance * pointsPerUnit) + 1
        );

        Vector3 direction =
            (endPoint - startPoint).normalized;

        Vector3 side =
            Vector3.Cross(direction, Vector3.up);

        if (side.sqrMagnitude < 0.001f)
            side = Vector3.Cross(direction, Vector3.right);

        side.Normalize();

        Vector3 secondSide =
            Vector3.Cross(direction, side).normalized;

        float seed = Random.Range(0f, 10000f);

        tracer.positionCount = pointCount;

        for (int index = 0; index < pointCount; index++)
        {
            float normalizedPosition =
                index / (float)(pointCount - 1);

            Vector3 point =
                Vector3.Lerp(
                    startPoint,
                    endPoint,
                    normalizedPosition
                );

            Vector3 noise =
                GetSmoothNoise3D(
                    seed,
                    normalizedPosition
                );

            // Garde les extrémités attachées au canon et au point d'impact.
            float taper =
                Mathf.Sin(normalizedPosition * Mathf.PI);

            Vector3 distortion =
                (side * noise.x + secondSide * noise.y) *
                noisePower *
                taper;

            tracer.SetPosition(
                index,
                point + distortion
            );
        }

        tracerMaterial.SetFloat(
            PathDissolveProperty,
            0f
        );

        tracerMaterial.SetFloat(
            DissolveProperty,
            0f
        );

        tracer.gameObject.SetActive(true);

        dissolveCoroutine =
            StartCoroutine(DissolveTracer());
    }

    private Vector3 GetSmoothNoise3D(
        float seed,
        float normalizedPosition
    )
    {
        float position =
            normalizedPosition * noiseScale;

        Vector3 noisePosition = new Vector3(
            seed + position,
            seed * 0.73f + position,
            seed * 1.37f + position
        );

        float noiseX = (
            Mathf.PerlinNoise(
                noisePosition.x,
                noisePosition.y
            ) +
            Mathf.PerlinNoise(
                noisePosition.y,
                noisePosition.z
            )
        ) * 0.5f;

        float noiseY = (
            Mathf.PerlinNoise(
                noisePosition.y,
                noisePosition.z
            ) +
            Mathf.PerlinNoise(
                noisePosition.x,
                noisePosition.z
            )
        ) * 0.5f;

        float noiseZ = (
            Mathf.PerlinNoise(
                noisePosition.x,
                noisePosition.z
            ) +
            Mathf.PerlinNoise(
                noisePosition.y,
                noisePosition.x
            )
        ) * 0.5f;

        return new Vector3(
            noiseX * 2f - 1f,
            noiseY * 2f - 1f,
            noiseZ * 2f - 1f
        );
    }

    private IEnumerator DissolveTracer()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, dissolveDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float dissolve =
                Mathf.Clamp01(elapsed / duration);

            tracerMaterial.SetFloat(
                PathDissolveProperty,
                dissolve
            );

            tracerMaterial.SetFloat(
                DissolveProperty,
                dissolve
            );

            yield return null;
        }

        tracerMaterial.SetFloat(
            PathDissolveProperty,
            1f
        );

        tracerMaterial.SetFloat(
            DissolveProperty,
            1f
        );

        tracer.positionCount = 0;
        tracer.gameObject.SetActive(false);
        dissolveCoroutine = null;
    }
}