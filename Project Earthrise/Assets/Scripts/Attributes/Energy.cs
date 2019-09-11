using System.Collections;
using GameDevTV.Utils;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes {
  public class Energy : MonoBehaviour, ISaveable {

    private LazyValue<float> energyPoints;
    private LazyValue<float> energyRecharge;
    private bool consumingEnergy;
    private bool isRecharging;

    private void Awake() {
      energyPoints = new LazyValue<float>(GetInitialEnergy);
      energyRecharge = new LazyValue<float>(GetInitialEnergyRecharge);
    }

    private void Start() {
      energyPoints.ForceInit();
      StartCoroutine(RechargeEnergy());
    }

    private float GetInitialEnergy() {
      return GetMaxEnergyPoints();
    }

    private float GetInitialEnergyRecharge() {
      return GetComponent<BaseStats>().GetStat(Stat.EnergyRecharge);
    }

    public float GetEnergyPoints() {
      return energyPoints.value;
    }

    public float GetMaxEnergyPoints() {
      return GetComponent<BaseStats>().GetStat(Stat.Energy);
    }

    public float GetFraction() {
      return energyPoints.value / GetMaxEnergyPoints();
    }

    public float GetPercentage() {
      return 100 * GetFraction();
    }

    public bool CanConsumeEnergy(float amount) {
      return energyPoints.value >= amount;
    }

    public void ConsumeEnergy(float amount) {
      consumingEnergy = true;
      energyPoints.value = Mathf.Max(energyPoints.value - amount, 0);
    }

    public void StopConsumingEnergy() {
      consumingEnergy = false;
      StartCoroutine(RechargeEnergy());
    }

    public void RestoreEnergy(float energyToRestore) {
      energyPoints.value = Mathf.Min(energyPoints.value + energyToRestore, GetMaxEnergyPoints());
    }

    public object CaptureState() {
      return energyPoints.value;
    }

    public void RestoreState(object state) {
      energyPoints.value = (float) state;
      StartCoroutine(RechargeEnergy());
    }

    private IEnumerator RechargeEnergy() {
      if (isRecharging) yield break;
      isRecharging = true;
      yield return new WaitForSeconds(1);
      while (!consumingEnergy) {
        RestoreEnergy(energyRecharge.value * Time.deltaTime);
        yield return new WaitForEndOfFrame();
      }
      isRecharging = false;
    }
  }
}