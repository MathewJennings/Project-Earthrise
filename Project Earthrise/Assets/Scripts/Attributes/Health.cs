using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes {
  public class Health : MonoBehaviour, ISaveable {

    [SerializeField] float regenerationPercentage = 70f;
    [SerializeField] UnityEvent onDie;
    [SerializeField] public TakeDamageEvent takeDamageUnityEvent;
    [System.Serializable]
    public class TakeDamageEvent : UnityEvent<float> { }

    LazyValue<float> healthPoints;
    private bool isDead;

    private void OnEnable() {
      GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
    }

    private void OnDisable() {
      GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
    }

    private void Awake() {
      healthPoints = new LazyValue<float>(GetInitialHealth);
    }

    private float GetInitialHealth() {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void Start() {
      healthPoints.ForceInit();
    }

    public float GetHealthPoints() {
      return healthPoints.value;
    }

    public float GetMaxHealthPoints() {
      return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    public float GetFraction() {
      return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    public float GetPercentage() {
      return 100 * GetFraction();
    }

    public bool IsDead() {
      return isDead;
    }

    public void TakeDamage(GameObject instigator, float damage) {
      healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
      takeDamageUnityEvent.Invoke(damage);
      if (healthPoints.value == 0 && !isDead) {
        onDie.Invoke();
        Die();
        AwardExperience(instigator);
      }
    }

    public Vector3 GetHitLocation() {
      CapsuleCollider capsule = GetComponent<CapsuleCollider>();
      if (capsule == null) {
        return transform.position;
      } else {
        return transform.position + Vector3.up * capsule.height / 2;
      }
    }

    public void Heal(float healthToRestore) {
      healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetComponent<BaseStats>().GetStat(Stat.Health));
    }

    public object CaptureState() {
      return healthPoints.value;
    }

    public void RestoreState(object state) {
      healthPoints.value = (float) state;
      if (healthPoints.value <= 0) {
        Die();
      } else if (isDead) {
        Revive();
      }
    }

    private void AwardExperience(GameObject instigator) {
      Experience instigatorExperience = instigator.GetComponent<Experience>();
      if (instigatorExperience == null) return;
      instigatorExperience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
    }

    private void RegenerateHealth(int newLevel) {
      float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
      healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
    }

    private void Die() {
      isDead = true;
      GetComponent<Animator>().SetTrigger("die");
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void Revive() {
      isDead = false;
      GetComponent<Animator>().SetTrigger("revive");
    }
  }
}