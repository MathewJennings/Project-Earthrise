using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText {
  public class DamageText : MonoBehaviour {

    [SerializeField] Text damageText;

    public void SetValue(float damageAmount) {
      damageText.text = String.Format("{0:0}", damageAmount);
    }

    public void DestroyText() {
      Destroy(this.gameObject);
    }
  }
}