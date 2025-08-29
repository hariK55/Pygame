using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions inputActions;
  
    private Rigidbody rb;
    private bool isGrounded;
    private bool inWater;
    private bool swimUp;
    [SerializeField] private float JumpForce = 20f;
    Vector3 gravity = new(0f, -1f, 0f);

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

    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // rig.weight = 0.0000f;
        if (inWater)
            swimUp = false;

    }

    float targetWeight;
    float changeSpeed;
   [SerializeField] float upForce = 5f;
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
        Vector2 movement= GetMovementVector();
        if (isGrounded && movement != Vector2.zero && !inWater)
        {
            changeSpeed = 10f;
            targetWeight = 1f;
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * changeSpeed);


            isGrounded = false;

        }
        else if (inWater)
        {
            swimUp = true;
            changeSpeed = 5f;
             targetWeight = .7f;
          

        }
        
      
       

    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if snake touches the ground again
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            rig.weight = 0.0000f;
           

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("water"))
        {
            inWater = true;
            Debug.Log("thaneer thaneer");
            Physics.gravity = Vector3.zero;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        inWater = false;
        Physics.gravity = new Vector3(0f, -10f, 0f);
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
            rb.AddForce(Vector3.up * upForce, ForceMode.Acceleration);
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * changeSpeed);
        }
    }

    public Vector2 GetMovementVector()
    {

        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;

    }
}
