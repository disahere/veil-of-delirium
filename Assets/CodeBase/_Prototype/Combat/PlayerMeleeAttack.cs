// Assets/CodeBase/Prototype/Combat/PlayerMeleeAttack.cs
using CodeBase._Prototype.CameraEffect;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CodeBase._Prototype.Combat
{
  public class PlayerMeleeAttack : MonoBehaviour
  {
    [Header("Refs")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Weapon weapon;
    [SerializeField] Slider chargeSlider;

    VeilInputActions _input;
    InputAction _attackAction;
    CameraEffects _cameraEffects;

    float _pressStartTime;
    bool _isPressing;

    void Awake()
    {
      _input = new VeilInputActions();
      _attackAction = _input.Player.Attack;

      _cameraEffects = playerCamera != null 
        ? playerCamera.GetComponent<CameraEffects>() 
        : null;
      
      if (chargeSlider != null)
      {
        chargeSlider.minValue = 0f;
        chargeSlider.maxValue = 1f;
        chargeSlider.value = 0f;
      }
    }

    void OnEnable()
    {
      _input.Player.Enable();
      _attackAction.started += OnAttackStarted;
      _attackAction.canceled += OnAttackCanceled;
    }

    void OnDisable()
    {
      _attackAction.started -= OnAttackStarted;
      _attackAction.canceled -= OnAttackCanceled;
      _input.Player.Disable();
    }

    void OnDestroy()
    {
      _input.Dispose();
      _input = null;
    }

    void Update()
    {
      if (!_isPressing || chargeSlider == null || weapon == null)
        return;
      
      float duration = Time.time - _pressStartTime;
      float t = duration / weapon.ChargeThreshold;
      chargeSlider.value = Mathf.Clamp01(t);
    }

    void OnAttackStarted(InputAction.CallbackContext ctx)
    {
      _pressStartTime = Time.time;
      _isPressing = true;

      if (chargeSlider != null)
        chargeSlider.value = 0f;
    }

    void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (!_isPressing)
        return;

      _isPressing = false;

      if (chargeSlider != null)
        chargeSlider.value = 0f;

      if (weapon == null || !weapon.CanAttack())
        return;

      float pressDuration = Time.time - _pressStartTime;
      bool isHeavy = pressDuration >= weapon.ChargeThreshold;

      DoAttack(isHeavy);
      weapon.CommitAttackUse();
    }

    void DoAttack(bool isHeavy)
    {
      if (playerCamera == null)
        return;

      float damage = weapon.RollDamage(isHeavy);
      float range = weapon.AttackRange;

      Vector3 origin = playerCamera.transform.position;
      Vector3 direction = playerCamera.transform.forward;

      if (Physics.Raycast(origin, direction, out var hit, range))
      {
        if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
          damageable.TakeDamage(damage);
        
        if (isHeavy)
        {
          var vfx = hit.collider.GetComponentInParent<HitVFXSpawner>();
          if (vfx != null)
            vfx.Spawn(hit.point, hit.normal);
        }
      }

      if (_cameraEffects != null)
      {
        if (isHeavy)
          _cameraEffects.PlayHeavyShake();
        else
          _cameraEffects.PlayLightShake();
      }
    }
  }
}
