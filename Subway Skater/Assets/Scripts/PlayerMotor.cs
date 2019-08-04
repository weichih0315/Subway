using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {

    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.5f;

    // Animator
    private Animator animator;

    //Movement
    private CharacterController characterController;
    private float jumpForce = 6.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int currentLane = 1;// 0:Left 1:middle 2:right
    private bool isRunning = false;

    //Speed Modifier 
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick = 0f;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    private void Start ()
    {
        speed = originalSpeed;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update ()
    {
        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameUI.instance.UpdateSpeedUI(speed - originalSpeed);
            GameManager.instance.UpdateModifierScore(speed - originalSpeed + 1);
        }

        SwipeInput.Direction swipeDirection = SwipeInput.Instance.SwipeDirection;

        if (swipeDirection == SwipeInput.Direction.Right)
            currentLane += 1;
        else if (swipeDirection == SwipeInput.Direction.Left)
            currentLane += -1;
        currentLane = Mathf.Clamp(currentLane, 0, 2);

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (currentLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (currentLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        // Let's calculate our move delta;
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        // Calaulate Y
        if (isGrounded)   //if Grounded
        {
            verticalVelocity = -0.1f;

            if (swipeDirection == SwipeInput.Direction.Up)
            {
                animator.SetBool("Sliding", false);
                // Jump
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (swipeDirection == SwipeInput.Direction.Down)
            {
                //Sliding
                StartSliding();
                Invoke("StopSliding",1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            
            if (swipeDirection == SwipeInput.Direction.Down)
            {
                // Fast Falling mechanic 
                verticalVelocity = -jumpForce;

                //Sliding
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the Penguin
        characterController.Move(moveVector * Time.deltaTime);

        //Rotate the penguin
        Vector3 direction = characterController.velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, TURN_SPEED);
        }
    }
    
    private bool IsGrounded()
    {
           Ray groundRay = new Ray(
            new Vector3(
                characterController.bounds.center.x,
            (characterController.bounds.center.y - characterController.bounds.extents.y) + 0.2f,
            characterController.bounds.center.z),
            Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRun()
    {
        animator.SetTrigger("StartRunning");
        isRunning = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
    }

    private void StartSliding()
    {
        animator.SetBool("Sliding", true);
        characterController.height /= 2;
        characterController.center = new Vector3(characterController.center.x, characterController.center.y /2, characterController.center.z);
    }

    private void StopSliding()
    {
        animator.SetBool("Sliding", false);
        characterController.height *= 2;
        characterController.center = new Vector3(characterController.center.x, characterController.center.y * 2, characterController.center.z);
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        isRunning = false;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        GameManager.instance.PlayerDead();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        string tag = hit.gameObject.tag;

        if (tag == "Obstacle")
            Death();
    }
}