using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CTeam))]
public class ArmyController : MonoBehaviour
{

    private CTeam team;

    private int selectableLayerMask;
    private int floorLayerMask;

    public List<UnitFSMBase> units = new List<UnitFSMBase>();

    public BaseController armyBase;

    public List<UnitFSMBase> unitsSelected = new List<UnitFSMBase>();
    private List<CSelectable> buildingsSelected = new List<CSelectable>();

    // multiselection
    [HideInInspector]
    public bool selecting = false;
    private Vector3 lastClick = new Vector3();
    private bool mouseButtonPressed = false;
    [HideInInspector]
    public Rect selectionRect = new Rect();

    private void Awake()
    {
        team = GetComponent<CTeam>();
    }

    // Start is called before the first frame update
    void Start()
    {
        selectableLayerMask = LayerMask.GetMask("Selectable");
        floorLayerMask = LayerMask.GetMask("Floor");

        // get the references of the units of this team of the scene
        UnitFSMBase[] unitsInScene = FindObjectsOfType<UnitFSMBase>();
        foreach (UnitFSMBase unit in unitsInScene)
        {
            if (unit.team.teamNumber == team.teamNumber && !units.Contains(unit))
            {
                units.Add(unit);

                unit.army = this;

                DistanceMatrix.InsertUnit(unit);
            }
        }

        // reset the units colors
        foreach (UnitFSMBase unit in units)
        {
            unit.team.teamColor = team.teamColor;
            unit.team.ResetColor();
        }

        // reset the base color
        armyBase.team.teamColor = team.teamColor;
        armyBase.team.ResetColor();
    }

    // Update is called once per frame
    void Update()
    {
        // left click down
        if (Input.GetMouseButtonDown(0))
        {
            lastClick = Input.mousePosition;
            mouseButtonPressed = true;

            selectionRect.x = Input.mousePosition.x;
            selectionRect.y = Input.mousePosition.y;
        }
        
        // left click up
        if (Input.GetMouseButtonUp(0))
        {
            mouseButtonPressed = false;
            selecting = false;

            // simple selection: the mouse is up and the last mouse
            // position is the same that lastClick
            if (Input.mousePosition == lastClick)
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    DeselectAllUnits();

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayerMask))
                {
                    Transform objectHit = hit.transform;

                    CTeam hitTeam = hit.transform.GetComponent<CTeam>();

                    if (hitTeam && hitTeam.teamNumber == team.teamNumber)
                    {
                        CSelectable selectable = hitTeam.GetComponent<CSelectable>();
                        if (selectable)
                        {
                            selectable.SetSelected();

                            UnitFSMBase unit = hitTeam.GetComponent<UnitFSMBase>();
                            if (unit)
                                unitsSelected.Add(unit);
                            else
                            {
                                BaseController building = hitTeam.GetComponent<BaseController>();
                                if (building)
                                    buildingsSelected.Add(selectable);
                            }
                        }
                    }
                }
                else
                {
                    DeselectAllUnits();
                }
            }            
        }

        // mouse moves while clicking
        if (mouseButtonPressed && (Input.mousePosition != lastClick))
        {
            // multiple selection ongoing
            if (!selecting)
            {
                DeselectAllUnits();

                selecting = true;
                selectionRect.width = selectionRect.height = 0;
            }
            else
            {
                // update the square selection points values
                selectionRect.width  = Input.mousePosition.x - selectionRect.x;
                selectionRect.height = Input.mousePosition.y - selectionRect.y;

                if (lastClick.x > Input.mousePosition.x)
                {
                    // drag towards left - flip x
                    selectionRect.width = lastClick.x - Input.mousePosition.x;
                    selectionRect.x = Input.mousePosition.x;
                }

                if (lastClick.y > Input.mousePosition.y)
                {
                    // dragging up - flip y 
                    selectionRect.height = lastClick.y - Input.mousePosition.y;
                    selectionRect.y = Input.mousePosition.y;
                }

                Debug.Log(selectionRect);

                // check multiselection
                foreach (UnitFSMBase unit in units)
                {
                    if (selectionRect.Contains(unit.screenPosition))
                    {
                        if (!unitsSelected.Contains(unit))
                        {
                            unitsSelected.Add(unit);
                            unit.selectable.SetSelected();
                        }
                    }
                    else if (unitsSelected.Contains(unit))
                    {
                        unitsSelected.Remove(unit);
                        unit.selectable.SetDeselected();
                    }
                }
            }
        }

        // right click
        if (Input.GetMouseButtonDown(1))
        {
            if (unitsSelected.Count > 0)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                int raycastMask = floorLayerMask | selectableLayerMask;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
                {
                    if (1 << hit.transform.gameObject.layer == floorLayerMask)
                    {
                        // right click on the floor
                        foreach (UnitFSMBase unit in unitsSelected)
                        {
                            unit.RightClickOnFloor(hit.point);
                        }
                    }
                    else if (1 << hit.transform.gameObject.layer == selectableLayerMask)
                    {
                        // right click on a CSelectable object
                        foreach (UnitFSMBase unit in unitsSelected)
                        {
                            unit.RightClickOnTransform(hit.transform);
                        }
                    }
                }
            }
        }

        // key actions
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (CSelectable building in buildingsSelected)
            {
                GameObject unitGO = building.GetComponent<BaseController>().Action(0);

                UnitFSMBase unit = unitGO.GetComponent<UnitFSMBase>();
                unit.army = this;
                unit.team.teamColor = team.teamColor;
                unit.team.teamNumber = team.teamNumber;
                unit.team.ResetColor();
                units.Add(unit);

                DistanceMatrix.InsertUnit(unit);
            }
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            for (int i = unitsSelected.Count - 1; i >= 0; i--)
            {
                UnitFSMBase unit = unitsSelected[i];
                UnitDied(unit);
            }
        }
    }

    private void DeselectAllUnits()
    {
        foreach(UnitFSMBase unit in unitsSelected)
        {
            unit.selectable.SetDeselected();
        }
        unitsSelected.Clear();

        foreach(CSelectable building in buildingsSelected)
        {
            building.SetDeselected();
        }
        buildingsSelected.Clear();
    }

    public void UnitDied(UnitFSMBase unit)
    {
        DistanceMatrix.DeleteUnit(unit);

        units.Remove(unit);
        if(unitsSelected.Contains(unit))
            unitsSelected.Remove(unit);

        Destroy(unit.gameObject);
    }
}
