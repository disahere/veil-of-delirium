// Assets/CodeBase/_Prototype/Combat/EnemyHitFeedback.cs
using System.Collections;
using UnityEngine;

namespace CodeBase._Prototype.Combat
{
  public class EnemyHitFeedback : MonoBehaviour
  {
    [Header("Scale")]
    [SerializeField] Transform target;
    [SerializeField] float scaleFactor = 0.9f;
    [SerializeField] float scaleDuration = 0.1f;

    [Header("Color")]
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float colorDuration = 0.1f;

    Vector3 _originalScale;
    Color _originalColor;
    Coroutine _routine;

    void Awake()
    {
      if (target == null)
        target = transform;

      _originalScale = target.localScale;

      if (targetRenderer != null)
        _originalColor = targetRenderer.material.color;
    }

    public void SetHitColor(Color color)
    {
      hitColor = color;
    }

    public void Play()
    {
      if (_routine != null)
        StopCoroutine(_routine);

      _routine = StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
      float t = 0f;

      Vector3 hitScale = _originalScale * scaleFactor;
      if (targetRenderer != null)
        targetRenderer.material.color = hitColor;

      while (t < 1f)
      {
        t += Time.deltaTime / Mathf.Max(scaleDuration, colorDuration);

        float lerpScaleT = Mathf.Clamp01(t);
        float lerpColorT = Mathf.Clamp01(t);

        target.localScale = Vector3.Lerp(hitScale, _originalScale, lerpScaleT);

        if (targetRenderer != null)
        {
          Color c = Color.Lerp(hitColor, _originalColor, lerpColorT);
          targetRenderer.material.color = c;
        }

        yield return null;
      }

      target.localScale = _originalScale;
      if (targetRenderer != null)
        targetRenderer.material.color = _originalColor;

      _routine = null;
    }
  }
}