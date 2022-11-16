using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] Clips;

    //InGameCanvas canvas;

    //move
    public float speed = 4.0f;
    private float currentSpeed;
    public float gravity = -9.8f;
    private CharacterController _charController;
    Vector3 Velocity;

    //mouse rotation
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float Sensitivity = 3.0f;
    //public float sensitivityHor = 3.0f;
    //public float sensitivityVert = 3.0f;
    private float minimumVert = -45.0f;
    private float maximumVert = 45.0f;
    private float _rotationX = 0;
    //camera
    private Camera cam;


    //jump
    //check ground
    bool isGrounded;
    public float jumpForce = 2f;
    private Rigidbody rb;
    private bool isGround;

    //health and respawn
    public int maxHp = 3;
    public int currentHp;
    public Transform spawnLocation;

    //check point & respawn
    public GameObject CheckPointIndicator;
    public Vector3 CheckPointIndicatorOffset = new Vector3(0, -1.5f, -0.85f);
    private bool respawning = false;

    //Score
    [HideInInspector]
    public int deathCount;
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int rounds;
    [HideInInspector]
    public bool EndKey = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _charController = GetComponent<CharacterController>();
        cam = Camera.main;
        Source = GetComponent<AudioSource>();
        //canvas = GameObject.Find("InGameCanvas").GetComponent<InGameCanvas>();
    }

    private void Start()
    {
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
        currentHp = maxHp;

        currentSpeed = speed;

        //load score
        // PlayerPrefs.SetInt("DeathCount",0);
        // PlayerPrefs.SetInt("Score",0);
        PlayerPrefs.Save();

        // Load Sensitivity
        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
    }

    private void Update()
    {
        if (currentHp > 0 && !respawning)
        {
            float deltaX = Input.GetAxis("Horizontal") * currentSpeed;
            float deltaZ = Input.GetAxis("Vertical") * currentSpeed;
            Vector3 movement = transform.right * deltaX + transform.forward * deltaZ;

            //run
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = speed * 1.3f;
            }
            else
            {
                currentSpeed = speed;
            }

            _charController.Move(movement * currentSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                //prevent double jump
                if (isGround)
                {
                    Velocity.y = 0f;
                    Velocity.y += Mathf.Sqrt(jumpForce * -3f * gravity);
                    Source.PlayOneShot(Clips[0]);
                }
            }
            if (Velocity.y <= -1f)
            {
                Velocity.y = Mathf.Clamp(Velocity.y + gravity * 4 * Time.deltaTime, -20f, 20f);
            }
            else
            {
                Velocity.y = Mathf.Clamp(Velocity.y + gravity * 3 * Time.deltaTime, -20f, 20f);
            }

            _charController.Move(Velocity * Time.deltaTime);

            isGround = _charController.isGrounded;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(PlayerPrefs.GetInt("Score"));
        //Debug.Log("death " + PlayerPrefs.GetInt("DeathCount"));
        if (currentHp > 0 && !respawning)
        {
            //mouse rotate screen
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, mouseX * Sensitivity, 0);
            }
            else if (axes == RotationAxes.MouseY)
            {
                _rotationX -= mouseY * Sensitivity;
                _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
                float rotationY = transform.localEulerAngles.y;
                transform.localEulerAngles = new Vector3(0, rotationY, 0);
                cam.transform.localEulerAngles = new Vector3(_rotationX, 0, 0);
            }
            else
            {
                _rotationX -= mouseY * Sensitivity;
                _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
                float delta = mouseX * Sensitivity;
                float rotationY = transform.localEulerAngles.y + delta;
                transform.localEulerAngles = new Vector3(0, rotationY, 0);
                cam.transform.localEulerAngles = new Vector3(_rotationX, 0, 0);
            }


            //player input
            //float deltaX = Input.GetAxis("Horizontal") * currentSpeed;
            //float deltaZ = Input.GetAxis("Vertical") * currentSpeed;
            //Vector3 movement = transform.right * deltaX + transform.forward * deltaZ;

            ////run
            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    currentSpeed = speed * 1.3f;
            //}
            //else
            //{
            //    currentSpeed = speed;
            //}

            //_charController.Move(movement * currentSpeed * Time.deltaTime);

            //jump
            //if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            //{
            //    //prevent double jump
            //    if (isGround)
            //    {
            //        Velocity.y = 0f;
            //        Velocity.y += Mathf.Sqrt(jumpForce * -3f * gravity);
            //        Source.PlayOneShot(Clips[0]);
            //    }
            //}

            //fall setting
            //if (Velocity.y <= -1f)
            //{
            //    Velocity.y = Mathf.Clamp(Velocity.y + gravity * 4 * Time.deltaTime, -20f, 20f);
            //}
            //else
            //{
            //    Velocity.y = Mathf.Clamp(Velocity.y + gravity * 3 * Time.deltaTime, -20f, 20f);
            //}


            //_charController.Move(Velocity * Time.deltaTime);

            //check is grounded
            //isGround = _charController.isGrounded;

        }
        else
        {
            if (!respawning)
            {
                _charController.enabled = false;
                respawning = true;
                //respawn player
                Respawn();
            }
        }
    }

    //check damage and death platform
    public void Hurt(int damage)
    {
        currentHp -= damage;
        Debug.Log("Health: " + currentHp);
    }

    void Respawn()
    {
        deathCount = PlayerPrefs.GetInt("DeathCount");
        deathCount += 1;
        PlayerPrefs.SetInt("DeathCount", deathCount);
        PlayerPrefs.Save();
        rb.velocity = Vector3.zero;
        rb.transform.position = spawnLocation.position;
        currentHp = maxHp;
        _charController.enabled = true;
        respawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 2)
        {
            if (other.gameObject.CompareTag("CheckPoint"))
            {
                spawnLocation = other.transform;
                
                //if (other.gameObject.GetComponent<CheckPoint>() != null)
                //{
                //    other.gameObject.GetComponent<CheckPoint>().isTrigger = true;
                //    Source.PlayOneShot(Clips[2]);
                //    CheckPointIndicator.transform.position = other.gameObject.transform.position + CheckPointIndicatorOffset;
                //}

                if (other.gameObject.name == "CheckPoint4")
                {
                    EndKey = true;
                    //canvas.TipsHolder.SetActive(true);
                    //canvas.Tips.text = "Now reach back to Starting Point.";
                }
            }
        }

        if (other.gameObject.CompareTag("KillPlane"))
        {
            currentHp = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 2)
        {
            if (other.gameObject.CompareTag("CheckPoint") && other.gameObject.name == "CheckPoint4")
            {
                //canvas.TipsHolder.SetActive(false);
            }
        }
    }
}
