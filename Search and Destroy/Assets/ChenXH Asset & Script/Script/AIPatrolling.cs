using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolling : MonoBehaviour
{
    //Health
    [SerializeField] int enemyHealth;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int getDamage = 10;

    //Movement
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

    //Chasing
    [SerializeField] Transform targetPlayer;
    float targetDist;
    public float sizeRadius;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] bool isFound;

    //Waypoints
    [SerializeField] Transform targetPath;
    [SerializeField] Transform[] pathPoints;
    [SerializeField] int numPathPoints;

    //Avoidance
    [SerializeField] Transform targetObstacles = null;
    public bool isObstacle = false;
    [SerializeField] List<GameObject> obstacles = new List<GameObject>();

    //Damage player
    float timer = 1;
    PlayerHealth playerHealth;

    Animator animator;     

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        accelerationRate = maxSpeed / accelerationTimeToMax;

        enemyHealth = maxHealth;

        playerHealth = targetPlayer.GetComponent<PlayerHealth>();

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
            }
            else
            {
                if (!isFound)
                {
                    Patrolling();
                }
                else
                {
                    Chasing();
                }

                targetObstacles = null;
            }

            if (targetObstacles != null)
            {
                currentDir = rb.velocity.normalized;
                //  Debug.Log(currentDir);
            }

            SteeringMovement();

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

            exclamationMark.SetActive(true);
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

            SteeringMovement();
        }

        if (targetDist < sizeRadius)
        {
            isFound = true;
        }
    }


    void SteeringMovement()
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            obstacles.Add(other.gameObject);
            isObstacle = true;
        }
        else if (other.CompareTag("Player"))
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine("countdownAttack");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            isObstacle = false;
            obstacles.Clear();
        }
        else if (other.CompareTag("Player"))
        {
            animator.SetBool("isAttacking", false);
            StopCoroutine("countdownAttack");
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
                Debug.Log("Attack");
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
