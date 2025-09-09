using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions inputActions;
  
    private Rigidbody rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool inWater;
    private bool swimUp;
    private bool getdown;
    [SerializeField] private float JumpForce = 20f;
    [SerializeField] private Vector3 gravity = new(0f, -0.2f, 0f);

    public Rig rig;
    public static PlayerInput Instance { get; private set; }

    public Vector3 customGravity = new Vector3(0, -15f, 0);

    private void Awake()
    {
        Instance = this;
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions.Player.Attack.performed += Attack_performed;
        inputActions.Player.Sprint.performed += Sprint_performed;
        inputActions.Player.Sprint.canceled += Sprint_canceled;
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Jump.canceled += Jump_canceled;
        inputActions.Player.Crouch.performed += Crouch_performed;
        inputActions.Player.Crouch.canceled += Crouch_canceled;
    }

    private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        getdown = false;
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       
            getdown = true;
       
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
      //  rig.weight = 0.0000f;
       
        swimUp = false;
       
       

    }

    float targetWeight;
    float changeSpeed;
   [SerializeField] float upForce = 1f;
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
        Vector2 movement= GetMovementVector();
       /* if (isGrounded && movement != Vector2.zero && !inWater)
        {
            changeSpeed = 10f;
            targetWeight = 1f;
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * changeSpeed);


            isGrounded = false;

        }
       */
         if (inWater)
        {
            swimUp = true;
            changeSpeed = 10f;
             targetWeight = .2f;
           


        }
        
      
       

    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if snake touches the ground again
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            // rig.weight = 0.0000f;
            changeSpeed = 10f;
            rig.weight = Mathf.Lerp(rig.weight, 0.0000f, Time.deltaTime * changeSpeed);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            //isGrounded = false;
           

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("water"))
        {
            inWater = true;
            Debug.Log("thaneer thaneer");
            
           // rb.AddForce(gravity, ForceMode.Acceleration);
            Physics.gravity = gravity;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("water"))
        {
            inWater = false;
            if(!isGrounded)
            rb.AddForce(Vector3.down*3f, ForceMode.Impulse);
            Physics.gravity =new Vector3(0f,-10f,0f);

        }
       
    }
    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PyMovement.Instance.Slow();
    }

    private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PyMovement.Instance.Fast();
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PyAnimation.Instance.Attack();
    }
   
    // Update is called once per frame
    void Update()
    {

        if (!isGrounded && !inWater)
        {
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
       

        if (inWater && swimUp)
        {
           
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime*changeSpeed );
            rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        }
        if(!swimUp)
             rig.weight = Mathf.Lerp(rig.weight, 0.0000f, Time.deltaTime*changeSpeed);

        if(getdown)
        {
            rb.AddForce(Vector3.down *10f, ForceMode.Impulse);
        }
    }

    public Vector2 GetMovementVector()
    {

        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;

    }
}
