using UnityEngine;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;
        bool isAlive = true;

        public bool IsAlive() { return isAlive; }

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(0, health - damage);

            if(health <= 0)
            {
                Die();
            }
        }

        void RemoveColliderOnDeath()
        {
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        void Die()
        {         
            if (!isAlive) return;
            isAlive = false;
            RemoveColliderOnDeath();
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger("Die");
        }

        public object CaptureState()
        {
            return health; 
        }

        public void RestoreState(object state)
        {
            health = (float)state;

            if (health <= 0)
            {
                Die();
            }
        }
    }
}
