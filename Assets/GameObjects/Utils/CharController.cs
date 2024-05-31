using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharController : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = .4f;
    [SerializeField]
    private float _playerSpeed = 2.0f;
    [SerializeField]
    private float _jumpHeight = 1.0f;
    [SerializeField]
    private float _gravityValue = -9.81f;

    [SerializeField]
    private PlayerInput _pInput;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private Transform _camera;

    private InputActionAsset _inputActions;
    private bool isGrounded;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _pInput = GetComponent<PlayerInput>();
        _camera = Camera.main.transform;
    }

    void Update()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        Vector2 input = _pInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * _camera.right + move.z * _camera.forward;
        move.y = 0;
        _controller.gameObject.transform.Translate(move * Time.deltaTime * _playerSpeed);

        // Changes the height position of the player..
        if (_pInput.actions["Jump"].triggered && _groundedPlayer)
        {
            //_animator.Play("Jump");
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.gameObject.transform.Translate(_playerVelocity * Time.deltaTime);

        //_animator.SetFloat("Blend", input.sqrMagnitude, _animationBlendMap, Time.deltaTime);

        if (input != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
        }
    }
}