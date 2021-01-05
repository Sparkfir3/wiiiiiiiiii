using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Coroutine moving;

    public void MoveToPosition(Vector3 targetPos) {
        if(moving != null)
            StopCoroutine(moving);
        moving = StartCoroutine(MovingCoroutine(targetPos, 1f));
    }

    private IEnumerator MovingCoroutine(Vector3 targetPos, float moveTime) {
        Vector3 originalPos = transform.position;
        for(float i = 0; i < moveTime; i += Time.deltaTime) {
            transform.position = Vector3.Lerp(originalPos, targetPos, i / moveTime);
            yield return null;
        }
    }

}
