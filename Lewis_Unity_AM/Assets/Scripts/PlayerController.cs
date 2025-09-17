using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Camera playerCam;

    Rigidbody rb;
    Ray ray;

    float verticalMove;
    float horizontalMove;

    public float speed = 5f;
    public float jumpHeight = 10f;
    public float groundDetectLength = .5f;

    public int health = 5;
    public int maxHealth = 5;


    public void Start()
    {
        ray = new Ray(transform.position, -transform.up);
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        
        // Player Rotation (Horiztonally)
        Quaternion playerRotation = playerCam.transform.rotation;
        playerRotation.x = 0;
        playerRotation.z = 0;
        transform.rotation = playerRotation;
        

        // Movement System (Take vert/horiz input and convert to 3D movement)
        Vector3 temp = rb.linearVelocity;

        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;

        ray.origin = transform.position;
        ray.direction = -transform.up;

        

        rb.linearVelocity = (temp.x * transform.forward) + 
                            (temp.y * transform.up) + 
                            (temp.z * transform.right);
    }
    // Read player input and convert to values to be used by rb for movement
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputAxis = context.ReadValue<Vector2>();

        verticalMove = inputAxis.y;
        horizontalMove = inputAxis.x;
    }
    // If raycast downward sees collider, player can jump.
    public void Jump()
    {
        if (Physics.Raycast(ray, groundDetectLength))
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "killzone")
            health = 0;

        if ((other.tag == "health") && (health < maxHealth))
        {
            health++;
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hazard")
            health--;
    }
}
