using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats {
  public class EnemyLevelDisplay : MonoBehaviour {

    [SerializeField] BaseStats baseStatsComponent;
    [SerializeField] Text levelText;

    private void Start() {
      levelText.text = String.Format("{0}", baseStatsComponent.GetLevel());
    }
  }
}