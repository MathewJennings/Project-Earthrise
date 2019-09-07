using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
  public class CinematicTrigger : MonoBehaviour {

    bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other) {
      if (!hasBeenTriggered && other.CompareTag("Player")) {
        GetComponent<PlayableDirector>().Play();
        hasBeenTriggered = true;
      }
    }
  }
}