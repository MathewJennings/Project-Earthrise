using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
  public class AIController : MonoBehaviour {
    [SerializeField] float chaseDistance = 5f;
    [SerializeField] float suspicionTime = 3f;
    [Range(0, 1)]
    [SerializeField] float patrolSpeedFraction = 0.35f;
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float waypointTolerance = 1f;
    [SerializeField] float waypointDwellTime = 3f;
    [SerializeField] float aggroCooldownTime = 5f;
    [SerializeField] float shoutDistance = 5f;

    private ActionScheduler actionScheduler;
    private Mover mover;
    private Fighter fighter;
    private Health health;
    private GameObject player;

    private LazyValue<Vector3> guardPosition;
    private int currentWaypointIndex;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    private float timeSinceLastSawPlayer = Mathf.Infinity;
    private float timeSinceAggrevated = Mathf.Infinity;

    private void Awake() {
      actionScheduler = GetComponent<ActionScheduler>();
      mover = GetComponent<Mover>();
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
      player = GameObject.FindWithTag("Player");
      guardPosition = new LazyValue<Vector3>(GetGuardPosition);
    }

    private Vector3 GetGuardPosition() {
      return transform.position;
    }

    private void Start() {
      guardPosition.ForceInit();
    }

    private void Update() {
      if (health.IsDead()) return;

      if (IsAggrevated() && fighter.CanAttack(player)) {
        AttackBehavior();
      } else if (timeSinceLastSawPlayer <= suspicionTime) {
        SuspicionBehavior();
      } else {
        PatrolBehavior();
      }

      UpdateTimers();
    }

    public void Aggrevate() {
      timeSinceAggrevated = 0;
    }

    private bool IsAggrevated() {
      return timeSinceAggrevated <= aggroCooldownTime ||
        Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
    }

    private void AttackBehavior() {
      timeSinceLastSawPlayer = 0;
      fighter.Attack(player);

      AggrevateNearbyEnemies();
    }

    private void AggrevateNearbyEnemies() {
      RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
      foreach(RaycastHit hit in hits) {
        AIController enemy = hit.collider.GetComponent<AIController>();
        if (enemy == null) continue;
        enemy.Aggrevate();
      }
    }

    private void SuspicionBehavior() {
      actionScheduler.CancelCurrentAction();
    }

    private void PatrolBehavior() {
      Vector3 nextPosition = guardPosition.value;
      if (patrolPath != null) {
        if (AtWaypoint()) {
          timeSinceArrivedAtWaypoint = 0;
          CycleWaypoint();
        }
        nextPosition = GetCurrentWaypoint();
      }
      if (timeSinceArrivedAtWaypoint >= waypointDwellTime) {
        mover.StartMoveAction(nextPosition, patrolSpeedFraction);
      }
    }

    private bool AtWaypoint() {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
      return distanceToWaypoint <= waypointTolerance;
    }

    private void CycleWaypoint() {
      currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
    }

    private Vector3 GetCurrentWaypoint() {
      return patrolPath.GetWaypoint(currentWaypointIndex);
    }

    private void UpdateTimers() {
      timeSinceLastSawPlayer += Time.deltaTime;
      timeSinceArrivedAtWaypoint += Time.deltaTime;
      timeSinceAggrevated += Time.deltaTime;
    }

    private void OnDrawGizmosSelected() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}