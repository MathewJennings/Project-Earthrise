using System;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
  /**
   * The NavMeshAgent of the attached GameObject will be disabled when IsJumping() returns true
   */
  public class Jumper : MonoBehaviour, IAction {

    [SerializeField] float jumpStrength = 7f;
    [SerializeField] float energyToJump = 10f;
    [SerializeField] float navMeshSamplingTolerance = 1f;

    private NavMeshAgent navMeshAgent;
    private Energy energy;
    private bool isJumping;
    private float jumpStartTime;
    private Vector3 jumpStartPosition;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      energy = GetComponent<Energy>();
    }

    private void Update() {
      if (!isJumping) return;

      float timeJumping = Time.time - jumpStartTime;
      if (timeJumping == 0f) {
        // Boost off of the ground
        transform.Translate(0, 0.2f, 0);
      } else if (timeJumping < 0.8f) {
        // Arc up and down
        transform.Translate(0, jumpStrength * Time.deltaTime * Mathf.Cos(Mathf.PI * timeJumping / 0.8f), 0);
      } else {
        // Fall rapidly
        transform.Translate(0, jumpStrength * Time.deltaTime * -15 * (timeJumping - 0.8f), 0);
      }
      if (timeJumping > 0.5f && NearGround()) {
        Land();
      }
    }

    public bool IsJumping() {
      return isJumping;
    }

    public void Jump() {
      if (isJumping) return;
      if (!energy.CanConsumeEnergy(energyToJump)) return;

      GetComponent<ActionScheduler>().StartAction(this);
      energy.ConsumeEnergy(energyToJump);
      navMeshAgent.enabled = false;
      isJumping = true;
      jumpStartTime = Time.time;
      jumpStartPosition = transform.position;
      GetComponent<Animator>().SetTrigger("jump");
    }

    public void Cancel() {
      // Let the Jump finish naturally
    }

    public void MoveWhileJumping(Vector3 direction, float speed, float elapsedRotationTime) {
      transform.position += direction.normalized * Time.deltaTime * speed;
      if (Mathf.Approximately(elapsedRotationTime, 0f)) {
        StartCoroutine(GetComponent<Mover>().RotateAsynchronously(direction));
      }
    }

    private bool NearGround() {
      return Vector3.Distance(transform.position, RaycastNavMesh()) < 0.02f;
    }

    private Vector3 RaycastNavMesh() {
      RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down);
      if (hits.Length < 1) return transform.position;

      float[] distances = new float[hits.Length];
      for (int i = 0; i < hits.Length; i++) {
        distances[i] = hits[i].distance;
      }
      Array.Sort(distances, hits);

      NavMeshHit navMeshHit;
      bool hasCastToNavMesh = NavMesh.SamplePosition(
        hits[hits.Length - 1].point, out navMeshHit, navMeshSamplingTolerance, NavMesh.AllAreas);
      if (!hasCastToNavMesh) { print("MISS"); return transform.position; }

      return navMeshHit.position;
    }

    private void Land() {
      navMeshAgent.enabled = true;
      isJumping = false;
      GetComponent<Animator>().SetTrigger("land");
    }
  }
}