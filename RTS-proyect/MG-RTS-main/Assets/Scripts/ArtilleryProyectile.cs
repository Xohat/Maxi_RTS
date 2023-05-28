using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryProyectile : MonoBehaviour
{
    // Reference to the spawner unit
    public UnitFSMArtillery artillery_unit { get; private set; }
    public CTeam Team { get; private set; }
    // Target position
    private Vector3 targetPosition;

    // These variables will be used to define the parabolic path
    public float speed = 10f;

    // The amount of damage the explosion will cause
    public float explosionRadius = 5f;
    // The amount of damage the explosion will cause
    public float explosionDamage = 30f;

    void Start()
    {
        // Initialize the position to the offset from the artillery unit
        Vector3 offsettedSpawnPositon = new Vector3(artillery_unit.transform.position.x + 2, artillery_unit.transform.position.y + 5, artillery_unit.transform.position.z);

        transform.position = offsettedSpawnPositon;
    }

    private void Update()
    {
        Vector3 offsettedTargetPosition = new Vector3(targetPosition.x, targetPosition.y + 1.5f, targetPosition.z);

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, offsettedTargetPosition, speed * Time.deltaTime);
    }

    public void SetData(UnitFSMArtillery spawner, CTeam team)
    {
        this.artillery_unit = spawner;
        this.Team = team;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Selectable")) 
        {
            // Check if the object has a CTeam with value 1 component and if so, deal damage
            CTeam team = other.GetComponent<CTeam>();
            CLife life = other.GetComponent<CLife>();

            if (team != null && life != null)
            {
                Debug.Log("has Team");
                if (Team.teamNumber == 1 && team.teamNumber == 0)
                {
                    Debug.Log("is Valid");
                    life.Damage(explosionDamage);
                }
                else if (team.teamNumber == 1)
                {
                    Debug.Log("is Valid");
                    life.Damage(explosionDamage);
                }
            }

            Destroy(gameObject);
        }
    }



    /*
    // This method will be called when the object collides with something
    void OnCollisionEnter(Collision collision)
    {
        // Instantiate the explosion effect at the collision point
        // Instantiate(explosionEffect, collision.contacts[0].point, Quaternion.identity);

        // Debug.Log("Collision check" + collision.contacts[0].point);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Selectable")) 
        {
            // Instantiate the explosion effect at the collision point
            // Instantiate(explosionEffect, collision.contacts[0].point, Quaternion.identity);

            // Check for null collision contacts
            if (collision.contacts.Length == 0)
            {
                Debug.Log("No collision contacts");
                return;
            }

            // Deal damage to all objects within the explosion radius
            Collider[] hitColliders = Physics.OverlapSphere(collision.contacts[0].point, explosionRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                // Check if the object has a CTeam with value 1 component and if so, deal damage
                CTeam team = hitCollider.GetComponent<CTeam>();
                CLife life = hitCollider.GetComponent<CLife>();

                // Check for null components
                if (team == null)
                {
                    Debug.Log("CTeam component is null on " + hitCollider.name);
                }
                if (life == null)
                {
                    Debug.Log("CLife component is null on " + hitCollider.name);
                }
                if (Team == null)
                {
                    Debug.Log("Team is null");
                }

                if (team != null && life != null)
                {
                    if (Team.teamNumber == 1 && team.teamNumber == 0)
                    {
                        life.Damage(explosionDamage);
                    }
                    else if (team.teamNumber == 1)
                    {
                        life.Damage(explosionDamage);
                    }
                }
            }

            // Mark that we have collided
            hasCollided = true;
        }

        // Optionally, destroy the object on landing
        // Destroy(gameObject);
    }
    */
}
