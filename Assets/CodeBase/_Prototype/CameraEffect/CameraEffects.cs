// Assets/CodeBase/Prototype/Camera/CameraEffects.cs
using System.Collections;
using UnityEngine;

namespace CodeBase._Prototype.CameraEffect
{
  public class CameraEffects : MonoBehaviour
  {
    [Header("Head Bob")]
    [SerializeField] Transform cameraRoot;
    [SerializeField] float bobSpeed = 6f;
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] float bobSmoothing = 8f;

    [Header("Shake")]
    [SerializeField] float lightShakeDuration = 0.08f;
    [SerializeField] float lightShakeAmplitude = 0.05f;
    [SerializeField] float heavyShakeDuration = 0.12f;
    [SerializeField] float heavyShakeAmplitude = 0.1f;

    Vector3 _defaultLocalPos;
    float _bobTimer;

    Vector3 _shakeOffset;
    Coroutine _shakeRoutine;

    void Awake()
    {
      if (cameraRoot == null)
        cameraRoot = transform;

      _defaultLocalPos = cameraRoot.localPosition;
    }

    void Update()
    {
      UpdateHeadBob();
      ApplyOffsets();
    }
    
    public void SetMoveAmount(float normalizedMoveAmount)
    {
      if (normalizedMoveAmount > 0.01f)
        _bobTimer += Time.deltaTime * bobSpeed * normalizedMoveAmount;
      else
        _bobTimer = Mathf.Lerp(_bobTimer, 0f, Time.deltaTime * bobSmoothing);
    }

    void UpdateHeadBob()
    {
      float sin = Mathf.Sin(_bobTimer);
      float bobOffsetY = sin * bobAmount;

      Vector3 target = _defaultLocalPos + new Vector3(0f, bobOffsetY, 0f);
      cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, target, Time.deltaTime * bobSmoothing);
    }

    void ApplyOffsets()
    {
      cameraRoot.localPosition += _shakeOffset;
    }

    public void PlayLightShake()
    {
      PlayShake(lightShakeDuration, lightShakeAmplitude);
    }

    public void PlayHeavyShake()
    {
      PlayShake(heavyShakeDuration, heavyShakeAmplitude);
    }

    void PlayShake(float duration, float amplitude)
    {
      if (_shakeRoutine != null)
        StopCoroutine(_shakeRoutine);

      _shakeRoutine = StartCoroutine(ShakeRoutine(duration, amplitude));
    }

    IEnumerator ShakeRoutine(float duration, float amplitude)
    {
      float timer = 0f;

      while (timer < duration)
      {
        timer += Time.deltaTime;
        float t = 1f - (timer / duration);

        _shakeOffset = Random.insideUnitSphere * (amplitude * t);
        yield return null;
      }

      _shakeOffset = Vector3.zero;
      _shakeRoutine = null;
    }
  }
}
