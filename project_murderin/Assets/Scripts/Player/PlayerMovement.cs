using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 7f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 1f;
    public Transform playerCamera;
    public GameObject localCamera;

    private Rigidbody rb;
    private bool isGrounded;
    private float cameraXRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            localCamera.SetActive(false);
            return;
        }
        CheckGrounded();
        RotatePlayerAndCamera();
        MovePlayer();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        float rayLength = 1.1f; // Длина луча чуть больше половины высоты персонажа
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);
    }

    void RotatePlayerAndCamera()
    {
        float Sensitivity = mouseSensitivity * 250;
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 forwardDirection = transform.forward * moveVertical;
        Vector3 rightDirection = transform.right * moveHorizontal;
        Vector3 moveDirection = (forwardDirection + rightDirection).normalized;

        // Мгновенное изменение скорости (без плавного разгона/торможения)
        Vector3 horizontalVelocity = moveDirection * speed;
        rb.velocity = new Vector3(
            horizontalVelocity.x,
            rb.velocity.y, // Сохраняем вертикальную скорость (гравитация/прыжок)
            horizontalVelocity.z
        );
    }
}