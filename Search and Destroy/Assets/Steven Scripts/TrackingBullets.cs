using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBullets : SteeringGeneral
{
    Transform target = null;
    public Vector3 destination;
    public Transform shootPos;
    public LayerMask enemy;

    float timer = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;
        timer--;
        Collider[] objs;

        objs = Physics.OverlapSphere(transform.position, 20f);
        foreach (Collider x in objs)
        {
            
            if(target == null)
            {
                if (x.gameObject.tag == "enemy")
                {
                    target = x.transform;
                    Debug.Log("enemy");
                }
            }
        }

        if(target != null && timer <= 0)
        {
            LinearMove(Seek(target.position, 30f));
        }
        else
        {
            rb.velocity = (destination - shootPos.position).normalized * 10f;
        }

        



        LookWhereYoureGoing();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 10f);
    }
}
