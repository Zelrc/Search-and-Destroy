using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class RayShooter : MonoBehaviour
{
    private Camera _camera;

    PlayerController playerscript;

    //change bullet
    public int bulletChoice;
    public float bulletSpeed = 20f;
    public GameObject[] bullets;
    private GameObject bullet;

    //fire projectile component
    public Transform shootPos;
    public float fireRate = 0.5f;
    private float nextFire = 0.0f;

    //check point
    public LayerMask layer;

    private Vector3 destination;

    private void Awake()
    {
        playerscript = GetComponent<PlayerController>();
    }

    void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        //lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cursor color
        GUI.contentColor = Color.white;

        //pre-set bullet to first type
        bulletChoice = 1;
        BulletType();

    }

    //used late update to prevent shoot position bug
    void LateUpdate()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)//left click
        {
            //set fire rate
            nextFire = Time.time + fireRate;

            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);//center of cam
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;

            playerscript.Source.PlayOneShot(playerscript.Clips[1]);

            if (Physics.Raycast(ray, out hit,layer))
            {
                //do something if hit 
                destination = hit.point;
            }
            else
            {
                destination = ray.GetPoint(1000);
            }
            InstanciateProjectile();
        }

        //switch bullet type
        SwitchBullet();

    }

    void InstanciateProjectile()
    {
        GameObject projectile = Instantiate(bullet, shootPos.position, shootPos.rotation);
        projectile.GetComponent<Rigidbody>().velocity = (destination - shootPos.position).normalized * bulletSpeed;
    }

    //change bullet
    void SwitchBullet()
    {
        if (bullets.Length <= 0)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bulletChoice = 1;
            playerscript.Source.PlayOneShot(playerscript.Clips[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bulletChoice = 2;
            playerscript.Source.PlayOneShot(playerscript.Clips[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bulletChoice = 3;
            playerscript.Source.PlayOneShot(playerscript.Clips[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bulletChoice = 4;
            playerscript.Source.PlayOneShot(playerscript.Clips[3]);
        }
        BulletType();
        
    }

    void BulletType()
    {
        bullet = bullets[bulletChoice-1];
    }

    //draw cursor at middle
    void OnGUI()
    {
        int size = 12;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }

}
