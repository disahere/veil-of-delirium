// Assets/CodeBase/_Prototype/Enemies/Enemy.cs
using CodeBase._Prototype.Combat;
using Fusion;
using UnityEngine;

namespace CodeBase._Prototype.Enemies
{
  [RequireComponent(typeof(Health))]
  [RequireComponent(typeof(EnemyHitFeedback))]
  public class Enemy : NetworkBehaviour
  {
    [SerializeField] EnemyConfig config;

    Health _health;
    EnemyHitFeedback _hitFeedback;

    [Networked] public float NetHealth { get; private set; }

    public EnemyConfig Config => config;
    public EnemyType Type => config != null ? config.enemyType : EnemyType.Dummy;

    public override void Spawned()
    {
      _health = GetComponent<Health>();
      _hitFeedback = GetComponent<EnemyHitFeedback>();

      float maxHp = config != null ? config.maxHealth : _health.MaxHealth;
      NetHealth = maxHp;

      _health.SetMaxHealth(maxHp);
      _health.SetCurrent(NetHealth);

      if (config != null)
        _hitFeedback.SetHitColor(config.hitTintColor);

      Debug.Log($"Enemy.Spawned on {Runner.LocalPlayer}, StateAuth={Object.HasStateAuthority}, InputAuth={Object.HasInputAuthority}");
    }

    void ApplyDamage(float amount, bool isHeavy, Vector3 hitPos, Vector3 hitNormal)
    {
      if (NetHealth <= 0f)
        return;

      NetHealth = Mathf.Max(0f, NetHealth - amount);
      _health.SetCurrent(NetHealth);
      
      PlayHitEffectsRpc(isHeavy, hitPos, hitNormal);

      if (NetHealth <= 0f)
      {
        NetHealth = 0f;
        
        Object.Runner.Despawn(Object);
      }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void PlayHitEffectsRpc(bool isHeavy, Vector3 hitPos, Vector3 hitNormal)
    {
      _health?.PlayHitFeedback();
      
      if (isHeavy)
      {
        var vfx = GetComponentInChildren<HitVFXSpawner>();
        if (vfx != null)
          vfx.Spawn(hitPos, hitNormal);
      }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ApplyMeleeHitRpc(float damage, bool isHeavy, Vector3 hitPos, Vector3 hitNormal)
    {
      if (_health == null)
        return;

      ApplyDamage(damage, isHeavy, hitPos, hitNormal);
    }
  }
}
