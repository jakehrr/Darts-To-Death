using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartHandler : MonoBehaviour
{
    public Transform dartThrowPos;
    public float duration = .35f;
    public float arcHeight = 2.0f;

    private void Update()
    {
        this.gameObject.transform.LookAt(dartThrowPos);
    }

    public void SlerpDartToPos()
    {
        StartCoroutine(SlerpDartCoroutine());
    }

    private IEnumerator SlerpDartCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = dartThrowPos.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            float height = arcHeight * 4 * t * (1 - t);
            currentPos.y += height;

            transform.position = currentPos;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}

