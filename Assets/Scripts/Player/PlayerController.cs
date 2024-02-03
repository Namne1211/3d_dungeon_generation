using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public bool inventoryOn;
    
    private Vector3 _move;

    private float _speed;
    [SerializeField] private float normalSpeed = 10f;
    [SerializeField] private float multiplySpeed = 10f;
    Rigidbody _rb;

    
    [SerializeField] private float jumpSpeed = 25f;
    [SerializeField] private float normalDrag = 8f;
    [SerializeField] private float airDrag = 2f;
    [SerializeField] private float airMultiply = 0.25f;

    [SerializeField] private Transform groundCheck;
    private bool _isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRange = 0.4f;

    [SerializeField] private float playerHeight = 4f;
    private RaycastHit _slopeHit;
    private Vector3 _onSlopeMoveDirection;

    private bool OnSlope()
    { 
        if(Physics.Raycast(transform.position,Vector3.down,out _slopeHit, playerHeight / 2 +0.5f))
        {
            if(_slopeHit.normal != Vector3.up)
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
        inventoryOn = true;
        Time.timeScale = 1f;
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }
    void Update()
    {
        CheckInventory();
        //cancel movement
        if (inventoryOn)
            return;
        
        var playerTransform = transform;
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        DragNGravity();
        Jump();
        //move
        
        _move = playerTransform.right * x + playerTransform.forward * z;

        _onSlopeMoveDirection = Vector3.ProjectOnPlane(_move, _slopeHit.normal);

    }

    //handle basic movement
    private void FixedUpdate()
    {

        if (_isGrounded && !OnSlope())
        {
            _rb.AddForce(_move.normalized * (_speed * multiplySpeed), ForceMode.Acceleration);           
        }
        else if(_isGrounded && OnSlope())
        {
            _rb.AddForce(_onSlopeMoveDirection.normalized * (_speed * multiplySpeed), ForceMode.Acceleration);           
        }
        else
        {
            _rb.AddForce(_move.normalized * (_speed * multiplySpeed * airMultiply), ForceMode.Acceleration);            
        }              
    }

    void DragNGravity()
    {
        //cancel movement
        if (inventoryOn)
            return;
        
        if (_isGrounded)
        {
            _rb.drag = normalDrag;
            _speed = normalSpeed;
        }
        else
        {
            _rb.drag = airDrag;            
        }
        
    }

    void Jump()
    {
        
        _isGrounded = Physics.CheckSphere(groundCheck.position,checkRange,groundLayer);
        
        //normal jump
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    void CheckInventory()
    {
        if(inventoryOn && !Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if(!inventoryOn && Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
}