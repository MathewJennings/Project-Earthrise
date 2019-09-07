using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
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

    public Health GetTarget() {
      return target;
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
      Health targetToCheck = target.GetComponent<Health>();
      return targetToCheck != null && !targetToCheck.IsDead();
    }

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
    void Shoot() {
      Hit();
    }

    // Animation Event
    void Hit() {
      if (target == null) return;

      float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

      if (currentWeapon.value != null) {
        currentWeapon.value.OnHit(target);
      }

      if (currentWeaponConfig.HasProjectile()) {
        currentWeaponConfig.LaunchProjectile(rightHand, leftHand, target, this.gameObject, damage);
      } else {
        target.TakeDamage(this.gameObject, damage);
      }
    }
  }
}