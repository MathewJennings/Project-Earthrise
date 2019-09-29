using System;
using TMPro;
using UnityEngine;

namespace RPG.UI {
  public class DamageText : MonoBehaviour {

    [SerializeField] TextMeshProUGUI damageText;

    public void SetValue(float damageAmount) {
      damageText.text = String.Format("{0:0}", damageAmount);
    }

    public void DestroyText() {
      Destroy(this.gameObject);
    }
  }
}