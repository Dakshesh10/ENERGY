using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines
{
    public const string buildingsPath = "Prefabs/Buildings/";
    public enum CellTypes
    {
        kNone = -1,
        kBoundary,
        kGround,
        kRoadL,
        kRoadR,
        kRoadBare,
        kCityFillerProp,
        kMax
    }

    public static string[] CellPrefabNames =
    {
        "Boundary_Cell",
        "Grass_Cell",
        "Road_Left",
        "Road_Right",
        "Road_Bare",
        "",
    };
}

