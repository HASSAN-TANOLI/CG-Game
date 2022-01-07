using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    private int desiredLane = 1; //0: left 1:middle 2:right
    public float laneDistance = 4; // distance between two lanes
    
    public float jumpForce;

    public float Gravity = -20;
    public bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Animator animator;
    private bool isSliding = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
            return;

        //Increase speed
        if(forwardSpeed < maxSpeed)
            forwardSpeed += 0.1f * Time.deltaTime;

        animator.SetBool("isGameStarted", true);  
        direction.z = forwardSpeed;

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);

        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)   //Checking whether the player left to the ground in order to make another jump player must be at ground level
        {
            direction.y = -2;
            if (SwipeManager.swipeUp)
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }

        if(SwipeManager.swipeDown)
        {
            StartCoroutine(Slide());
        }
        //Gather the inputs on which lane we should be 

        if (SwipeManager.swipeDown)
        {
            desiredLane++;
            if (desiredLane == 3)

                desiredLane = 2;

        }

        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1)

                desiredLane = 0;

        }

        //calcuate where we should be in the lane in future
        //calculating the target position

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;

        }

        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 movDir = diff.normalized * 25 * Time.deltaTime;
        if (movDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(movDir);
        else
            controller.Move(diff);

    }
     

    private void Jump()
    {
        direction.y = jumpForce;
          
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds(1.3f);
        controller.center = new Vector3(0, 0, 0);
        controller.height = 2;
        animator.SetBool("isSliding", false);

   
        isSliding = false;
    }
}
