// Assets/CodeBase/_Prototype/Enemies/EnemyConfig.cs
using UnityEngine;

namespace CodeBase._Prototype.Enemies
{
  [CreateAssetMenu(
    fileName = "EnemyConfig",
    menuName = "Veil/Enemies/Enemy Config")]
  public class EnemyConfig : ScriptableObject
  {
    [Header("Base")]
    public EnemyType enemyType = EnemyType.Dummy;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRadius = 8f;
    public float attackRadius = 1.8f;

    [Header("Combat")]
    public float damagePerHit = 10f;
    public float attacksPerSecond = 0.8f;

    [Header("FX")]
    public Color hitTintColor = Color.red;
  }
}