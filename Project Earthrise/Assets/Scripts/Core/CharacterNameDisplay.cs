using TMPro;
using UnityEngine;

namespace RPG.Core {
  public class CharacterNameDisplay : MonoBehaviour {

    [SerializeField] CharacterName characterName;
    [SerializeField] TextMeshProUGUI characterNameText;

    private void Start() {
      characterNameText.text = characterName.getCharacterName();
    }
  }
}