using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolling : MonoBehaviour
{
    [SerializeField] Transform[] pathPoints;
    [SerializeField] int numPathPoints;
    Transform nextPoint;
    float radius = 1;

    Rigidbody rb;
    [SerializeField] float maxSpeed = 3f;
    Vector3 targetPos;
    Vector3 targetDir;
    [SerializeField] int force;

    [SerializeField] float accelerationRate;
    [SerializeField] float accelerationTimeToMax = 3f;
    [SerializeField] float currentSpeed;

    Vector3 steeringDir;
    Vector3 currentDir;
    [SerializeField] float steeringForce = 1f;

    [SerializeField] bool isFound;
    [SerializeField] Transform targetPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        accelerationRate = maxSpeed / accelerationTimeToMax;

        nextPoint = pathPoints[0];
    }

    void Update()
    {
        if (isFound == false)
        {
            if (Vector3.Distance(pathPoints[numPathPoints].transform.position, transform.position) < radius)
            {
                numPathPoints++;

                if (numPathPoints >= pathPoints.Length)
                {
                    numPathPoints = 0;
                }

                nextPoint = pathPoints[numPathPoints];
            }
            else
            {
                targetPos = nextPoint.position;
                targetDir = (targetPos - transform.position).normalized;
                targetDir.y = 0;

                currentDir = Vector3.RotateTowards(transform.forward, targetDir, 3 * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(currentDir);

                if (currentSpeed < maxSpeed)
                {
                    currentSpeed = currentSpeed + accelerationRate * Time.deltaTime;
                }
                else
                {
                    currentSpeed = maxSpeed;
                }

                steeringDir = targetDir - currentDir;
                rb.velocity = (currentSpeed * currentDir) + (steeringDir * steeringForce);
            }
        }
        else
        {
            targetPos = targetPlayer.position;
            targetDir = (targetPos - transform.position).normalized;
            targetDir.y = 0;

            currentDir = Vector3.RotateTowards(transform.forward, targetDir, 3 * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(currentDir);

            if (currentSpeed < maxSpeed)
            {
                currentSpeed = currentSpeed + accelerationRate * Time.deltaTime;
            }
            else
            {
                currentSpeed = maxSpeed;
            }
            steeringDir = targetDir - currentDir;
            rb.velocity = (currentSpeed * currentDir) + (steeringDir * steeringForce);
        }

        Debug.DrawRay(transform.position, transform.forward * 3, Color.green);
        Debug.DrawRay(transform.position, targetDir * 3, Color.red);
        Debug.DrawRay(transform.position, steeringDir * 3, Color.yellow);
        //Debug.Log(rb.velocity.magnitude);
    }
}
