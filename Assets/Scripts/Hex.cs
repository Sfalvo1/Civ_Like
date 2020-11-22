using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<summary>
// Defines grid position, world space pos, size, neighbors, etc. of a Hex tile.
// However, does NOT interact directly with Unity at all
//</summary>


public class Hex
{
    public Hex(HexMap hexMap, int q, int r)
    {
        this.hexMap = hexMap;
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }

    // Q + R + S = 0
    // S = -(Q + R)

    public readonly int Q; // Column
    public readonly int R; // Row
    public readonly int S;

    // Data for map generation and maybe in-game effects

    public float elevation;
    public float moisture;

    private HexMap hexMap;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    float radius = 1f;

    // Returns world space position of this hex
    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (this.Q + this.R/2f),
            0,
            HexVerticalSpacing() * this.R
            );
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public Vector3 PositionFromCamera( Vector3 cameraPosition, float numRows, float numColumns)
    {
        // float mapHeight = numRows * HexHorizontalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;

        // Want howManyWidthsFromCamera to be between -0.5 to 0.5
        if(Mathf.Abs(howManyWidthsFromCamera) <= 0.5f)
        {
            // no changes
            return position;
        }

        // If at 0.6, then we want to be at -0.4
        // If at 0.8, then we want to be at -0.2
        // If at 2.2, then we want to be at 0.2
        // If at 2.8, then want to be at -0.2
        // 2.6 => -0.4
        // -0.6 => 0.4

        if(howManyWidthsFromCamera > 0)
        {
            howManyWidthsFromCamera += 0.5f;
        }
        else
        {
            howManyWidthsFromCamera -= 0.5f;
        }

        int howManyWidthToFix = (int)howManyWidthsFromCamera;

        position.x -= howManyWidthToFix * mapWidth;

        return position;
    }

    public static float Distance(Hex a, Hex b)
    {
        // FIXME: wrapping
        return Mathf.Max(
            Mathf.Abs(a.Q - b.Q),
            Mathf.Abs(a.R - b.R),
            Mathf.Abs(a.S - b.S)
            );
        // The biggest out of any of these is the distance
    }

}
