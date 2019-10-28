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

    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float jumpAirTime = 0.8f;
    [SerializeField] float energyToJump = 10f;
    [SerializeField] float navMeshSamplingTolerance = 1f;

    private NavMeshAgent navMeshAgent;
    private Energy energy;
    private bool isJumping;
    private float jumpStartTime;
    private float jumpStartHeight;
    private bool isColliding;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      energy = GetComponent<Energy>();
    }

    private void Update() {
      if (!isJumping) return;

      float normalizedJumpTime = (Time.time - jumpStartTime) / jumpAirTime;
      transform.position = new Vector3(transform.position.x, jumpStartHeight + jumpHeight * Mathf.Sin(Mathf.PI * normalizedJumpTime), transform.position.z);
      if (ShouldLand(normalizedJumpTime)) {
        Land();
      }
    }

    private void OnTriggerEnter(Collider other) {
      isColliding = true;
    }

    private void OnTriggerExit(Collider other) {
      isColliding = false;
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
      jumpStartHeight = transform.position.y;
      GetComponent<Animator>().SetTrigger("jump");
    }

    public void Cancel() {
      // Let the Jump finish naturally
    }

    public void MoveWhileJumping(Vector3 direction, float speed, float elapsedRotationTime) {
      if (!isColliding) {
        transform.position += direction.normalized * Time.deltaTime * speed;
      }
      if (Mathf.Approximately(elapsedRotationTime, 0f)) {
        StartCoroutine(GetComponent<Mover>().RotateAsynchronously(direction));
      }
    }

    private bool ShouldLand(float normalizedJumpTime) {
      Vector3 potentialLandingPosition = RaycastNavMesh();
      return normalizedJumpTime > 1 || (normalizedJumpTime > 0.5f && Vector3.Distance(transform.position, potentialLandingPosition) < 0.01f);
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
      if (!hasCastToNavMesh) return transform.position;

      return navMeshHit.position;
    }

    private void Land() {
      navMeshAgent.enabled = true;
      isJumping = false;
      GetComponent<Animator>().SetTrigger("land");
    }
  }
}