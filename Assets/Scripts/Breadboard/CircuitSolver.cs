using System.Collections.Generic;
using UnityEngine;

public class CircuitSolver : MonoBehaviour
{
    [Header("Ref")]
    public BreadboardManager breadboardManager;


    [Header("Power Rails")]
    public GridRegion positivePowerRail;
    public GridRegion negativePowerRail;

    [Header("Voltage")]
    public float supplyVoltage = 5f;

    private Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
    private List<CircuitComponent> components = new List<CircuitComponent   >();


    int nextNodeId;



    public void Solve()
    {
        nodeMap.Clear();
        nextNodeId = 0;


        //creating nodeMap
        foreach(CircuitComponent comp in components)
        {
            foreach(ComponentLeg leg in comp.legs)
            {
                string key = GetKeyNode(leg);
                if (!nodeMap.ContainsKey(key))
                {
                    nodeMap[key] = new Node(nextNodeId++);
                }
                leg.node = nodeMap[key];
            }
        }

        //setting battery
        foreach(CircuitComponent comp in components)
        {
            if (comp is BatteryComponent battery)
            {
                if(battery.legs[0].node != null && battery.legs[1].node != null)
                {
                    battery.legs[0].node.voltage = supplyVoltage;
                    battery.legs[1].node.voltage = 0f;
                }
            }
        }

    }

    public void RegisterComponent(CircuitComponent comp)
    {
        if (!components.Contains(comp))
        {
            components.Add(comp);
        }
        Solve();
    }

    public void UnRegisterComponent(CircuitComponent comp)
    {
        components.Remove(comp);

        foreach(ComponentLeg leg in comp.legs)
        {
            leg.node = null;
        }

        comp.ResetState();
        Solve();
    }


    private string GetKeyNode(ComponentLeg leg)
    {
        GridRegion legRegion = leg.snappedRegion;

        Vector3 legLocalPos = breadboardManager.breadboardRoot.InverseTransformPoint(leg.transform.position);
        Vector3 legPosRelToRegion = legLocalPos - legRegion.localOrigin;

        if(legRegion.connectivityType == GridRegion.ConnectivityType.rowsConnected)
        {
            int row = Mathf.RoundToInt(legPosRelToRegion.x / legRegion.rowSpacing);
            row = Mathf.Clamp(row, 0, legRegion.rows - 1);

            return $"{legRegion.regionName}_row_{row}";
        }
        else
        {
            int col = Mathf.RoundToInt(legPosRelToRegion.z / legRegion.columnSpacing);
            col = Mathf.Clamp(col, 0,legRegion.columns - 1);


            if (legRegion.isSegmentedCol)
            {
                int segmentIndex = col / legRegion.segmentedColSize;
                return $"{legRegion.regionName}_seg_{segmentIndex}";
            }
            else
            {
                return $"{legRegion.regionName}_rail";
            }
        }
    }
}
