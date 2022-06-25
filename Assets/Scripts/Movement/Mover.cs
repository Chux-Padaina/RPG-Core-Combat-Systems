
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent;

        const string animatorForwardSpeed = "ForwardSpeed";

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!GetComponent<Health>().IsAlive())
            {
                navMeshAgent.enabled = false;
            }
            UpdateAnimator();
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat(animatorForwardSpeed, speed);
        }

        public void StartMoveAction(Vector3 destination)
        {
            if (GetComponent<Health>().IsAlive())
            {
                GetComponent<ActionScheduler>().StartAction(this);
                MoveTo(destination);
            }
        }
        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = destination;
        }

    }
}
