using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [Space]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundRaycastDistance = 0.2f;
    [SerializeField] private int groundRayDensity = 5;
    [SerializeField] private float minCheckThreshold = 0.02f;
    [SerializeField] private LayerMask groundMask;

    [Space]
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float maxFallSpeed = 20f;

    private float yVelocity = 0f;

    private float _playerWidth = 0f;

    private bool _isGrounded = false;
    private bool _isJumping = false;
    
    void Start()
    {
        // For now, we're going to get the player's width from their Capsule Collider Radius
        _playerWidth = GetComponent<CapsuleCollider>().radius * 2f;
        Debug.Log("[PlayerController.cs] '_playerWidth' is: " + _playerWidth);
    }
    
    void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"),
                                       Input.GetAxisRaw("Vertical"));
        
        transform.Translate(movement.x * moveSpeed * Time.deltaTime * Vector3.right);
        
        // Checking if grounded...
        float[] groundDistances = new float[groundRayDensity];
        float rayDistanceStep = (groundRayDensity == 1) ? 0f : _playerWidth / (groundRayDensity - 1);
        _isGrounded = false;    // This will change to true if at least 1 ray returns.
        for (int i = 0; i < groundRayDensity; i++)
        {
            groundDistances[i] = groundRaycastDistance;
            
            RaycastHit groundHit;
            Vector3 origin = (groundCheckTransform.position - Vector3.right * (_playerWidth / 2) + Vector3.up * 0.1f) + (rayDistanceStep * i * Vector3.right);
            if (Physics.Raycast(origin, Vector3.down, out groundHit, groundRaycastDistance, groundMask))
            {
                _isGrounded = true;
                groundDistances[i] = groundHit.point.y - groundCheckTransform.position.y;
            }
            
            Debug.DrawRay(origin, Vector3.down * groundRaycastDistance, Color.red);
        }

        if (_isGrounded)
        {
            // Now, we'll translate the player according to the minimum distance we detected.
            float minDistance = groundRaycastDistance;
            for (int i = 0; i < groundRayDensity; i++)
            {
                minDistance = (Mathf.Abs(groundDistances[i]) < minDistance) ? groundDistances[i] : minDistance;
            }

            if (Mathf.Abs(minDistance) > minCheckThreshold)
            {
                transform.Translate(Vector3.up * minDistance);
            }

            yVelocity = 0f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
            transform.Translate(Vector3.up * yVelocity);
        }
    }

    // We have a Rigidbody on the Player, but isKinematic is turned on and Gravity is turned off.
    // I'm going to experiment implementing my own gravity. The Rigidbody is purely for collision checks
    //  in the future.
    void FixedUpdate()
    {
        
    }
}
