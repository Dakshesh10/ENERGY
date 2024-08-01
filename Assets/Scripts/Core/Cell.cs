using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum CellStates : byte
{
    kNone,
    kAlive,
    kDead
}


public class Cell : MonoBehaviour
{
    private int currentState;                 //1 for alive and 0 for dead. This is used to quantify the state of cells and neighbours.

    private bool isAlive;

    private SpriteRenderer renderer;

    public IntVector2 coordinates;
    public Defines.CellTypes thisCellType;

    public int CurrentState {
        get => currentState;

        set
        {
            currentState = value;
            IsAlive = currentState == 0 ? false : true;
        }
    }

    public bool IsAlive
    {
        get
        {
            return IsAlive;        
        }

        private set
        {
            IsAlive = value;
        }
    }

    public void Initialize(int x, int y, bool defaultState)
    {
        renderer = GetComponent<SpriteRenderer>();
        CurrentState = defaultState ? 1 : 0;
    }

    /// <summary>
    /// Toggles life-state of cell. 
    /// </summary>
    /// <returns>changed state</returns>
    public int ToggleLife()
    {
        CurrentState = !IsAlive ? 1 : 0;
        return CurrentState;
    }
}
