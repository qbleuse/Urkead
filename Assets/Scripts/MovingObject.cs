using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]                        private Vector3    start            = Vector3.zero;
    [SerializeField]                        private Vector3    end              = Vector3.zero;
    [SerializeField, Range(0.0f, 10.0f)]    private float      speed            = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]     private float      epsilon          = 0.000001f;
    [SerializeField]                        private bool       lerp             = false;
    [SerializeField]                        private bool       resetWhenLoad    = true;

    public  bool            isRunning   = false;
    private bool            isPaused    = false;
    private bool            runningLerp = false;
    private Vector3         initPos;
    private Coroutine       movingState;
    
    [HideInInspector] public    Vector3 direction     { get; private set; }
    [HideInInspector] public    Vector3 saveDirection   = Vector3.zero;
    [HideInInspector] public    Vector3 loadDirection   = Vector3.zero;
    [HideInInspector] public    Vector3 loadPos         = Vector3.zero;

    private void OnDestroy()
    {
        if (movingState != null)
            StopCoroutine(movingState);
    }

    public void Reset()
    {
        if (resetWhenLoad)
        {
            if (loadPos.sqrMagnitude != 0)
                transform.position = loadPos;
            else
                transform.position = initPos;

            if (movingState != null)
                StopCoroutine(movingState);
            movingState = null;

            loadDirection = saveDirection;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        runningLerp = lerp;
        initPos     = transform.position;

        if (isRunning)
        {
            if (movingState != null)
                StopCoroutine(movingState);
            movingState = null;

            if (lerp)
                movingState = StartCoroutine(LerpMovingCoroutine());
            else
                movingState = StartCoroutine(MovingCoroutine());
        }
    }

    public void ZoneEnter()
    {
        isRunning = true;

        if (movingState != null)
            StopCoroutine(movingState);
        movingState = null;

        if (lerp)
            movingState = StartCoroutine(LerpMovingCoroutine());
        else
            movingState = StartCoroutine(MovingCoroutine());
    }

    private IEnumerator MovingCoroutine()
    {
        while (true)
        {
            if (loadDirection.sqrMagnitude != 0)
            {
                direction = loadDirection;
                while (!(Vector3.Distance(transform.position - initPos, loadDirection) <= epsilon))
                {

                    transform.position += loadDirection.normalized * Time.deltaTime * speed;
                    yield return null;

                }
                loadDirection = Vector3.zero;
            }
            else if (!(Vector3.Distance(transform.position - initPos, end) <= epsilon))
            {
                direction = end;
                while (!(Vector3.Distance(transform.position - initPos, end) <= epsilon))
                {

                    transform.position += end.normalized * Time.deltaTime * speed;
                    yield return null;

                }
            }
            else if (!(Vector3.Distance(transform.position - initPos, start) <= epsilon))
            {
                direction = start;
                while (!(Vector3.Distance(transform.position - initPos, start) <= epsilon))
                {
                    transform.position += start.normalized * Time.deltaTime * speed;
                    yield return null;
                }
            }
            else
                yield return null;
        }
        
    }

    private IEnumerator LerpMovingCoroutine()
    {
        while (true)
        {
            if (loadDirection.sqrMagnitude != 0)
            {
                direction = loadDirection;
                while (!(Vector3.Distance(transform.position - initPos, loadDirection) <= epsilon))
                {
                    transform.position = Vector3.Lerp(transform.position, initPos + loadDirection, Time.deltaTime * speed);
                    yield return null;

                }
                loadDirection = Vector3.zero;
            }
            else if (!(Vector3.Distance(transform.position - initPos, end) <= epsilon))
            {
                direction = end;
                while (!(Vector3.Distance(transform.position - initPos, end) <= epsilon))
                {
                    transform.position = Vector3.Lerp(transform.position, initPos + end, Time.deltaTime * speed);
                    yield return null;
                }
            }
            else if (!(Vector3.Distance(transform.position - initPos, start) <= epsilon))
            {
                direction = start;
                while (!(Vector3.Distance(transform.position - initPos, start) <= epsilon))
                {
                    transform.position = Vector3.Lerp(transform.position, initPos + start, Time.deltaTime * speed);
                    yield return null;
                }
            }
            else
                yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (runningLerp != lerp)
        {
            if (isRunning)
            {
                if (movingState != null)
                    StopCoroutine(movingState);
                movingState = null;

                if (lerp)
                    movingState = StartCoroutine(LerpMovingCoroutine());
                else
                    movingState = StartCoroutine(MovingCoroutine());
            }
            runningLerp = lerp;
        }
        else if (PauseMenu.GameIsPaused && isRunning && !isPaused)
        {
            isPaused        = PauseMenu.GameIsPaused;
            if (movingState != null)
                StopCoroutine(movingState);
            movingState     = null;
        }
        else if (isRunning && !PauseMenu.GameIsPaused && isPaused)
        {
            isPaused        = PauseMenu.GameIsPaused;
            if (movingState != null)
                StopCoroutine(movingState);
            movingState = null;

            if (lerp)
                movingState = StartCoroutine(LerpMovingCoroutine());
            else
                movingState = StartCoroutine(MovingCoroutine());
        }
        else if (isRunning && movingState == null && !isPaused)
        {
            if (lerp)
                movingState = StartCoroutine(LerpMovingCoroutine());
            else
                movingState = StartCoroutine(MovingCoroutine());
        }
    }
}
