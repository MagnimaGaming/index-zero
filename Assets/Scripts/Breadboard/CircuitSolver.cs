using System.Collections.Generic;
using UnityEngine;

public class CircuitSolver : MonoBehaviour
{
    [Header("Ref")]
    public BreadboardManager breadboardManager;

    [Header("Voltage")]
    public float supplyVoltage = 5f;
    public float solvedCurrent = 0f;

    private Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
    private List<CircuitComponent> components = new List<CircuitComponent>();


    int nextNodeId;



    public void Solve()
    {
        nodeMap.Clear();
        nextNodeId = 0;


        //creating nodeMap
        foreach (CircuitComponent comp in components)
        {
            foreach (ComponentLeg leg in comp.legs)
            {
                if (!leg.isSnapped) return;
                string key = GetKeyNode(leg);
                if (!nodeMap.ContainsKey(key))
                {
                    nodeMap[key] = new Node(nextNodeId++);
                }
                leg.node = nodeMap[key];
            }
        }

        MergeWireNodes();

        //setting battery
        foreach (CircuitComponent comp in components)
        {
            if (comp is BatteryComponent battery)
            {
                if (battery.legs[0].node != null && battery.legs[1].node != null)
                {
                    battery.legs[0].node.voltage = supplyVoltage;
                    battery.legs[1].node.voltage = 0f;
                }
            }
        }

        CalculateVoltages();

        foreach (CircuitComponent comp in components)
        {
            comp.Simulate(this);
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

        foreach (ComponentLeg leg in comp.legs)
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

        if (legRegion.connectivityType == GridRegion.ConnectivityType.rowsConnected)
        {
            int row = Mathf.RoundToInt(legPosRelToRegion.x / legRegion.rowSpacing);
            row = Mathf.Clamp(row, 0, legRegion.rows - 1);

            return $"{legRegion.regionName}_row_{row}";
        }
        else
        {
            int col = Mathf.RoundToInt(legPosRelToRegion.z / legRegion.columnSpacing);
            col = Mathf.Clamp(col, 0, legRegion.columns - 1);


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

    private void CalculateVoltages()
    {
        float totalResistance = 0f;
        float totalForwardVoltage = 0f;
        float highVoltage = -1;
        float lowVoltage = -1;
        bool batterOn = false;


        foreach (CircuitComponent comp in components)
        {
            if (comp is ResisterComponent resister)
            {
                totalResistance += resister.resistance;
            }

            if (comp is LEDComponent led)
            {
                totalForwardVoltage += led.forwardVoltage;
            }

            if (comp is BatteryComponent battery)
            {
                if (battery.legs[0].node != null)
                {
                    highVoltage = battery.legs[0].node.voltage;
                }

                if (battery.legs[1].node != null)
                {
                    lowVoltage = battery.legs[1].node.voltage;
                }
            }
        }

        if (!batterOn) return;
        if (lowVoltage < 0 && highVoltage < 0) return;

        float availableVoltage = highVoltage - lowVoltage;
        if (availableVoltage <= 0f) return;

        if (totalResistance <= 0f)
        {
            solvedCurrent = 999f;
            return;
        }

        float voltageAcrossResister = availableVoltage - totalForwardVoltage;
        if (voltageAcrossResister <= 0)
        {
            solvedCurrent = 0f;
            return;
        }


        solvedCurrent = voltageAcrossResister / totalResistance;


        foreach (var kvp in nodeMap)
        {
            if (kvp.Value.voltage == -1f)
            {
                kvp.Value.voltage = lowVoltage + totalForwardVoltage;
            }
        }

    }

    private void MergeWireNodes()
    {
        foreach (CircuitComponent comp in components)
        {
            if (comp is JumperWireComponent wireComponent)
            {
                if (wireComponent.legs[0].node == null) return;
                if (wireComponent.legs[1].node == null) return;

                Node nodeA = wireComponent.legs[0].node;
                Node nodeB = wireComponent.legs[1].node;

                if (nodeA == null || nodeA == null) return;

                if (nodeA == nodeB) return;

                MergeIntoOneNode(nodeA, nodeB);
            }
        }
    }


    private void MergeIntoOneNode(Node keepNode, Node removeNode)
    {
        if (keepNode == null || removeNode == null) return;

        foreach(CircuitComponent comp in components)
        {
            foreach(ComponentLeg leg in comp.legs)
            {
                if (leg.node == removeNode)
                {
                    leg.node = keepNode;
                }
            }
        }

        //update nodeMap

        foreach(var key in new List<string>(nodeMap.Keys))
        {
            if (nodeMap[key] == removeNode)
            {
                nodeMap[key] = keepNode;
            }
        }
    }
}
