using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolling : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] int enemyHealth;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int getDamage = 10;

    [Space(25)]
    [Header("MOVEMENT")]
    Rigidbody rb;
    [SerializeField] float maxSpeed = 5f;
    Vector3 targetPos;
    Vector3 targetDir;
    [SerializeField] int force;

    [SerializeField] float accelerationRate;
    [SerializeField] float accelerationTimeToMax = 3f;
    [SerializeField] float currentSpeed;

    Vector3 steeringDir;
    Vector3 currentDir;
    [SerializeField] float steeringForce = 1f;

    [Space(25)]
    [Header("CHASING")]
    [SerializeField] Transform targetPlayer;
    float targetDist;
    public float sizeRadius;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] bool isFound;

    [Space(25)]
    [Header("WAYPOINTS")]
    [SerializeField] Transform targetPath;
    [SerializeField] Transform[] pathPoints;
    [SerializeField] int numPathPoints;

    [Space(25)]
    [Header("AVOIDANCE")]
    [SerializeField] Transform targetObstacles = null;
    public bool isObstacle = false;
    [SerializeField] List<GameObject> obstacles = new List<GameObject>();

    [Space(25)]
    [Header("DAMAGE PLAYER")]
    float timer = 1;
    PlayerHealth playerHealth;

    [Space(25)]
    [Header("TRAP AREA")]
    public bool isTrap;
    [SerializeField] Transform fleeSpot;
    public float sizeRadiusInTrap;

    Animator animator;     

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        accelerationRate = maxSpeed / accelerationTimeToMax;

        enemyHealth = maxHealth;

        playerHealth = targetPlayer.GetComponent<PlayerHealth>();

        //fleeSpot = GameObject.FindGameObjectWithTag("TrapArea").transform;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        OnDeath();

        if (this.transform.gameObject != null)
        {
            if (isObstacle == true)
            {
                //Collision Avoid
                targetObstacles = obstacles[0].transform;

                targetPos = targetObstacles.position;
                targetDir = -(targetPos - transform.position).normalized;

                SeekSteering();
            }
            else
            {
                if (!isTrap)
                {
                    if (!isFound)
                    {
                        Patrolling();
                    }
                    else
                    {
                        Chasing();
                        exclamationMark.SetActive(true);
                    }
                }
                else
                {
                    if (!isFound)
                    {
                        CheckPlayerInTrap();
                        exclamationMark.SetActive(false);
                    }
                    else
                    {
                        Chasing();
                        exclamationMark.SetActive(true);
                    }
                }

                targetObstacles = null;
            }

            if (targetObstacles != null)
            {
                currentDir = rb.velocity.normalized;
                //Debug.Log(currentDir);
            }

            Debug.DrawRay(transform.position, transform.forward * 3, Color.green);
            Debug.DrawRay(transform.position, targetDir * 3, Color.red);
            Debug.DrawRay(transform.position, steeringDir * 3, Color.yellow);
            //Debug.Log(rb.velocity.magnitude);
        }

        Physics.IgnoreLayerCollision(6, 10);
    }

    void Chasing()
    {
        if (targetPlayer != null)
        {
            targetPos = targetPlayer.position;
            targetDir = (targetPos - transform.position).normalized;

            SeekSteering();
        }
    }

    void Patrolling()
    {
        if (targetPlayer != null)
        {
            targetDist = (transform.position - targetPlayer.position).magnitude;
        }

        if (pathPoints != null)
        {
            if ((targetPos - transform.position).magnitude < 1f)
            {
                numPathPoints++;

                if (numPathPoints == pathPoints.Length)
                {
                    numPathPoints = 0;
                }
            }

            targetPath = pathPoints[numPathPoints];
            targetPos = pathPoints[numPathPoints].position;
            targetDir = (targetPos - transform.position).normalized;

            SeekSteering();
        }

        if (targetDist < sizeRadius)
        {
            isFound = true;
            //Debug.Log("check in patrol");
        }
    }


    void SeekSteering()
    {
        //targetPos = targetPlayer.position;
        //targetDir = (targetPos - transform.position).normalized;
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

        //Steering
        steeringDir = targetDir - currentDir;
        rb.velocity = (currentSpeed * currentDir) + (steeringDir * steeringForce);
    }

    void CheckPlayerInTrap()
    {
        if (targetPlayer != null)
        {
            targetPos = fleeSpot.transform.position;
            targetDist = (transform.position - targetPlayer.position).magnitude;
            targetDir = (targetPos - transform.position).normalized;
        }

        SeekSteering();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            enemyHealth -= getDamage;
            isFound = true;

            Destroy(collision.gameObject);
            //Debug.Log("Destroy Bullet");

            if (enemyHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TrapArea"))
        {
            if (targetDist < sizeRadius)
            {
                isFound = true;
                //Debug.Log("check in trap");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bullet") && !other.CompareTag("TrapArea"))
        {
            obstacles.Add(other.gameObject);
            isObstacle = true;
        }
        else if (other.CompareTag("Player"))
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine("countdownAttack");
        }
        else if (isFound && !isTrap)
        {
            if (other.CompareTag("TrapArea"))
            {
                fleeSpot = other.transform;
                sizeRadius = sizeRadiusInTrap;
                isTrap = true;
                isFound = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bullet") && !other.CompareTag("TrapArea"))
        {
            isObstacle = false;
            obstacles.Clear();
        }
        else if (other.CompareTag("Player"))
        {
            animator.SetBool("isAttacking", false);
            StopCoroutine("countdownAttack");
        }
        else if (isTrap)
        {
            if (other.CompareTag("TrapArea"))
            {
                targetPos = fleeSpot.transform.position;
                exclamationMark.SetActive(false);
                isFound = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sizeRadius);
    }

    IEnumerator countdownAttack()
    {
        timer = 3;

        while (true)
        {
            timer -= 1 * Time.deltaTime;

            if (timer <= 0)
            {
                playerHealth.health -= 1;
                //Debug.Log("Attack");
                timer = 3;
            }

            yield return null;
        }
    }

    public void TakeDamage(int _damage)
    {
        enemyHealth -= _damage;
    }

    private void OnDeath()
    {
        if (enemyHealth <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
