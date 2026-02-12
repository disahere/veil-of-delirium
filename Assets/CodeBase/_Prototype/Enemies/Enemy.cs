// Assets/CodeBase/_Prototype/Enemies/Enemy.cs
using CodeBase._Prototype.Combat;
using UnityEngine;

namespace CodeBase._Prototype.Enemies
{
  [RequireComponent(typeof(Health))]
  [RequireComponent(typeof(EnemyHitFeedback))]
  public class Enemy : MonoBehaviour
  {
    [SerializeField] EnemyConfig config;

    Health _health;
    EnemyHitFeedback _hitFeedback;

    void Awake()
    {
      _health = GetComponent<Health>();
      _hitFeedback = GetComponent<EnemyHitFeedback>();

      if (config != null)
      {
        _health.SetMaxHealth(config.maxHealth);
        
        _hitFeedback.SetHitColor(config.hitTintColor);
      }
    }

    public EnemyConfig Config => config;
    public EnemyType Type => config != null ? config.enemyType : EnemyType.Dummy;
  }
}