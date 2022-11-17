using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekSteering : SteeringGeneral
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LinearMove(Seek(target.position, 20f));
        LookWhereYoureGoing();
        
    }
}
