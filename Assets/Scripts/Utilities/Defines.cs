using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines
{
    public enum CellTypes
    {
        None = -1,
        Solid,
        Source,
        Bulb,
        Slot_2,
        Slot2_Curve,
        Slot_3,
        Slot_4,
        Max
    }

    public static string[] CellPrefabNames =
    {
        "SolidTile",
        "SourceRoot",
        "BulbRoot",
        "2SlotRoot",
        "2SlotsCurveRoot",
        "3SlotsRoot",
        "4SlotsRoot"
    };

    public enum Directions
    {
        North, South, West, East
    };
}

