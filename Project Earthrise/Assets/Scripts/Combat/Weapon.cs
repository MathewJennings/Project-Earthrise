using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
  public class Weapon : MonoBehaviour {

    [SerializeField] public UnityEvent onHit;
    [SerializeField] GameObject hitEffect = null;

    List<CombatTarget> collidingTargets = new List<CombatTarget>();

    public List<CombatTarget> GetCollidingCombatTargets() {
      return collidingTargets;
    }

    public void OnHit(Health target) {
      onHit.Invoke();
      if (hitEffect != null) {
        Instantiate(hitEffect, target.GetHitLocation(), target.transform.rotation);
      }
    }

    private void OnTriggerEnter(Collider other) {
      CombatTarget enemy = other.GetComponent<CombatTarget>();
      if (enemy != null) {
        collidingTargets.Add(enemy);
      }
    }

    private void OnTriggerExit(Collider other) {
      CombatTarget enemy = other.GetComponent<CombatTarget>();
      if (enemy != null) {
        collidingTargets.Remove(enemy);
      }
    }
  }
}