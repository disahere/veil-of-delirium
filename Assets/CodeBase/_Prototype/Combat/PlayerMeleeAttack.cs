// Assets/CodeBase/Prototype/Combat/PlayerMeleeAttack.cs
using CodeBase._Prototype.CameraEffect;
using CodeBase._Prototype.Enemies;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CodeBase._Prototype.Combat
{
  public class PlayerMeleeAttack : NetworkBehaviour
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

    public override void Spawned()
    {
      if (Object.HasInputAuthority)
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

        _input.Player.Enable();
        _attackAction.started += OnAttackStarted;
        _attackAction.canceled += OnAttackCanceled;
      }
    }
    
    void OnDisable()
    {
      if (_input != null)
      {
        _attackAction.started -= OnAttackStarted;
        _attackAction.canceled -= OnAttackCanceled;
        _input.Player.Disable();
      }
    }

    void OnDestroy()
    {
      if (_input != null)
      {
        _input.Dispose();
        _input = null;
      }
    }

    void Update()
    {
      if (!Object.HasInputAuthority)
        return;

      if (!_isPressing || chargeSlider == null || weapon == null)
        return;

      float duration = Time.time - _pressStartTime;
      float t = duration / weapon.ChargeThreshold;
      chargeSlider.value = Mathf.Clamp01(t);
    }

    void OnAttackStarted(InputAction.CallbackContext ctx)
    {
      if (!Object.HasInputAuthority)
        return;

      _pressStartTime = Time.time;
      _isPressing = true;

      if (chargeSlider != null)
        chargeSlider.value = 0f;
    }

    void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (!Object.HasInputAuthority)
        return;

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
      if (!Object.HasInputAuthority)
        return;

      if (playerCamera == null)
        return;

      float damage = weapon.RollDamage(isHeavy);
      float range = weapon.AttackRange;

      Vector3 origin = playerCamera.transform.position;
      Vector3 direction = playerCamera.transform.forward;

      if (Physics.Raycast(origin, direction, out var hit, range))
      {
        if (hit.collider.TryGetComponent<Enemy>(out var enemy))
        {
          enemy.ApplyMeleeHitRpc(damage, isHeavy, hit.point, hit.normal);
        }
      }

      if (_cameraEffects != null && _cameraEffects.isActiveAndEnabled)
      {
        if (isHeavy)
          _cameraEffects.PlayHeavyShake();
        else
          _cameraEffects.PlayLightShake();
      }
    }
  }
}
