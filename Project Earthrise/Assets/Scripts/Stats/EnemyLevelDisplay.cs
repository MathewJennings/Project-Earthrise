using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
  public class EnemyLevelDisplay : MonoBehaviour {

    [SerializeField] BaseStats baseStatsComponent;
    [SerializeField] TextMeshProUGUI levelText;

    private void Start() {
      levelText.text = String.Format("{0}", baseStatsComponent.GetLevel());
    }
  }
}