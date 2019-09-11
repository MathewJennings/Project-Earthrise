using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
  [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
  public class WeaponConfig : ScriptableObject {
    [SerializeField] Weapon equippedPrefab = null;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] float damage = 5f;
    [SerializeField] float percentageBonus = 0f;
    [SerializeField] float range = 2f;
    [SerializeField] bool isRightHanded = true;
    [SerializeField] Projectile projectile = null;
    [SerializeField] float energyToAttack = 0f;

    private const string weaponName = "Weapon";

    public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) {
      DestroyOldWeapon(rightHand, leftHand);

      Weapon weapon = null;
      if (equippedPrefab != null) {
        weapon = Instantiate(equippedPrefab, GetHand(rightHand, leftHand));
        weapon.gameObject.name = weaponName;
      }
      var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
      if (animatorOverride != null) {
        animator.runtimeAnimatorController = animatorOverride;
      } else if (overrideController != null) {
        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
      }
      return weapon;
    }

    private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
      Transform oldWeapon = rightHand.Find(weaponName);
      if (oldWeapon == null) {
        oldWeapon = leftHand.Find(weaponName);
      }
      if (oldWeapon == null) return;
      oldWeapon.name = "DESTROYING";
      Destroy(oldWeapon.gameObject);
    }

    public float getDamage() {
      return damage;
    }

    public float getPercentageBonus() {
      return percentageBonus;
    }

    public float getRange() {
      return range;
    }

    public bool HasProjectile() {
      return projectile != null;
    }

    public float GetEnergyToAttack() {
      return energyToAttack;
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage) {
      Projectile projectileInstance = Instantiate(projectile, GetHand(rightHand, leftHand).position, Quaternion.identity);
      projectileInstance.SetTarget(target, instigator, calculatedDamage);
    }

    private Transform GetHand(Transform rightHand, Transform leftHand) {
      return isRightHanded ? rightHand : leftHand;
    }
  }
}