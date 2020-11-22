using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap_Continent : HexMap
{
    public override void GenerateMap()
    {
        // Makes base version
        base.GenerateMap();

        int numContinents = 3;
        int continentSpacing = numColumns / numContinents;


        Random.InitState(0); // The seed of Random
        for (int c = 0; c < numContinents; c++)
        {
            // Make raised area
            int numSplats = Random.Range(4, 8);

            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 8);
                int y = Random.Range(range, numRows - range);
                int x = Random.Range(0, 10) - y / 2 + (c * continentSpacing);

                ElevateArea(x, y, range);
            }
        }

        // Add lumpiness. Perlin Noise?

        float noiseResolution = 0.01f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        float noiseScale = 2f; // Larger values, more islands

        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = GetHexAt(column, row);

                float n = Mathf.PerlinNoise(
                    ((float)column / numColumns / noiseResolution) + noiseOffset.x, 
                    ((float)row / (Mathf.Max(numColumns, numRows)) / noiseResolution) + noiseOffset.y)
                    - 0.5f;

                h.elevation += n * noiseScale;
            }
        }

        // Set mesh to mountain/hill/flat/water based on height

        noiseResolution = 0.01f;
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        noiseScale = 2f; // Larger values, more islands

        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                Hex h = GetHexAt(column, row);

                float n = Mathf.PerlinNoise(
                    ((float)column / numColumns / noiseResolution) + noiseOffset.x,
                    ((float)row / (Mathf.Max(numColumns, numRows)) / noiseResolution) + noiseOffset.y)
                    - 0.5f;

                h.moisture += n * noiseScale;
            }
        }

        // Simulate rainfall/moisture (prob perlin) and set plans/grasslands + forest

        // Make all hex visuals match the data
        UpdateHexVisuals();
    }

    void ElevateArea(int q, int r, int range, float centerHeight = 0.8f)
    {
        Hex centerHex = GetHexAt(q, r);

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);

        foreach(Hex h in areaHexes)
        {
            //if(h.elevation < 0)
            //{
            //    h.elevation = 0;
            //}

            h.elevation = centerHeight * Mathf.Lerp( 1f, 0.25f, Hex.Distance(centerHex, h) / range );
            // affects height according to how far away a hex is from the center of the splat
        }

    }

}
