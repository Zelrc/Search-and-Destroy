using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 move;

    public CharacterController controller;

    public float speed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public float dashRate = 0.5f;
    public float nextDash = 0.0f;
    public float dashSpeed = 40f;
    public bool isDash = false;

    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        
        move = transform.right * x + transform.forward * z;

        

        


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time > nextDash)
        {

            
            StartCoroutine(dashCooldown());

            nextDash = Time.time + dashRate;

            

        }

        if (!isDash)
        {
            controller.Move(move * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * dashSpeed * Time.deltaTime);
        }


        velocity.y += gravity * 2 * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


    }

    IEnumerator dashCooldown()
    {
        isDash = true;
        
        yield return new WaitForSeconds(0.2f);

        isDash = false;
    }
    
}
