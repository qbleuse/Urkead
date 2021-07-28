using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform)),RequireComponent(typeof(ParticleSystem))]
public class AnimParticleUI : MonoBehaviour
{
    [SerializeField]    private RectTransform   UIpos       = null;
                        private ParticleSystem  particle    = null;
    
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
