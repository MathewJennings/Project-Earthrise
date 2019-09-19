using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
  public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {

    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] Transform leftHand = null;
    [SerializeField] Transform rightHand = null;
    [SerializeField] WeaponConfig defaultWeaponConfig = null;

    private Health target;
    float timeSinceLastAttack = Mathf.Infinity;
    WeaponConfig currentWeaponConfig;
    LazyValue<Weapon> currentWeapon;

    private void Awake() {
      currentWeaponConfig = defaultWeaponConfig;
      currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
    }

    private Weapon SetupDefaultWeapon() {
      return AttachWeapon(defaultWeaponConfig);
    }

    private void Start() {
      currentWeapon.ForceInit();
    }

    private void Update() {
      timeSinceLastAttack += Time.deltaTime;

      if (target == null) return;
      if (target.IsDead()) return;

      if (!IsInRange()) {
        GetComponent<Mover>().MoveTo(target.transform.position, 1f);
      } else {
        GetComponent<Mover>().Cancel();
        AttackBehavior();
      }
    }

    public void EquipWeapon(WeaponConfig weapon) {
      currentWeaponConfig = weapon;
      currentWeapon.value = AttachWeapon(weapon);
    }

    private Weapon AttachWeapon(WeaponConfig weapon) {
      Animator animator = GetComponent<Animator>();
      return weapon.Spawn(rightHand, leftHand, animator);
    }

    public bool CanAttack(GameObject target) {
      if (target == null) return false;
      if (!GetComponent<Mover>().CanMoveTo(target.transform.position)) return false;

      Health targetToCheck = target.GetComponent<Health>();
      return targetToCheck != null && !targetToCheck.IsDead();
    }

    // Player attacks without a specific target
    public void Attack() {
      if (!GetComponent<Energy>().CanConsumeEnergy(currentWeaponConfig.GetEnergyToAttack())) return;

      StartCoroutine(GetComponent<Mover>().RotateAsynchronously(Camera.main.transform.forward));
      GetComponent<ActionScheduler>().StartAction(this);
      if (!isAnAttackQueued()) {
        GetComponent<Energy>().ConsumeEnergy(currentWeaponConfig.GetEnergyToAttack());
      }
      TriggerAttackAnimation();
    }

    // Enemies attack specific targets
    public void Attack(GameObject target) {
      GetComponent<ActionScheduler>().StartAction(this);
      this.target = target.GetComponent<Health>();
    }

    public void Cancel() {
      target = null;
      StopAttackAnimation();
      GetComponent<Mover>().Cancel();
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
      if (stat == Stat.Damage) {
        yield return currentWeaponConfig.getDamage();
      }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat) {
      if (stat == Stat.Damage) {
        yield return currentWeaponConfig.getPercentageBonus();
      }
    }

    public object CaptureState() {
      if (currentWeaponConfig == null) {
        EquipWeapon(defaultWeaponConfig);
      }
      return currentWeaponConfig.name;
    }

    public void RestoreState(object state) {
      string weaponName = (string) state;
      WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
      EquipWeapon(weapon);
    }

    private bool IsInRange() {
      return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.getRange();
    }

    private void AttackBehavior() {
      transform.LookAt(target.transform);
      if (timeSinceLastAttack >= timeBetweenAttacks) {
        TriggerAttackAnimation();
        timeSinceLastAttack = 0;
      }
    }

    private bool isAnAttackQueued() {
      return GetComponent<Animator>().GetBool("attack");
    }

    private void TriggerAttackAnimation() {
      GetComponent<Animator>().ResetTrigger("stopAttack");
      // This will trigger the Hit() event.
      GetComponent<Animator>().SetTrigger("attack");
    }

    private void StopAttackAnimation() {
      GetComponent<Animator>().ResetTrigger("attack");
      GetComponent<Animator>().SetTrigger("stopAttack");
    }

    // Animation Event
    private void Shoot() {
      Hit();
    }

    // Animation Event
    private void Hit() {

      Health targetToHit = GetTargetToHit();
      float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
      if (currentWeaponConfig.HasProjectile() && GetComponent<ActionScheduler>().GetCurrentAction() is Fighter) { // Make Movement interrupt launching projectiles
        currentWeaponConfig.LaunchProjectile(rightHand, leftHand, targetToHit, this.gameObject, damage);
      }

      Energy energyComponent = GetComponent<Energy>();
      if (energyComponent != null) energyComponent.StopConsumingEnergy();

      if (targetToHit == null) return;

      if (currentWeapon.value != null) {
        currentWeapon.value.OnHit(targetToHit);
      }
      if (!currentWeaponConfig.HasProjectile()) {
        targetToHit.TakeDamage(this.gameObject, damage);
      }
    }

    private Health GetTargetToHit() {
      if (this.CompareTag("Player")) {
        List<CombatTarget> hittableEnemies = currentWeapon.value.GetCollidingCombatTargets();
        if (hittableEnemies.Count == 0) return null;
        hittableEnemies.Sort(
          (x, y) => Vector3.Distance(currentWeapon.value.transform.position, x.GetComponent<Health>().GetHitLocation())
          .CompareTo(Vector3.Distance(currentWeapon.value.transform.position, y.GetComponent<Health>().GetHitLocation()))
        );
        Health candidate = hittableEnemies[0].GetComponent<Health>();
        if (CanAttack(candidate.gameObject)) return candidate;
        return null;
      } else {
        return target;
      }
    }
  }
}