using Fusion;
using UnityEngine;

namespace CodeBase._Prototype.Movement
{
  public class VeilPlayerController : NetworkBehaviour
  {
    [SerializeField] float moveSpeed = 5f;

    CharacterController _controller;
    VeilInputActions _input;
    Vector2 _moveInput;

    public override void Spawned()
    {
      _controller = GetComponent<CharacterController>();

      if (Object.HasInputAuthority)
      {
        _input = new VeilInputActions();
        _input.Player.Enable();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled  += _ => _moveInput = Vector2.zero;
      }
    }

    void OnDestroy()
    {
      if (_input != null)
      {
        _input.Player.Disable();
        _input.Dispose();
      }
    }

    void Update()
    {
      if (!Object.HasInputAuthority) return;

      Vector3 input = new Vector3(_moveInput.x, 0f, _moveInput.y);
      if (input.sqrMagnitude > 1f)
        input.Normalize();

      Vector3 move = transform.TransformDirection(input) * moveSpeed;
      _controller.Move(move * Time.deltaTime);
    }
  }
}