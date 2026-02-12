// Assets/CodeBase/_Prototype/Combat/Health.cs
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class Health : MonoBehaviour, IDamageable
  {
    [SerializeField] float maxHealth = 100f;

    float _current;
    EnemyHitFeedback _hitFeedback;

    void Awake()
    {
      _current = maxHealth;
      _hitFeedback = GetComponent<EnemyHitFeedback>();
    }

    public void SetMaxHealth(float value)
    {
      maxHealth = Mathf.Max(1f, value);
      _current = maxHealth;
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