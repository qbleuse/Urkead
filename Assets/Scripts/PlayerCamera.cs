using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]                            private Transform    camTarget      = null;
    [SerializeField, Range( 0.0f, 100.0f)]      private float        trackingSpeed  = 0f;
    [SerializeField, Range(-1000.0f, 1000.0f)]  private float        minX           = 0f;
    [SerializeField, Range(-1000.0f, 1000.0f)]  private float        minY           = 0f;
    [SerializeField, Range(-1000.0f, 1000.0f)]  private float        maxX           = 0f;
    [SerializeField, Range(-1000.0f, 1000.0f)]  private float        maxY           = 0f;
    [SerializeField]                            private Vector3      relativePos    = Vector3.zero;
    [SerializeField]                            private bool         rail           = false;
    [SerializeField]                            private bool         lookAt         = true;
    [SerializeField]                            private Vector3      localRotation  = Vector3.zero;

    [HideInInspector] public    Camera      cam { get; private set; }
                      private   Coroutine   shakeCoroutine = null;

    /*=================== COROUTINE =====================*/
    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed <= duration)
        {
            if (!PauseMenu.GameIsPaused && !WinScreen.gameIsWin)
            {
                transform.localPosition = transform.localPosition + Random.onUnitSphere * magnitude;

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        if (elapsed > duration)
        {
            StopCoroutine(shakeCoroutine);
        }
    }

    public void MakeShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake(duration,magnitude));
    }


    /*===================== EVENTS =======================*/

    public void CamChangeInfo(Checkpoint newZone)
    {
        relativePos     = newZone.camPosition;

        localRotation   = newZone.camRotation;
        rail            = newZone.onRail;
        lookAt          = newZone.lookAtCondition;

        /* to stay on relative position, we keep the good value you have to go on y coordinates*/
        if (rail)
        {
            relativePos.y       += camTarget.position.y;
        }
    }

    /*==================== UNITY METHODS =======================*/
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        shakeCoroutine = null;
    }

    private void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin)
            return;

        /* Lerp to corresponding position and rotation */
        if (camTarget != null)
        {

                if (!rail)
                {
                    Vector3 toPos   = camTarget.position + (camTarget.rotation * relativePos);
                    Vector3 curPos  = Vector3.Lerp(transform.position, toPos, trackingSpeed * Time.fixedDeltaTime);
                    curPos          = new Vector3(Mathf.Clamp(curPos.x, minX, maxX), Mathf.Clamp(curPos.y, minY, maxY), Mathf.Clamp(curPos.z, minX, maxX));

                    transform.position = curPos;
                }
                else
                {
                    Vector3 rotatePos = camTarget.rotation * relativePos;
                    Vector3 toPos     = new Vector3(rotatePos.x + camTarget.position.x, rotatePos.y, rotatePos.z + camTarget.position.z);
                    Vector3 curPos    = Vector3.Lerp(transform.position, toPos, trackingSpeed * Time.fixedDeltaTime);
                    curPos            = new Vector3(Mathf.Clamp(curPos.x, minX, maxX), Mathf.Clamp(curPos.y, minY, maxY), Mathf.Clamp(curPos.z, minX, maxX));

                    transform.position = curPos;
                }

                if (lookAt)
                {
                    Quaternion toRot    = Quaternion.Euler(localRotation) * Quaternion.LookRotation(camTarget.position - transform.position, camTarget.up);
                    Quaternion curRot   = Quaternion.Slerp(transform.rotation, toRot, trackingSpeed * Time.fixedDeltaTime);
                    transform.rotation  = curRot;
                }

        }
       
    }
}
