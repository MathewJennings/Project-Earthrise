using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {
  public class EnergyDisplay : MonoBehaviour {

    Energy energy;

    private void Awake() {
      energy = GameObject.FindWithTag("Player").GetComponent<Energy>();
    }

    private void Update() {
      GetComponent<Image>().fillAmount = energy.GetFraction();
    }
  }
}