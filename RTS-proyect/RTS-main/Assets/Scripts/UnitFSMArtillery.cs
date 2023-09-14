using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFSMArtillery : UnitFSMBase
{
    public enum ArtilleryState
    {
        None,
        Alert,
        Attacking,
        Chasing
    }

    public ArtilleryState currentArtilleryState;

    public MachineGunFireEfects fireEfects;

    private float alertHitTimer = 1f;
    private float alertHitTimerAux = 0f;

    public GameObject projectilePrefab;

    private Vector3 eyePosition = new Vector3(0, 2.5f, 0);

    protected override void UpdateIdle()
    {
        switch (currentArtilleryState)
        {
            case ArtilleryState.None:
                base.UpdateIdle();
                break;

            case ArtilleryState.Alert:
                {
                    if(alertHitTimerAux <= 0)
                    {
                        SearchForEnemies();
                        alertHitTimerAux = alertHitTimer;
                    }
                    else
                    {                     
                        alertHitTimerAux -= Time.deltaTime;
                    }
                    break;
                }

            case ArtilleryState.Attacking:
                FireProjectile(enemiesInVisionSphere[0].gameObject.transform.position);
                break;

            case ArtilleryState.Chasing:
                break;
        }
    }
    public void FireProjectile(Vector3 targetPosition)
    {
        if (enemiesInVisionSphere.Contains(currentEnemy))
        {
            transform.LookAt(currentEnemy.transform.position);
            attackCadenceAux += Time.deltaTime;

            if (attackCadenceAux >= attackCadence)
            {
                // Instantiate the projectile
                GameObject projectileObject = Instantiate(
                    projectilePrefab,
                    transform.position,
                    Quaternion.LookRotation(targetPosition - transform.position)
                );

                ArtilleryProyectile projectile = projectileObject.GetComponent<ArtilleryProyectile>();

                // Set the reference to the spawner, its team, and the target position
                projectile.SetData(this, team);
                projectile.SetTargetPosition(targetPosition);

                attackCadenceAux = 0;
            }
        }
    }

    private void SearchForEnemies()
    {
        if(enemiesInVisionSphere.Count > 0)
        {
            // Throw a ray for each enemy inside vision sphere
            for (int i = 0; i < enemiesInVisionSphere.Count; i++)
            {
                Debug.DrawLine(eyePosition, enemiesInVisionSphere[i].transform.position, Color.yellow);

                Vector3 dir = enemiesInVisionSphere[i].transform.position - (transform.position + eyePosition);
                dir.Normalize();

                RaycastHit hit;

                if (Physics.Raycast(transform.position + eyePosition, dir, out hit, visionSphereRadius))
                {
                    CTeam enemy = hit.transform.GetComponent<CTeam>();
                    Vector3 enemyPosition = enemy.transform.position;

                    if (enemy) 
                    {
                        // Attack only if we can see the target we are aiming for
                        if (hit.transform.gameObject == enemiesInVisionSphere[i].gameObject)
                        {
                            currentEnemy = enemiesInVisionSphere[i];
                            currentArtilleryState = ArtilleryState.Attacking;
                            attackCadenceAux = 0;
                            return;
                        }
                    }
                }
            }
        }
    }

    public override void EnemyEntersInVisionSphere(UnitFSMBase enemy)
    {
        base.EnemyEntersInVisionSphere(enemy);

        if(currentArtilleryState == ArtilleryState.None)
        {
            currentArtilleryState = ArtilleryState.Alert;
            alertHitTimerAux = 0;
        }    
    }

    public override void EnemyLeavesVisionSphere(UnitFSMBase enemy)
    {
        base.EnemyLeavesVisionSphere(enemy);

        if(enemiesInVisionSphere.Count == 0)
        {
            currentArtilleryState = ArtilleryState.None;
        }
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        GUI.Label(new Rect(screenPosition.x - 20f, Screen.height - screenPosition.y - 40f, 200f, 20f), currentArtilleryState.ToString());
    }
}
