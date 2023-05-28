using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMatrix : MonoBehaviour
{
    // All units in team 0
    public static List<UnitFSMBase> Army0 = new List<UnitFSMBase>();

    // All units in team 1
    public static List<UnitFSMBase> Army1 = new List<UnitFSMBase>();

    // rows = Army 0 columns = Army 1
    // distanceMatrix[2][3] is distance ^2 between Army0[2] and Army1[3]
    public static List<List<float>> distanceMatrix = new List<List<float>>();

    private enum IterateMode
    {
        no_search,
        all_at_once,
        // ChatGPT method
    }

    private IterateMode mode = IterateMode.all_at_once;

    private void LateUpdate()
    {
        if (Army0.Count == 0 || Army1.Count == 0)
            return;

        switch (mode)
        {
            case IterateMode.no_search:
                break;

            case IterateMode.all_at_once:
                UpdateAll();
                break;
        }
    }

    private void UpdateAll()
    {
        for (int i = 0; i < Army0.Count; i++)
        {
            for (int j = 0; j < Army1.Count; j++)
            {
                UpdateTwoUnits(i, j);
            }
        }
    }

    private void UpdateTwoUnits(int i, int j)
    {
        UnitFSMBase unit0 = Army0[i];
        UnitFSMBase unit1 = Army1[j];

        float prevDist = distanceMatrix[i][j];

        float xDist = Mathf.Abs(unit0.transform.position.x - unit1.transform.position.x);

        if (xDist < unit0.visionSphereRadius || xDist < unit1.visionSphereRadius)
        {
            float zDist = Mathf.Abs(unit0.transform.position.z - unit1.transform.position.z);

            if (zDist < unit0.visionSphereRadius || zDist < unit1.visionSphereRadius)
            {
                float newDist = xDist * xDist + zDist * zDist;
                distanceMatrix[i][j] = newDist;

                if (prevDist >= unit0.visionSphereRadius2 && newDist <= unit0.visionSphereRadius2)
                {
                    // Unit0 sees Unit1 for the first time
                    unit0.EnemyEntersInVisionSphere(unit1);
                }
                else if (prevDist < unit0.visionSphereRadius2 && newDist > unit0.visionSphereRadius2)
                {
                    // Unit0 stop seeing Unit1
                    unit0.EnemyLeavesVisionSphere(unit1);
                }

                if (prevDist >= unit1.visionSphereRadius2 && newDist <= unit1.visionSphereRadius2)
                {
                    // Unit0 sees Unit1 for the first time
                    unit1.EnemyEntersInVisionSphere(unit0);
                }
                else if (prevDist < unit1.visionSphereRadius2 && newDist > unit1.visionSphereRadius2)
                {
                    // Unit0 stop seeing Unit1
                    unit1.EnemyLeavesVisionSphere(unit0);
                }

            }
            else
            {
                if (prevDist <= unit0.visionSphereRadius2)
                    unit0.EnemyLeavesVisionSphere(unit1);
                if (prevDist <= unit1.visionSphereRadius2)
                    unit1.EnemyLeavesVisionSphere(unit0);

                distanceMatrix[i][j] = float.MaxValue;
            }
        }
        else
        {
            if (prevDist <= unit0.visionSphereRadius2)
                unit0.EnemyLeavesVisionSphere(unit1);
            if (prevDist <= unit1.visionSphereRadius2)
                unit1.EnemyLeavesVisionSphere(unit0);

            distanceMatrix[i][j] = float.MaxValue;
        }
    }

    public static void InsertUnit(UnitFSMBase unit)
    {
        if (unit.team.teamNumber == 0)
        {
            // Insert a new row
            Army0.Add(unit);

            List<float> newRow = new List<float>();

            foreach (UnitFSMBase item in Army1)
                newRow.Add(float.MaxValue);

            distanceMatrix.Add(newRow);
        }
        else if (unit.team.teamNumber == 1)
        {
            // Insert a new column
            Army1.Add(unit);

            for (int i = 0; i < Army0.Count; i++)
            {
                distanceMatrix[i].Add(float.MaxValue);
            }
        }
    }

    public static void DeleteUnit(UnitFSMBase unit)
    {
        if(unit.team.teamNumber == 0)
        {
            // Delete a row
            int unitIndex = Army0.IndexOf(unit);

            Army0.RemoveAt(unitIndex);
            distanceMatrix.RemoveAt(unitIndex);
        }
        else if(unit.team.teamNumber == 1)
        {
            // Delete a column
            int unitIndex = Army1.IndexOf(unit);

            Army1.RemoveAt(unitIndex);

            for (int i = 0; i < Army0.Count; i++)
            {
                distanceMatrix[i].RemoveAt(unitIndex);
            }
        }
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 10;

        int iCount = distanceMatrix.Count;
        int jCount;
        for (int i = 0; i < iCount; i++)
        {
            jCount = distanceMatrix[i].Count;
            for (int j = 0; j < jCount; j++)
            {
                float aux = distanceMatrix[i][j];
                if (aux != float.MaxValue)
                    GUI.Label(new Rect(100 + 50 * j, 100 + 20 * i, 50, 20), aux.ToString());
                else
                    GUI.Label(new Rect(100 + 50 * j, 100 + 20 * i, 50, 20), "---");
            }
        }
    }
}
