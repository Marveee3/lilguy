using System.Collections;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    Vector3 originalPosition;
    Coroutine shakeRoutine;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeRoutine = null;
    }

    public void StartShake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            transform.localPosition = originalPosition;
        }

        shakeRoutine = StartCoroutine(Shake(duration, magnitude));
    }
}
