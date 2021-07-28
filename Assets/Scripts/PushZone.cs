using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PushInfo
{
    public Vector3 pushedDirection;
    public float pushedCoeff;
}

public class PushZone : MonoBehaviour
{
    [SerializeField]                private Vector3  pushedZoneDirection = Vector3.zero;
    [SerializeField,Range(0,60)]    private float    pushedZoneCoeff     = 0f;

    public PushInfo pushZoneInfo;

    // Start is called before the first frame update
    void Start()
    {
        pushZoneInfo.pushedDirection    = pushedZoneDirection;
        pushZoneInfo.pushedCoeff        = pushedZoneCoeff;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
