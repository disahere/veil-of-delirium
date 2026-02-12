using UnityEngine;
using UnityEngine.InputSystem;

namespace CodeBase._Prototype.Combat
{
  public class PlayerMeleeAttack : MonoBehaviour
  {
    [Header("Refs")]
    [SerializeField] Camera playerCamera;

    [Header("Attack")]
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 2.5f;
    [SerializeField] float cooldown = 0.3f;

    VeilInputActions _input;
    InputAction _attack;
    float _nextAttackTime;

    void Awake()
    {
      _input = new VeilInputActions();
      _attack = _input.Player.Attack;
    }

    void OnEnable()
    {
      _input.Player.Enable();
      _attack.performed += OnAttackPerformed;
    }

    void OnDisable()
    {
      _attack.performed -= OnAttackPerformed;
      _input.Player.Disable();
    }

    void OnDestroy()
    {
      _input.Dispose();
      _input = null;
    }

    void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
      if (Time.time < _nextAttackTime)
        return;

      DoAttack();
      _nextAttackTime = Time.time + cooldown;
    }

    void DoAttack()
    {
      if (playerCamera == null)
        return;

      Vector3 origin = playerCamera.transform.position;
      Vector3 direction = playerCamera.transform.forward;

      if (Physics.Raycast(origin, direction, out var hit, range))
      {
        if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
        {
          damageable.TakeDamage(damage);
        }
      }
    }
  }
}