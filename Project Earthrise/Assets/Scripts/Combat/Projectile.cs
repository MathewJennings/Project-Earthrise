using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
  public class Projectile : MonoBehaviour {

    [SerializeField] float speed = 15f;
    [SerializeField] bool isHoming = false;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] float maxLifetime = 10f;
    [SerializeField] GameObject[] destroyOnHit = null;
    [SerializeField] float lifeAfterImpact = 0.2f;
    [SerializeField] UnityEvent onHit;

    private Health target = null;
    GameObject instigator = null;
    private float damage = 0f;

    private void Start() {
      if (target == null) {
        transform.LookAt(transform.position + instigator.transform.forward);
      } else {
        transform.LookAt(target.GetHitLocation());
      }
      Destroy(this.gameObject, maxLifetime);
    }

    private void Update() {
      if (isHoming && target != null && !target.IsDead()) {
        transform.LookAt(target.GetHitLocation());
      }
      transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public void SetTarget(Health target, GameObject instigator, float damage) {
      this.target = target;
      this.instigator = instigator;
      this.damage = damage;
    }

    private void OnTriggerEnter(Collider other) {
      if (target != null) {
        if (other.GetComponent<Health>() != target) return;
        if (target.IsDead()) return;
        target.TakeDamage(instigator, damage);
        if (hitEffect != null) {
          Instantiate(hitEffect, target.GetHitLocation(), transform.rotation);
        }
      } else {
        Health hitCharacter = other.GetComponent<Health>();
        if (hitCharacter == null || hitCharacter.IsDead() || hitCharacter.GetComponent<CombatTarget>() == null) return;
        hitCharacter.TakeDamage(instigator, damage);
        if (hitEffect != null) {
          Instantiate(hitEffect, hitCharacter.GetHitLocation(), transform.rotation);
        }
      }
      onHit.Invoke();
      speed = 0f;
      foreach (GameObject toDestroy in destroyOnHit) {
        Destroy(toDestroy);
      }
      Destroy(this.gameObject, lifeAfterImpact);
    }
  }
}