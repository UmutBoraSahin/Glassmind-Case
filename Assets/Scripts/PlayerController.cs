using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CharacterController characterController;
    public Transform playerCamera;
    public AudioSource footstepSource;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;

    [Header("Audio Settings")]
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;
    private float stepTimer;
    private int lastIndex = -1;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraPitch = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        // 1. Movement Logic
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // 2. Footstep Logic with Ground Detection
        bool isNearGround = Physics.Raycast(transform.position, Vector3.down, 5.0f);

        if (moveInput.magnitude > 0.1f && isNearGround)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = stepInterval;
            }
        }

        // 3. Mouse Look Logic
        float yaw = lookInput.x * lookSensitivity;
        cameraPitch -= lookInput.y * lookSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        transform.Rotate(Vector3.up * yaw);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void PlayFootstepSound()
    {
        if (footstepClips.Length > 0)
        {
            int randomIndex;

            // Ensure the same sound never plays twice in a row
            do
            {
                randomIndex = Random.Range(0, footstepClips.Length);
            }
            while (randomIndex == lastIndex && footstepClips.Length > 1);

            lastIndex = randomIndex;

            // Randomize pitch for more variety
            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.PlayOneShot(footstepClips[randomIndex]);
        }
    }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
}