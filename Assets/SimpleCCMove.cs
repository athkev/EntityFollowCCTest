using UnityEngine;

public class SimpleCCMove : MonoBehaviour
{
    [SerializeField] public Transform _controller;
    [SerializeField] public bool _grabbutton;
    [SerializeField] public int _fps = 60;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    public static SimpleCCMove Instance { get; private set; }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Instance = this;
    }

    private void Update()
    {
        Application.targetFrameRate = _fps;
    }

    void FixedUpdate()
    {
        groundedPlayer = characterController.isGrounded;

        // Movement
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = move.normalized;
        characterController.Move(transform.TransformDirection(move) * Time.deltaTime * moveSpeed);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        // Gravity
        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
