using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField] private Material original;
    [SerializeField] private Material detected;

    

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = original;
    }

    // Update is called once per frame
    void Update()
    {
        if(Turret.inRange)
        {
            rend.sharedMaterial = detected;
        }
        else
        {
            rend.sharedMaterial = original;
        }
    }
}
