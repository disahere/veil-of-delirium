// Assets/CodeBase/_Prototype/Combat/Health.cs
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class Health : MonoBehaviour, IDamageable
  {
    [SerializeField] float maxHealth = 100f;

    float _current;

    void Awake()
    {
      _current = maxHealth;
    }

    public void TakeDamage(float amount)
    {
      if (_current <= 0f)
        return;

      _current -= amount;
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