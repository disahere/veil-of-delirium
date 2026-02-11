// Assets/CodeBase/_Prototype/Movement/VeilPlayerController.cs
using Fusion;
using UnityEngine;

namespace CodeBase._Prototype.Movement
{
  public class VeilPlayerController : NetworkBehaviour
  {
    [SerializeField] float moveSpeed = 5f;

    [Header("Camera")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform cameraRoot;

    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    [Header("Movement")]
    [SerializeField] float gravity = -20f;
    [SerializeField] float jumpHeight = 1.5f;

    CharacterController _controller;
    VeilInputActions _input;
    Vector2 _moveInput;
    Vector2 _lookInput;
    bool _jumpPressed;
    float _pitch;
    float _yaw;
    float _verticalVelocity;

    public override void Spawned()
    {
      _controller = GetComponent<CharacterController>();

      if (Object.HasInputAuthority)
      {
        if (playerCamera != null)
        {
          playerCamera.gameObject.SetActive(true);
          playerCamera.enabled = true;
        }

        _input = new VeilInputActions();
        _input.Player.Enable();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled  += _ => _moveInput = Vector2.zero;

        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled  += _ => _lookInput = Vector2.zero;

        _input.Player.Jump.performed += _ => _jumpPressed = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Vector3 e = transform.eulerAngles;
        _yaw   = e.y;
        _pitch = 0f;
      }
      else
      {
        if (playerCamera != null)
        {
          playerCamera.enabled = false;
          playerCamera.gameObject.SetActive(false);
        }
      }
    }

    void OnDestroy()
    {
      if (_input != null)
      {
        _input.Player.Disable();
        _input.Dispose();
        _input = null;
      }
    }

    public override void FixedUpdateNetwork()
    {
      if (!Object.HasInputAuthority || _controller == null)
        return;
      
      Quaternion yawRot = Quaternion.Euler(0f, _yaw, 0f);

      Vector3 worldInput = new Vector3(_moveInput.x, 0f, _moveInput.y);
      if (worldInput.sqrMagnitude > 1f)
        worldInput.Normalize();

      Vector3 localMove = yawRot * worldInput;
      Vector3 velocity  = localMove * moveSpeed;

      if (_controller.isGrounded)
      {
        if (_verticalVelocity < 0f)
          _verticalVelocity = -1f;

        if (_jumpPressed)
        {
          _jumpPressed = false;
          _verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }
      }
      else
      {
        _verticalVelocity += gravity * Runner.DeltaTime;
      }

      velocity.y = _verticalVelocity;

      _controller.Move(velocity * Runner.DeltaTime);

      _jumpPressed = false;
    }

    void Update()
    {
      if (!Object.HasInputAuthority)
        return;

      HandleLook();
      HandleCameraPitch();
    }

    void LateUpdate()
    {
      if (!Object.HasInputAuthority)
        return;
      
      Vector3 e = transform.eulerAngles;
      e.y = _yaw;
      transform.rotation = Quaternion.Euler(e);
    }

    void HandleLook()
    {
      float mouseX = _lookInput.x * mouseSensitivity;
      float mouseY = _lookInput.y * mouseSensitivity;

      _yaw   += mouseX;
      _pitch -= mouseY;
      _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    void HandleCameraPitch()
    {
      if (cameraRoot == null)
        return;

      Vector3 camEuler = cameraRoot.localEulerAngles;
      camEuler.x = _pitch;
      cameraRoot.localEulerAngles = camEuler;
    }
  }
}
