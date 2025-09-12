using System.Numerics;
using Cysharp.Threading.Tasks.Triggers;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PyMovement : MonoBehaviour
{
    public static PyMovement Instance { get; private set; }


    [SerializeField] private float slowSpeed;
    [SerializeField] private float fastSpeed;
   
    [SerializeField] private Transform cam;

    private Rigidbody rb;

    private bool isSlither;
    private bool isFast;


    private float currentSpeed;
    private float targetAngle;
    private float angle;  
    private float turnSmoothvelocity;

    private Vector2 movementvector;
    private Vector3 movement;
    private Vector3 moveDir;
    private RaycastHit hit;

    private void Awake()
    {
        Instance = this;
        currentSpeed = slowSpeed;
        rb = GetComponent<Rigidbody>();
       
    }
  

    private void FixedUpdate()
    {
        Movement();
       
    }
    Quaternion targetRotation;
    Vector3 slopeForward;
    private void Movement()
    {
        movementvector = PlayerInput.Instance.GetMovementVector();
        movement = new Vector3(movementvector.x, 0, movementvector.y).normalized;



        if (movement != Vector3.zero)
        {
            // --- Rotation ---
            targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y ;
           

            // --- Movement ---
            moveDir = UnityEngine.Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            SlopeAlign(moveDir);
            Vector3 targetPosition = rb.position + moveDir.normalized * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition); // ✅ physics-based move


        }
        if (!isFast)
            isSlither = movement != Vector3.zero;


    }
    public float smoothTime = 0.3f;

    private Quaternion smoothDampVel; // stores angular velocity for smooth damp
    float offset = 4.5f;
    private void SlopeAlign(Vector3 moveDir)
    {
        Vector3 origin = transform.position + transform.forward *offset;
        if (Physics.Raycast(origin, Vector3.down, out hit, 6f))
        {
            Vector3 groundNormal = hit.normal;
            Vector3 forward = moveDir.magnitude > 0.1f ? moveDir : transform.forward;

            // Keep forward direction projected onto slope
            Vector3 slopeForward = Vector3.ProjectOnPlane(forward, groundNormal);
         
            Quaternion targetRotation = Quaternion.LookRotation(slopeForward, groundNormal);


            // Smooth damp rotation
            Quaternion newRotation = SmoothDampRotation(rb.rotation, targetRotation, ref smoothDampVel, smoothTime);

            // Apply with physics
            rb.MoveRotation(newRotation);
         
          
        }

    }
    Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime)
    {
        if (Time.deltaTime < Mathf.Epsilon) return current;

        // Find relative rotation (delta)
        Quaternion delta = target * Quaternion.Inverse(current);

        delta.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        if (Mathf.Abs(angle) > 0.01f)
        {
            float t = 1f - Mathf.Exp(-Time.deltaTime / smoothTime);
            Quaternion step = Quaternion.AngleAxis(angle * t, axis);
            return step * current;
        }

        return target;
    }

   

    public bool IsSlithering()
    {
        return isSlither;
    }
    public bool IsFast()
    {
        return isFast;
    }


    public void Fast()
    {
        currentSpeed = fastSpeed;
        isFast = true;
        isSlither = false;
    
    }
    public void Slow()
    {
        isFast = false;
        
        currentSpeed = slowSpeed;
    }
}
