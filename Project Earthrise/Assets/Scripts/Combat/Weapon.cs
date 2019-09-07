using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
  public class Weapon : MonoBehaviour {

    [SerializeField] UnityEvent onHit;
    [SerializeField] GameObject hitEffect = null;

    public void OnHit(Health target) {
      print("Hit!");
      onHit.Invoke();
      if (hitEffect != null) {
        Instantiate(hitEffect, GetHitLocation(target), target.transform.rotation);
      }
    }

    private Vector3 GetHitLocation(Health target) {
      CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
      if (targetCapsule == null) {
        return target.transform.position;
      } else {
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
      }
    }
  }
}