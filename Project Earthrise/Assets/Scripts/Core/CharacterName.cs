using UnityEngine;

namespace RPG.Core {
  public class CharacterName : MonoBehaviour {

    [SerializeField] string characterName;

    public string getCharacterName() {
      return characterName;
    }
  }
}