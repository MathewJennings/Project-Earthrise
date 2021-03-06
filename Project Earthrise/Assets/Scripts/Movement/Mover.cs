﻿using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
  public class Mover : MonoBehaviour, IAction, ISaveable {

    [SerializeField] float maxSpeed = 6f;
    [Range(1, 2)]
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float maxRotationTime = 1f;
    [SerializeField] float energyToSprint = 0.1f;
    [SerializeField] float maxNavMeshProjectionDistance = 1f;
    [SerializeField] float maxNavPathLength = 40f;

    private NavMeshAgent navMeshAgent;
    private Health health;
    private Energy energy;
    private float elapsedRotationTime;
    private Jumper jumper;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
      energy = GetComponent<Energy>();
      jumper = GetComponent<Jumper>();
    }

    private void OnEnable() {
      health.onDie.AddListener(DisableNavMeshAgent);
    }

    private void OnDisable() {
      health.onDie.RemoveListener(DisableNavMeshAgent);
    }

    private void Update() {
      UpdateAnimator();
    }

    public void StartMoveAction(Vector3 destination, float speedFraction) {
      GetComponent<ActionScheduler>().StartAction(this);
      MoveTo(destination, speedFraction);
    }

    public bool CanMoveTo(Vector3 destination) {
      NavMeshPath path = new NavMeshPath();
      bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
      if (!hasPath) return false;
      if (path.status != NavMeshPathStatus.PathComplete) return false;
      if (GetPathLength(path) > maxNavPathLength) return false;
      return true;
    }

    public void MoveTo(Vector3 destination, float speedFraction) {
      navMeshAgent.destination = destination;
      navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
      navMeshAgent.isStopped = false;
    }

    public void MoveInDirection(Vector3 direction, float speedFraction) {
      GetComponent<ActionScheduler>().StartAction(this);
      if (speedFraction > 1) {
        if (energy.CanConsumeEnergy(energyToSprint)) {
          energy.ConsumeEnergy(energyToSprint);
        } else {
          speedFraction = 1;
        }
      }
      if (jumper.IsJumping()) {
        jumper.MoveWhileJumping(direction, maxSpeed * Mathf.Max(0, speedFraction), elapsedRotationTime);
      } else {
        navMeshAgent.destination = transform.position + direction;
        navMeshAgent.speed = maxSpeed * Mathf.Max(0, speedFraction);
        navMeshAgent.isStopped = false;
      }
    }

    public IEnumerator RotateAsynchronously(Vector3 newForward) {
      while (!DoneRotating(newForward)) {
        elapsedRotationTime += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newForward), elapsedRotationTime);
        if (elapsedRotationTime / rotationSpeed > maxRotationTime) {
          elapsedRotationTime = 0f;
          yield break;
        }
        yield return new WaitForEndOfFrame();
      }
      elapsedRotationTime = 0f;
    }

    public void Cancel() {
      navMeshAgent.isStopped = true;
    }

    public object CaptureState() {
      Dictionary<string, SerializableVector3> stateMap = new Dictionary<string, SerializableVector3>();
      stateMap["position"] = new SerializableVector3(transform.position);
      stateMap["rotation"] = new SerializableVector3(transform.rotation.eulerAngles);
      return stateMap;
    }

    public void RestoreState(object state) {
      Dictionary<string, SerializableVector3> stateMap = (Dictionary<string, SerializableVector3>) state;

      GetComponent<ActionScheduler>().CancelCurrentAction();
      NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
      navMeshAgent.enabled = false;
      transform.position = stateMap["position"].ToVector3();
      transform.rotation = Quaternion.Euler(stateMap["rotation"].ToVector3());
      navMeshAgent.enabled = true;
    }

    private float GetPathLength(NavMeshPath path) {
      float total = 0f;
      if (path.corners.Length < 2) return total;
      for (int i = 0; i < path.corners.Length - 1; i++) {
        total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
      }
      return total;
    }

    private void UpdateAnimator() {
      Vector3 velocity;
      if (jumper.IsJumping()) {
        velocity = GetComponent<Rigidbody>().velocity * 100;
      } else {
        velocity = transform.InverseTransformDirection(navMeshAgent.velocity);
      }
      float speed = velocity.z;
      GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    private bool DoneRotating(Vector3 newForward) {
      Vector3 normalizedPlayerForward = transform.forward.normalized;
      Vector3 normalizedNewForward = new Vector3(newForward.x, 0, newForward.z).normalized;
      return Vector3.Distance(normalizedPlayerForward, normalizedNewForward) < 0.01f;
    }

    private void DisableNavMeshAgent() {
      navMeshAgent.enabled = false;
    }
  }
}