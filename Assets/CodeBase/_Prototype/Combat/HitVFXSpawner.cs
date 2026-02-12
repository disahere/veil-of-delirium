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
    }
  }
}