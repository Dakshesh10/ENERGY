using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntVector2
{
    public int x, y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.y += b.y;

        return a;
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        a.x -= b.x;
        a.y -= b.y;

        return a;
    }

    public static IntVector2 operator *(IntVector2 a, int f)
    {
        a.x *= f;
        a.y *= f;

        return a;
    }

    public static bool operator >(IntVector2 a, IntVector2 b)
    {
        if(a.x > b.x && a.y > b.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator <(IntVector2 a, IntVector2 b)
    {
        if (a.x < b.x && a.y < b.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        if (a.x == b.x && a.y == b.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        if (a.x != b.x || a.y != b.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Gives a random IntVector2 instance where x and z are in range of v1 and v2 x and z respectively.
    /// </summary>
    /// <returns></returns>
    public static IntVector2 RandomFromRange(IntVector2 v1, IntVector2 v2)
    {
        int x = v1.x + Mathf.FloorToInt(Random.value * (v2.x - v1.x));
        int z = v1.y + Mathf.FloorToInt(Random.value * (v2.y - v1.y));
        return new IntVector2(x,z);
    }

    public static IntVector2 Zero
    {
        get
        {
            return new IntVector2(0, 0);
        }
    }

    public static IntVector2 One
    {
        get
        {
            return new IntVector2(1, 1);
        }
    }

    public bool isInRange(IntVector2 a, IntVector2 b)
    {
        if (x >= a.x && x <= b.x && y >= a.y && y <= b.y)
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return $"<{x}, {y}>"; ;
    }
}
