// Assets/CodeBase/_Prototype/Combat/WeaponSway.cs
using UnityEngine;
using CodeBase._Prototype.Movement;

namespace CodeBase._Prototype.Combat
{
  public class WeaponSway : MonoBehaviour
  {
    [Header("Refs")]
    [SerializeField] VeilPlayerController playerController;

    [Header("Position Sway")]
    [SerializeField] float posSwayAmount = 0.02f;
    [SerializeField] float posMaxOffset = 0.06f;
    [SerializeField] float posSmooth = 8f;

    [Header("Rotation Sway")]
    [SerializeField] float rotSwayAmount = 2f;
    [SerializeField] float rotMaxAngle = 5f;
    [SerializeField] float rotSmooth = 10f;

    Vector3 _initialLocalPos;
    Quaternion _initialLocalRot;

    void Awake()
    {
      _initialLocalPos = transform.localPosition;
      _initialLocalRot = transform.localRotation;
    }

    void LateUpdate()
    {
      if (playerController == null)
        return;

      Vector2 look = playerController.LookInput;
      
      float mouseX = look.x * Time.deltaTime;
      float mouseY = look.y * Time.deltaTime;

      DoPositionSway(mouseX, mouseY);
      DoRotationSway(mouseX, mouseY);
    }

    void DoPositionSway(float mouseX, float mouseY)
    {
      float offsetX = -mouseX * posSwayAmount;
      float offsetY = -mouseY * posSwayAmount;

      offsetX = Mathf.Clamp(offsetX, -posMaxOffset, posMaxOffset);
      offsetY = Mathf.Clamp(offsetY, -posMaxOffset, posMaxOffset);

      Vector3 target = _initialLocalPos + new Vector3(offsetX, offsetY, 0f);

      transform.localPosition = Vector3.Lerp(
        transform.localPosition,
        target,
        posSmooth * Time.deltaTime);
    }

    void DoRotationSway(float mouseX, float mouseY)
    {
      float rotX = mouseY * rotSwayAmount;
      float rotY = -mouseX * rotSwayAmount;

      rotX = Mathf.Clamp(rotX, -rotMaxAngle, rotMaxAngle);
      rotY = Mathf.Clamp(rotY, -rotMaxAngle, rotMaxAngle);

      Quaternion targetRot = _initialLocalRot *
                             Quaternion.Euler(rotX, rotY, 0f);

      transform.localRotation = Quaternion.Lerp(
        transform.localRotation,
        targetRot,
        rotSmooth * Time.deltaTime);
    }
  }
}
