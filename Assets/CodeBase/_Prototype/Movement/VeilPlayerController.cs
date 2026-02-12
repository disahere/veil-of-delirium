// Assets/CodeBase/_Prototype/Movement/VeilPlayerController.cs
using CodeBase._Prototype.CameraEffect;
using Fusion;
using UnityEngine;

namespace CodeBase._Prototype.Movement
{
  public class VeilPlayerController : NetworkBehaviour
  {
    [Header("Movement")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float jumpHeight = 1.5f;

    [Header("Crouch")]
    [SerializeField] float crouchHeightFactor = 0.6f;
    [SerializeField] float crouchLerpSpeed = 12f;
    [SerializeField] float crouchCameraOffset = -0.4f;

    [Header("Camera")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform cameraRoot;

    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    CharacterController _controller;
    VeilInputActions _input;

    Vector2 _moveInput;
    Vector2 _lookInput;
    bool _jumpPressed;
    bool _isSprinting;
    bool _isCrouching;

    float _pitch;
    float _yaw;
    float _verticalVelocity;

    CameraEffects _cameraEffects;

    float _standHeight;
    Vector3 _standCenter;
    float _standCameraY;

    public override void Spawned()
    {
      _controller = GetComponent<CharacterController>();
      _cameraEffects = playerCamera != null
        ? playerCamera.GetComponent<CameraEffects>()
        : null;

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
        _input.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += _ => _lookInput = Vector2.zero;

        _input.Player.Jump.performed += _ => _jumpPressed = true;
        _input.Player.Sprint.performed += _ => _isSprinting = true;
        _input.Player.Sprint.canceled += _ => _isSprinting = false;
        _input.Player.Crouch.performed += _ => _isCrouching = !_isCrouching;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 e = transform.eulerAngles;
        _yaw = e.y;
        _pitch = 0f;

        _standHeight = _controller.height;
        _standCenter = _controller.center;

        if (cameraRoot != null)
          _standCameraY = cameraRoot.localPosition.y;
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

      HandleMovement();
    }

    void Update()
    {
      if (!Object.HasInputAuthority)
        return;

      HandleLook();
      HandleCameraPitch();
      HandleCrouchSmoothing();
    }

    void LateUpdate()
    {
      if (!Object.HasInputAuthority)
        return;

      Vector3 e = transform.eulerAngles;
      e.y = _yaw;
      transform.rotation = Quaternion.Euler(e);
    }

    void HandleMovement()
    {
      Quaternion yawRot = Quaternion.Euler(0f, _yaw, 0f);
      Vector3 worldInput = new Vector3(_moveInput.x, 0f, _moveInput.y);

      float moveAmount = Mathf.Clamp01(worldInput.magnitude);

      if (worldInput.sqrMagnitude > 1f)
        worldInput.Normalize();

      float speed = _isSprinting && !_isCrouching
        ? sprintSpeed
        : walkSpeed * (_isCrouching ? 0.5f : 1f);

      Vector3 localMove = yawRot * worldInput;
      Vector3 velocity = localMove * speed;

      if (_controller.isGrounded)
      {
        if (_verticalVelocity < 0f)
          _verticalVelocity = -1f;

        if (_jumpPressed && !_isCrouching)
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
      
      if (_cameraEffects != null)
        _cameraEffects.SetMoveAmount(moveAmount);
    }

    void HandleLook()
    {
      float mouseX = _lookInput.x * mouseSensitivity;
      float mouseY = _lookInput.y * mouseSensitivity;

      _yaw += mouseX;
      _pitch -= mouseY;
      _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    void HandleCameraPitch()
    {
      if (cameraRoot == null)
        return;

      Vector3 camEuler = cameraRoot.localEulerAngles;
      camEuler.x = _pitch;
      cameraRoot.localEulerAngles = camEuler;
    }

    void HandleCrouchSmoothing()
    {
      if (_controller == null || cameraRoot == null)
        return;

      float targetHeight = _isCrouching ? _standHeight * crouchHeightFactor : _standHeight;
      float targetCamY = _isCrouching ? _standCameraY + crouchCameraOffset : _standCameraY;

      _controller.height = Mathf.Lerp(_controller.height, targetHeight, crouchLerpSpeed * Time.deltaTime);

      Vector3 center = _controller.center;
      center.y = Mathf.Lerp(
        center.y,
        (_standCenter.y / _standHeight) * _controller.height,
        crouchLerpSpeed * Time.deltaTime);
      _controller.center = center;

      Vector3 camPos = cameraRoot.localPosition;
      camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchLerpSpeed * Time.deltaTime);
      cameraRoot.localPosition = camPos;
    }
  }
}
