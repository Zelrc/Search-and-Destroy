using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringGeneral : MonoBehaviour
{
    public Rigidbody rb;
    public float rotSpeed = 1.5f;
    public float deaccelThres = 5f;
    public float accelTime = 0.1f;
    public float maxAccel = 20f;


    Queue<Vector3> veloQ = new Queue<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LookAtDirection(Vector3 direction)
    {
        direction.Normalize();

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = -1 * (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90);
            float rotation = Mathf.LerpAngle(rb.rotation.eulerAngles.y, angle, Time.deltaTime * rotSpeed);

            rb.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    public void LookWhereYoureGoing()
    {
        Vector3 direction = rb.velocity.normalized;
        
        if(veloQ.Count == 5)
        {
            veloQ.Dequeue();
        }

        veloQ.Enqueue(rb.velocity.normalized);

        direction = Vector3.zero;

        foreach(Vector3 velo in veloQ)
        {
            direction += velo;
        }

        direction /= veloQ.Count;

        LookAtDirection(direction);
    }

    public Vector3 Seek(Vector3 targetPosition, float maxSeekAccel)
    {

        Vector3 acceleration = (targetPosition - transform.position);

        acceleration.Normalize();

        acceleration *= maxSeekAccel;

        return acceleration;
    }

    public Vector3 Arrive(Vector3 targetPosition)
    {
        Debug.DrawLine(transform.position, targetPosition, Color.cyan, 0f, false);

        Vector3 targetVelocity = targetPosition - transform.position;

        float dist = targetVelocity.magnitude;

        if (dist < 0.1f)
        {
            rb.velocity = Vector3.zero;
            Debug.Log("CHange?");
            return Vector3.zero;
        }

        float targetSpeed;
        if (dist > deaccelThres)
        {
            targetSpeed = 10f;
        }
        else
        {
            targetSpeed = 10f * (dist / deaccelThres);
        }


        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;


        Vector3 acceleration = targetVelocity - rb.velocity;

        acceleration *= 1 / accelTime;

        if (acceleration.magnitude > maxAccel)
        {
            acceleration.Normalize();
            acceleration *= maxAccel;
        }

        return acceleration;
    }

    public void LinearMove(Vector3 linearAcceleration)
    {
        rb.velocity += linearAcceleration * Time.deltaTime;

        if (rb.velocity.magnitude > 10.0f)
        {
            rb.velocity = rb.velocity.normalized * 10.0f;
        }
    }
}
