using RPG.Attributes;
using RPG.Saving;
using UnityEngine;

namespace RPG.Combat {
  public class WeaponPickup : MonoBehaviour, ISaveable {

    [SerializeField] WeaponConfig weapon;
    [SerializeField] float healthToRestore = 0f;
    [SerializeField] float respawnTime = 5f;

    private float timeSinceHidden = Mathf.Infinity;

    public object CaptureState() {
      return timeSinceHidden;
    }

    public void RestoreState(object state) {
      timeSinceHidden = (float) state;
      if (timeSinceHidden < respawnTime) {
        ShowPickup(false);
      }
    }

    private void Update() {
      timeSinceHidden += Time.deltaTime;
      if (timeSinceHidden >= respawnTime) {
        ShowPickup(true);
      }
    }

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
      timeSinceHidden = 0f;
      ShowPickup(false);
    }

    private void ShowPickup(bool shouldShow) {
      GetComponent<Collider>().enabled = shouldShow;
      foreach (Transform child in transform) {
        child.gameObject.SetActive(shouldShow);
      }
    }
  }
}