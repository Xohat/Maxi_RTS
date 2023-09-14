using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTeam : MonoBehaviour
{

    public Color teamColor;
    public int teamNumber;

    private void Start()
    {
        ResetColor();
    }

    public void ResetColor()
    {
        CSelectable selectable = GetComponent<CSelectable>();
        if (selectable)
            selectable.SetColor(teamColor);
    }

}
