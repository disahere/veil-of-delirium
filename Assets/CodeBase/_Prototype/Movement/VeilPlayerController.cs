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
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    CharacterController _controller;
    VeilInputActions _input;
    Vector2 _moveInput;
    Vector2 _lookInput;
    float _pitch;

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
        _input.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += _ => _lookInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
      
      Vector3 worldInput = new Vector3(_moveInput.x, 0f, _moveInput.y);
      if (worldInput.sqrMagnitude > 1f)
        worldInput.Normalize();
      
      Vector3 localMove = transform.TransformDirection(worldInput);
      Vector3 move = localMove * moveSpeed * Runner.DeltaTime;

      _controller.Move(move);
    }

    void Update()
    {
      if (!Object.HasInputAuthority)
        return;

      HandleLook();
    }

    void HandleLook()
    {
      if (cameraRoot == null)
        return;

      float mouseX = _lookInput.x * mouseSensitivity;
      float mouseY = _lookInput.y * mouseSensitivity;
      
      transform.Rotate(Vector3.up, mouseX);
      
      _pitch -= mouseY;
      _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

      Vector3 euler = cameraRoot.localEulerAngles;
      euler.x = _pitch;
      cameraRoot.localEulerAngles = euler;
    }
  }
}
