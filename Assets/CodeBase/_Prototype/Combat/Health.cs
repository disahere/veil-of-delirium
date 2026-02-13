// Assets/CodeBase/_Prototype/Combat/Health.cs
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class Health : MonoBehaviour, IDamageable
  {
    float _maxHealth = 100f;

    float _current;
    EnemyHitFeedback _hitFeedback;

    public float MaxHealth => _maxHealth;
    public float Current => _current;

    void Awake()
    {
      _current = _maxHealth;
      _hitFeedback = GetComponent<EnemyHitFeedback>();
    }

    public void SetMaxHealth(float value)
    {
      _maxHealth = Mathf.Max(1f, value);
      _current = _maxHealth;
    }

    public void SetCurrent(float value)
    {
      _current = Mathf.Max(0f, value);
      _hitFeedback?.Play();
    }

    public void PlayHitFeedback()
    {
      _hitFeedback?.Play();
    }
    
    public void TakeDamage(float amount)
    {
      if (_current <= 0f)
        return;

      _current -= amount;
      _hitFeedback?.Play();

      if (_current <= 0f)
      {
        _current = 0f;
        OnDeath();
      }
    }

    void OnDeath()
    {
      gameObject.SetActive(false);
    }
  }
}