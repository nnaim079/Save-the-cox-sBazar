using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -15f;

    [Header("Touchpad & Keyboard Rotation")]
    public float keyboardTurnSpeed = 90f; 
    public float touchpadSensitivity = 2f; 

    private Vector3 velocity;
    private float verticalRotation = 0f;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        if (controller != null)
        {
            controller.Move(move.normalized * moveSpeed * Time.deltaTime);

            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        
        float turnAmount = 0f;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))  turnAmount -= 1f;
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow)) turnAmount += 1f;

        
        turnAmount += Input.GetAxis("Mouse X") * touchpadSensitivity;

        
        transform.Rotate(Vector3.up * turnAmount * keyboardTurnSpeed * Time.deltaTime);

        
        float pitch = 0f;
        if (Input.GetKey(KeyCode.UpArrow))   pitch -= 1f;
        if (Input.GetKey(KeyCode.DownArrow)) pitch += 1f;

        verticalRotation += pitch * 60f * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}