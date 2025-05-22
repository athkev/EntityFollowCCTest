using UnityEngine;

[DefaultExecutionOrder(-4)]
public class ControllerMovement : MonoBehaviour
{
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
        var movementInput = _jointsDemoInputActionAsset.Movement.ControllerMovement.ReadValue<Vector2>();
        var rotationInput = _jointsDemoInputActionAsset.Movement.ControllerRotation.ReadValue<float>();

        transform.position += new Vector3(movementInput.x, 0, movementInput.y) * _movementSpeed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(Vector3.up * rotationInput * _rotationSpeed * Time.fixedDeltaTime);
    }
}