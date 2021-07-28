using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroneMovement : MonoBehaviour
{
    [SerializeField] private float      rightBorder    = 5;
    [SerializeField] private float      leftBorder     = -5;
    [SerializeField] private float      speed          = 1;
    [SerializeField] private float      height         = 3;
    [SerializeField] private float      lerpSpeed      = 1;
    [HideInInspector]public  bool       isUp           = false;
    [HideInInspector]public  Transform  initCheckpoint = null;
    [HideInInspector]public  bool       isMoving       = false;
    [HideInInspector]public  Vector3    initPos;
    [HideInInspector]public  Vector3    currentPos;
    [HideInInspector]public  Vector3    direction;

    private Transform   player;
    private Vector3     rightBorderVector;
    private Vector3     leftBorderVector;
    

    // Start is called before the first frame update
    void Start()
    {
        player              = GameObject.Find("Player").transform;
        direction           = Vector3.right;
        currentPos          = Vector3.zero;
        rightBorderVector   = Vector3.right * rightBorder;
        leftBorderVector    = Vector3.right * leftBorder;
        initCheckpoint      = transform.parent;
        initPos             = transform.position;
}

// Update is called once per frame
    void Update()
    {
        if (isUp)
        {
            if (!isMoving)
            {
                if (Mathf.Abs(player.position.x - transform.position.x) < rightBorder - 1)
                {
                    isMoving    = true;
                    currentPos += player.rotation * Vector3.forward * (rightBorder - 1);
                    direction   = player.rotation * Vector3.back;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused || WinScreen.gameIsWin || TrainingRead.Read)
            return;

        if (isMoving)
        {
            if (currentPos.x < leftBorderVector.x || currentPos.x > rightBorderVector.x)
            {
                direction *= -1;
            }

            currentPos += direction * speed * Time.fixedDeltaTime;

            Vector3 lerpY = player.position + Vector3.up * height;
            lerpY.y = Mathf.Lerp(transform.position.y, lerpY.y, lerpSpeed * Time.fixedDeltaTime);
            transform.position = lerpY + currentPos;
        }

        RaycastHit raycast;

        if (Physics.Raycast(transform.position, direction, out raycast, 0.1f))
        {
            Collider collider = raycast.collider;
            if (collider && !collider.CompareTag("Enemy") && !collider.CompareTag("Enemy Bullet") && !collider.CompareTag("Checkpoint") && !collider.CompareTag("Slow Zone"))
            {
                gameObject.SetActive(false);
                isMoving = false;
            }
        }

        else if (Physics.Raycast(transform.position, -transform.up, out raycast, 0.1f))
        {
            Collider collider = raycast.collider;
            if (collider && !collider.CompareTag("Enemy") && !collider.CompareTag("Enemy Bullet") && !collider.CompareTag("Checkpoint") && !collider.CompareTag("Slow Zone"))
            {
                gameObject.SetActive(false);
                isMoving = false;
            }
        }
    }
}
