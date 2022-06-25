using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        float chaseDistance = 5f;

        GameObject player;

        Vector3 spawnLocation;

        Health healthComponent;

        float playerLastSeen = Mathf.Infinity;

        public float suspicionTime = 4f;

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
                playerLastSeen = 0;
                GetComponent<Fighter>().Attack(player);           
            }
            else if (playerLastSeen <= suspicionTime)
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                GetComponent<Fighter>().Cancel();
                GetComponent<Mover>().StartMoveAction(spawnLocation);
            }
            playerLastSeen += Time.deltaTime;
        }

        // unity calls
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
