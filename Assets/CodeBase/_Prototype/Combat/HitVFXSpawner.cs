// Assets/CodeBase/_Prototype/Combat/HitVFXSpawner.cs
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class HitVFXSpawner : MonoBehaviour
  {
    [Header("Blood Impact")]
    [SerializeField] ParticleSystem bloodImpactPrefab;

    public void Spawn(Vector3 hitPoint, Vector3 hitNormal)
    {
      if (bloodImpactPrefab == null)
        return;

      var fx = Instantiate(
        bloodImpactPrefab,
        hitPoint,
        Quaternion.LookRotation(hitNormal)
      );
      fx.Play();

      var main = fx.main;
      float life = main.duration;
      if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
        life += main.startLifetime.constantMax;
      else
        life += main.startLifetime.constant;

      Destroy(fx.gameObject, life);
    }
  }
}