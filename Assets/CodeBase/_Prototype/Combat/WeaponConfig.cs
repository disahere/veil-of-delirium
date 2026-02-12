// Assets/CodeBase/Prototype/Combat/WeaponConfig.cs

using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  [CreateAssetMenu(
    fileName = "WeaponConfig",
    menuName = "Veil/Combat/Weapon Config")]
  public class WeaponConfig : ScriptableObject
  {
    [Header("Base")]
    public WeaponType weaponType = WeaponType.Melee;

    [Header("Melee Damage")]
    public float lightDamageMin = 10f;
    public float lightDamageMax = 20f;

    [Tooltip("Multiplier for heavy attack damage relative to light.")]
    public float heavyDamageMultiplier = 2f;

    [Header("Melee Range / Tempo")]
    public float attackRange = 2.5f;

    [Tooltip("Attacks per second (1 / value = cooldown).")]
    public float attacksPerSecond = 2f;

    [Tooltip("Hold time in seconds to treat attack as heavy.")]
    public float chargeThreshold = 0.4f;
    
    [Header("Ranged (WIP)")]
    public float projectileSpeed = 20f;
  }
}