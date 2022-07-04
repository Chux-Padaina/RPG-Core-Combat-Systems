using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float maxSpeed = 5.6f;
        [SerializeField] float chaseDistance = 5f;
        [Range(0,1)]
        [SerializeField] float chaseSpeedMultiplier = 0.75f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedMultiplier = 0.3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellTime = 2f;

        GameObject player;
        Vector3 spawnLocation;
        Health healthComponent;
        float playerLastSeen = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        public float suspicionTime = 4f;
        int currentWaypointIndex = 0;

        private void Start()
        {
            healthComponent = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            spawnLocation = transform.position;
        }

        private void Update()
        {
            if (!healthComponent.IsAlive()) return;
            IsPlayerInRange();
        }

        private void IsPlayerInRange()
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
            {
                AttackBehaviour();
            }
            else if (playerLastSeen <= suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            playerLastSeen += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            GetComponent<Mover>().SetSpeed(chaseSpeedMultiplier * maxSpeed);
            playerLastSeen = 0;
            GetComponent<Fighter>().Attack(player);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            GetComponent<Mover>().SetSpeed(patrolSpeedMultiplier * maxSpeed);
            Vector3 nextPosition = spawnLocation;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceArrivedAtWaypoint >= dwellTime)
            {
                GetComponent<Fighter>().Cancel();
                GetComponent<Mover>().StartMoveAction(nextPosition);
            }

        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex =  patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        // unity calls
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
