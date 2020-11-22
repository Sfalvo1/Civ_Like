using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{

    public GameObject HexPrefab;

    [Header("Meshes")]
    public Mesh meshWater;
    public Mesh meshFlat;
    public Mesh meshHill;
    public Mesh meshMountain;

    [Header("Materials")]
    public Material matOcean;
    public Material matPlains;
    public Material matGrasslands;
    public Material matMountains;
    public Material matDesert;

    // Tile height above x makes the tile x mesh
    [Header("Height Values")]
    public float heightMountain = 0.85f;
    public float heightHill = 0.6f;
    public float heightFlat = 0.0f;

    [Header("Moisture Values")]
    public float moistureJungle = 1f;
    public float moistureForest = 0.5f;
    public float moistureGrasslands = 0f;
    public float moisturePlains = -0.5f;

    [Header("Map Dimensions")]
    public int numRows = 60;
    public int numColumns = 120;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;
    
    public Hex GetHexAt(int x, int y)
    {
        if(hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated!");
            return null;
        }

        // will wrap around to the other side, rather than being OOB
        try
        {
            if (x < 0)
            {
                x += numColumns;
            }
            if(y < 0)
            {
                y += numRows;
            }
            return hexes[x % numColumns, y % numRows];
        }
        catch
        {
            Debug.LogError("GetHexAt: " + x + "," + y);
            return null;
        }
    }

    void Start()
    {
        GenerateMap();
    }

    virtual public void GenerateMap()
    {
        // Generate a map full of ocean

        hexes = new Hex[numColumns, numRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();

        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                // Instantiate Hex
                Hex h = new Hex(this, column, row);
                h.elevation = -0.5f;

                hexes[column, row] = h;

                Vector3 pos = h.PositionFromCamera(
                    Camera.main.transform.position,
                    numRows,
                    numColumns
                );

                GameObject hexGO = Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    this.transform
                    );

                hexGO.name = string.Format("Hex: {0}, {1}", column, row);

                hexToGameObjectMap[h] = hexGO;

                hexGO.GetComponent<HexComponent>().hex = h;
                hexGO.GetComponent<HexComponent>().hexMap = this;

                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);
            }
        }

        UpdateHexVisuals();

        // StaticBatchingUtility.Combine(this.gameObject);
    }

    public void UpdateHexVisuals()
    {
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = hexes[column, row];

                GameObject hexGO = hexToGameObjectMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                // Moisture

                if (h.elevation >= heightFlat)
                {
                    if (h.moisture >= moistureJungle)
                    {
                        mr.material = matGrasslands;
                        // TODO spawn jungle
                    }
                    else if (h.moisture >= moistureForest)
                    {
                        mr.material = matGrasslands;
                        // TODO spawn forests
                    }
                    else if (h.moisture >= moistureGrasslands)
                    {
                        mr.material = matGrasslands;
                    }
                    else if (h.moisture >= moisturePlains)
                    {
                        mr.material = matPlains;
                    }
                    else
                    {
                        mr.material = matDesert;
                    }
                }

                // Elevation

                if (h.elevation >= heightMountain)
                {
                    mr.material = matMountains;
                    mf.mesh = meshMountain;
                }
                else if (h.elevation >= heightHill)
                {
                    mf.mesh = meshHill;
                }
                else if (h.elevation >= heightFlat)
                {
                    mf.mesh = meshFlat;
                }
                else
                {
                    mr.material = matOcean;
                    mf.mesh = meshWater;
                }
            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();

        for(int dx = -range; dx < range-1; dx++)
        {
            for(int dy = Mathf.Max(-range+1, -dx-range); dy < Mathf.Min(range, -dx+range-1); dy++)
            {
                results.Add(GetHexAt(centerHex.Q + dx, centerHex.R + dy));
            }
        }

        return results.ToArray();
    }

}
