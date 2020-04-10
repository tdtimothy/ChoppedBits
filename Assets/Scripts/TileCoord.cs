using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileCoord
{
    public int x;
    public int y;

    public TileCoord(int nx, int ny)
    {
        x = nx;
        y = ny;
    }

    public void mult(int m)
    {
        x *= m;
        y *= m;
    }

    public void add(TileCoord p)
    {
        x += p.x;
        y += p.y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(TileCoord p)
    {
        return (x == p.x && y == p.y);
    }

    public static TileCoord fromVector(Vector2 v)
    {
        return new TileCoord((int)v.x, (int)v.y);
    }

    public static TileCoord fromVector(Vector3 v)
    {
        return new TileCoord((int)v.x, (int)v.y);
    }

    public static TileCoord mult(TileCoord p, int m)
    {
        return new TileCoord(p.x * m, p.y * m);
    }

    public static TileCoord add(TileCoord p, TileCoord o)
    {
        return new TileCoord(p.x + o.x, p.y + o.y);
    }

    public static TileCoord clone(TileCoord p)
    {
        return new TileCoord(p.x, p.y);
    }

    public static TileCoord zero
    {
        get { return new TileCoord(0, 0); }
    }
    public static TileCoord one
    {
        get { return new TileCoord(1, 1); }
    }
    public static TileCoord up
    {
        get { return new TileCoord(0, 1); }
    }
    public static TileCoord down
    {
        get { return new TileCoord(0, -1); }
    }
    public static TileCoord right
    {
        get { return new TileCoord(1, 0); }
    }
    public static TileCoord left
    {
        get { return new TileCoord(-1, 0); }
    }
}
