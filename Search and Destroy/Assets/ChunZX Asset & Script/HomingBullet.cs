using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    Transform target;
    Vector3 direction;

    [SerializeField] private float speed = 20;
    [SerializeField] private float rotationSpeed = 5;

    PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = target.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if(target != null)
        {
            direction = target.position - transform.position;
            direction = direction.normalized;

            var rot = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            playerHealth.health -= 1;
        }
    }

}
