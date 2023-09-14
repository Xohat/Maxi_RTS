using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CTeam))]
public class BaseController : MonoBehaviour
{

    [HideInInspector]
    public CTeam team;

    public Transform spawnPoint;

    public GameObject baseUnitPrefab;

    private void Awake()
    {
        team = GetComponent<CTeam>();
    }

    public GameObject Action(int actionType)
    {
        GameObject unit = null;
        switch (actionType)
        {
            case 0:
                unit = GameObject.Instantiate(baseUnitPrefab, spawnPoint.position + Random.insideUnitSphere, spawnPoint.rotation);
                break;
        }

        return unit;
    }
}
