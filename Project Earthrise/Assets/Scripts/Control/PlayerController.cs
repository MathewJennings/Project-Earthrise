using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
  public class PlayerController : MonoBehaviour {

    [SerializeField] private CanvasGroup pauseOverlay;
    [SerializeField] private GameObject pausePanel;
    private Health playerHealth;

    private void Awake() {
      playerHealth = GetComponent<Health>();
      UnpauseGame();
    }

    void Update() {
      InteractWithPauseMenu();
      if (pausePanel.activeInHierarchy) return;

      InteractWithMovement();
      InteractWithCombat();
    }

    private void InteractWithPauseMenu() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        if (pausePanel.activeInHierarchy == false) {
          PauseGame();
        } else {
          UnpauseGame();
        }
      }
    }

    private void PauseGame() {
      Time.timeScale = 0f;
      pauseOverlay.alpha = 1f;
      pausePanel.SetActive(true);
      Cursor.visible = true;
    }

    public void UnpauseGame() {
      Time.timeScale = 1f;
      pauseOverlay.alpha = 0f;
      pausePanel.SetActive(false);
      Cursor.visible = false;
    }

    private void InteractWithMovement() {
      Vector3 movementTarget = Vector3.zero;
      float speed = 1f;
      if (Input.GetKey(KeyCode.W)) {
        movementTarget += Camera.main.transform.forward;
      }
      if (Input.GetKey(KeyCode.A)) {
        movementTarget -= Camera.main.transform.right;
      }
      if (Input.GetKey(KeyCode.S)) {
        movementTarget -= Camera.main.transform.forward;
      }
      if (Input.GetKey(KeyCode.D)) {
        movementTarget += Camera.main.transform.right;
      }
      if (Input.GetKey(KeyCode.LeftControl)) {
        speed = 0.3f;
      }
      if (Input.GetKey(KeyCode.LeftShift)) {
        speed = 1.2f;
      }
      if (movementTarget != Vector3.zero) {
        GetComponent<Mover>().MoveInDirection(movementTarget, speed);
      }

      if (Input.GetKeyDown(KeyCode.Space)) {
        GetComponent<Jumper>().Jump();
      }

      if (Input.GetKeyUp(KeyCode.LeftShift)  || Input.GetKeyUp(KeyCode.Space)) {
        GetComponent<Energy>().StopConsumingEnergy();
      }
    }

    private void InteractWithCombat() {
      if (Input.GetMouseButtonDown(0)) {
        GetComponent<Fighter>().Attack();
      }
    }
  }
}