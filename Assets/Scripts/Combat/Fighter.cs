using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        float weaponRange;
        [SerializeField]
        float timeBetweenAttacks = 1f;
        [SerializeField]
        float currentWeaponDamage = 25f;

        private float timeSinceLastAttack = Mathf.Infinity;

        Health target;
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (!target.IsAlive()) return;
            MoveWithinRange();           
        }
        public void Attack(GameObject combatTarget)
        {
            if (!GetComponent<Health>().IsAlive()) return;
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().SetTrigger("StopAttack");
            GetComponent<Animator>().ResetTrigger("Attack");
        }

        void MoveWithinRange()
        {
            if (target != null && Vector3.Distance(transform.position, target.transform.position) > weaponRange)
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                // Will trigger Hit event
                GetComponent<Animator>().ResetTrigger("StopAttack");
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0;
            }
        }

        // Animation Event
        void Hit()
        {
            if(target == null) return;  
            target.TakeDamage(currentWeaponDamage);
        }
    }
}
