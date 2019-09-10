using System.Collections;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
  public class WeaponPickup : MonoBehaviour {

    [SerializeField] WeaponConfig weapon;
    [SerializeField] float healthToRestore = 0f;
    [SerializeField] float respawnTime = 5f;

    private void OnTriggerEnter(Collider other) {
      if (other.CompareTag("Player")) {
        Pickup(other.gameObject);
      }
    }

    private void Pickup(GameObject subject) {
      if (weapon != null) {
        subject.GetComponent<Fighter>().EquipWeapon(weapon);
      }
      if (healthToRestore > 0) {
        subject.GetComponent<Health>().Heal(healthToRestore);
      }
      StartCoroutine(HideForSeconds(respawnTime));
    }

    private IEnumerator HideForSeconds(float seconds) {
      ShowPickup(false);
      yield return new WaitForSeconds(seconds);
      ShowPickup(true);
    }

    private void ShowPickup(bool shouldShow) {
      GetComponent<Collider>().enabled = shouldShow;
      foreach (Transform child in transform) {
        child.gameObject.SetActive(shouldShow);
      }
    }
  }
}