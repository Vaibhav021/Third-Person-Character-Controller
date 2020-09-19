using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables

    //Components
    private CharacterController controller;
    private Animator anim;
    private Camera cam;

    //Inputs
    private float moveX;
    private float moveZ;

    [Header("Bools")]
    private bool running = false;
    private bool jumping = false;
    private bool isGrounded = false;
    public bool canRun;
    private bool canJump;
    private bool isAttacking = false;

    //Gravity Data
    private float gravity = -9.81f;
    private Vector3 downwardVelocity;

    [Header("Movement")]
    public float speed = 0;
    public float jumpHeight = 0;
    private float velocityMagnitude;


    [Header("Look Rotation")]
    public float rotationSpeed = 0;
    public GameObject body = null;
    private Vector3 moveDirection;

    [Header("Detector")]
    public Transform groundDetector = null;
    public float detctorRadius = 0;
    public LayerMask groundLayer = default;



    #endregion

    void Start()
    {
        //Components
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main;

    }

    void Update()
    {
        UpdateInputs();
        UpdateBoolean();

        UpdateMovement();
        UpdateLookDirection();
        UpdateJumpAndGravity();
        UpdateAnimations();


    }

    void UpdateInputs()
    {
        //Move Inputs
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        //Jump Input
        canJump = Input.GetButtonDown("Jump");

        velocityMagnitude = new Vector2(moveX, moveZ).sqrMagnitude;


    }

    void UpdateBoolean()
    {
        //Ground Check
        isGrounded = Physics.CheckSphere(groundDetector.position, detctorRadius, groundLayer);

        running = moveX != 0 || moveZ != 0;


        canRun = running && !isAttacking;

    }

    void UpdateMovement()
    {
       
        if (velocityMagnitude >= 0.4f && canRun)
        {
            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            controller.Move(move * speed * Time.deltaTime);
        }

    }

    void UpdateLookDirection()
    {
        var forward = cam.transform.forward;

        forward.y = 0;

        forward.Normalize();

        moveDirection = forward;

        if (moveX != 0 || moveZ != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed);
            body.transform.localRotation = Quaternion.Slerp(body.transform.localRotation, Quaternion.LookRotation(new Vector3(moveX, 0f, moveZ)), rotationSpeed * 2);
        }

    }

    void UpdateJumpAndGravity()
    {
        //Update Gravity
        if (isGrounded && downwardVelocity.y < 0)
            downwardVelocity.y = -1f;
        else
            downwardVelocity.y += gravity * Time.deltaTime;

        //Update Jump
        if (canJump && isGrounded && jumping)
        {
            //if (moveX != 0 || moveZ != 0)
            {
                downwardVelocity.y = Mathf.Sqrt((jumpHeight / 2f) * -2f * gravity);
            }
        }

        controller.Move(downwardVelocity * Time.deltaTime * 2f);
    }

    void UpdateAnimations()
    {
        #region Movement==============================================

        anim.SetFloat("VelocityMagnitude", velocityMagnitude, 0.0f, Time.deltaTime);

        #endregion

        #region Jump==================================================

        if (canJump)
        {
            jumping = true;
            anim.SetBool("isJumping", true);
        }
        else if (isGrounded)
        {
            jumping = false;
            anim.SetBool("isJumping", false);
        }

        #endregion

        #region Attack================================================

        if (anim.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
            isAttacking = true;
        else
            isAttacking = false;


        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }

        #endregion

    }




































}//class