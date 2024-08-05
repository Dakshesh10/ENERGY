using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines
{
    public enum CellTypes
    {
        None = -1,
        Solid = 0,
        Source = 1,
        Bulb = 2,
        Slot_2 = 3,
        Slot2_Curve = 4,
        Slot_3 = 5,
        Slot_4 = 6,
        Source_2Slots = 7,
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
        "4SlotsRoot",
        "Bulb_2Slot-Lshape"
    };

    public enum Directions
    {
        North = 0, South, West, East
    };

    public static Dictionary<Directions, Directions> directionCheckMatrix = new Dictionary<Directions, Directions>()
    {
            { Directions.North, Directions.South },
            { Directions.East, Directions.West},
            { Directions.South, Directions.North },
            { Directions.West, Directions.East }
    };

    public const string PlayerCurrentLevel = "PLAYER_CURRENT_LEVEL";
}

