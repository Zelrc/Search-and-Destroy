using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    Transform target;
    [SerializeField] private Transform[] spawnPoint;

    [SerializeField] private GameObject bullet;

    public static bool inRange = false;

    int index = 0;

    private void Start()
    {
        target = GameObject.Find("Player").transform;
        Invoke("SpawnMissle", 2f);
    }

    //private void Update()
    //{
    //    float distance = Vector3.Distance(target.position, transform.position);

    //    if(distance < DetectRange)
    //    {
    //        SpawnMissle();
    //    }

    //    Debug.Log(distance);
    //}

    void SpawnMissle()
    {
        if (inRange)
        {
            GameObject NewBullet = Instantiate(bullet, spawnPoint[index].position, spawnPoint[index].rotation);

            if (index < spawnPoint.Length)
            {
                index++;
            }
            if (index == 3)
            {
                index = 0;
            }

        }
        Invoke("SpawnMissle", 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
