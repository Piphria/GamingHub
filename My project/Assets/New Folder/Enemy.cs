using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IHittable
{
    public NavMeshAgent pathfinder;
    Transform target;
    public int damage = 15;

    public int maxHealth = 20;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    public bool IsDead
    {
        get
        {
            return currentHealth <= 0;
        }
    }

    public void TakeDamage(int weaponDamage)
    {
        currentHealth = currentHealth - weaponDamage;
    }


    public void Update()
    {
        
          
        if (IsDead)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            if (!IsDead)
            {
                pathfinder.SetDestination(target.position);
                yield return new WaitForSeconds(refreshRate);
            }
                
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
  
        if (collision.collider.CompareTag("Player"))
        {
           IHittable damageableObject = collision.collider.GetComponent<IHittable>();
            Debug.Log("RIP");
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage);
                Debug.Log("RIPDebug.Log");
            }

        }
       
    }
}
