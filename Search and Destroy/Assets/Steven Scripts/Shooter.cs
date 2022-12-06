using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private Camera _camera;

    PlayerMovement player;

    public float bulletSpeed = 20f;
    public GameObject bullet;

    bool tracking = false;

    public Transform shootPos;
    public float fireRate = 0.5f;
    public float nextFire = 0.0f;

    public LayerMask layer;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        _camera = GetComponentInChildren<Camera>();

        GUI.contentColor = Color.black;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, layer))
            {
                destination = hit.point;
            }
            else
            {
                destination = ray.GetPoint(1000);
            }
            InstanciateProjectile();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tracking = false;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            tracking = true;
        }
    }

    void InstanciateProjectile()
    {
        GameObject projectile = Instantiate(bullet, shootPos.position, shootPos.rotation);

        if(tracking)
        {
            projectile.GetComponent<TrackingBullets>().shootPos = shootPos;
            projectile.GetComponent<TrackingBullets>().destination = destination;
        }
        else
        {
            projectile.GetComponent<Rigidbody>().velocity = (destination - shootPos.position).normalized * bulletSpeed;
            projectile.GetComponent<TrackingBullets>().enabled = false;
        }
        
        //projectile.GetComponent<Rigidbody>().velocity = (destination - shootPos.position).normalized * bulletSpeed;

    }

    private void OnGUI()
    {
        int size = 12;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }
}
