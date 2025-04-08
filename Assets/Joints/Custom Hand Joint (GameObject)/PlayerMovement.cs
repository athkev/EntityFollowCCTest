using UnityEngine;

[DefaultExecutionOrder(-5)]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _rotationSpeed = 10;

    private JointsDemoInputActionAsset _jointsDemoInputActionAsset;
    private void Awake()
    {
        _jointsDemoInputActionAsset = new();
        _jointsDemoInputActionAsset.Enable();
    }

    private void FixedUpdate()
    {
        var movementInput = _jointsDemoInputActionAsset.Movement.PlayerMovement.ReadValue<Vector2>();
        var rotationInput = _jointsDemoInputActionAsset.Movement.PlayerRotation.ReadValue<float>();

        _rb.linearVelocity = new Vector3(movementInput.x, 0, movementInput.y) * _movementSpeed;
        _rb.angularVelocity = new Vector3(0, rotationInput, 0) * Mathf.Deg2Rad * _rotationSpeed;
    }
}