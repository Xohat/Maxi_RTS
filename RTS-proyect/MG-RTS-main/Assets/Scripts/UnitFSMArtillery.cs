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
                break;
            case ArtilleryState.Chasing:
                break;
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

                    if (enemy)
                        Debug.Break();
                }
                else
                {
                    Debug.Log("Not in view sight");
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
