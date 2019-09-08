using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core {
  public class CharacterNameDisplay : MonoBehaviour {

    [SerializeField] CharacterName characterName;
    [SerializeField] Text characterNameText;

    private void Start() {
      characterNameText.text = characterName.getCharacterName();
    }
  }
}