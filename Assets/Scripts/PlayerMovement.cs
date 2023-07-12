using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2;

    [SerializeField] private GameObject SprintIcon;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    public float moveSpeed;
    float runMultiplier = 1f;
    float runSkillMultiplier = 1f;
    [SerializeField] float airMultiplier = 0.4f;
    private bool runSkillAble = true;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.2f;
    bool isGrounded;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rigid;

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    
    private void Start() 
    {
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;
    }

    private void Update() 
    {
        if (!GameManager.gameStop)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            MyInput();
            ControlDrag();

            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) { runMultiplier = 2; }
            if (Input.GetKeyUp(KeyCode.LeftShift)) { runMultiplier = 1; }
            if (Input.GetKeyDown(KeyCode.E) && runSkillAble) { StartCoroutine(RunSkillDelay()); }


            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }
    }

    IEnumerator RunSkillDelay()
    {
        SprintIcon.SetActive(false);
        runSkillAble = false;
        runSkillMultiplier = 2;
        yield return new WaitForSeconds(30f);
        runSkillMultiplier = 1;
        yield return new WaitForSeconds(60f);
        runSkillAble = true;
        SprintIcon.SetActive(true);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.transform.forward * verticalMovement + orientation.transform.right * horizontalMovement;
    }

    void Jump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ControlDrag()
    {
        if(isGrounded)
        {
            rigid.drag = groundDrag;
        }
        else
        {
            rigid.drag = airDrag;
        }
    }

    private void FixedUpdate() 
    {
        if (!GameManager.gameStop) { MovePlayer(); }    
    }

    void MovePlayer()
    {
        if(isGrounded && !OnSlope())
        {
            rigid.useGravity = true;
            rigid.AddForce(moveDirection.normalized * moveSpeed * runMultiplier * runSkillMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rigid.useGravity = false;
            rigid.AddForce(slopeMoveDirection.normalized * moveSpeed * runMultiplier * runSkillMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rigid.useGravity = true;
            rigid.AddForce(moveDirection.normalized * moveSpeed * runMultiplier * airMultiplier * runSkillMultiplier, ForceMode.Acceleration);
        }
    }
}
