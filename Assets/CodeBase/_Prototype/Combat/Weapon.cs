// Assets/CodeBase/Prototype/Combat/Weapon.cs
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class Weapon : MonoBehaviour
  {
    [SerializeField] WeaponConfig config;

    float _nextAttackTime;

    public WeaponType Type => config != null ? config.weaponType : WeaponType.Melee;

    public float AttackRange => config != null ? config.attackRange : 2.5f;

    public float ChargeThreshold => config != null ? config.chargeThreshold : 0.4f;

    public bool CanAttack()
    {
      return Time.time >= _nextAttackTime;
    }

    public float GetCooldown()
    {
      if (config == null || config.attacksPerSecond <= 0f)
        return 0.5f;

      return 1f / config.attacksPerSecond;
    }

    public float RollDamage(bool isHeavy)
    {
      if (config == null)
        return 10f;

      float baseDamage = Random.Range(config.lightDamageMin, config.lightDamageMax);
      if (isHeavy)
        baseDamage *= config.heavyDamageMultiplier;

      return baseDamage;
    }

    public void CommitAttackUse()
    {
      _nextAttackTime = Time.time + GetCooldown();
    }
  }
}