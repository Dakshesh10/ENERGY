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
        None = -1, North = 0, South, West, East, Max
    };

    /// <summary>
    /// Used to lookup for opposite direction.
    /// </summary>
    public static Dictionary<Directions, Directions> directionCheckLookup = new Dictionary<Directions, Directions>()
    {
            { Directions.North, Directions.South },
            { Directions.East, Directions.West},
            { Directions.South, Directions.North },
            { Directions.West, Directions.East }
    };

    /// <summary>
    /// Used to lookup for converting direction into world vector.
    /// </summary>
    public static Dictionary<Directions, Vector2> directionAxisLookup = new Dictionary<Directions, Vector2>()
    {
            { Directions.North, Vector2.up },
            { Directions.East,  Vector2.right},
            { Directions.South, Vector2.down },
            { Directions.West,  Vector2.left }
    };

    /// <summary>
    /// Used to lookup for converting world vector into direction.
    /// </summary>
    public static Dictionary<Vector2, Directions> directionAxisInverseLookup = new Dictionary<Vector2, Directions>()
    {
            { Vector2.up, Directions.North },
            { Vector2.right, Directions.East },
            { Vector2.down, Directions.South },
            { Vector2.left, Directions.West }
    };

    public const string PlayerCurrentLevel = "PLAYER_CURRENT_LEVEL";
}

