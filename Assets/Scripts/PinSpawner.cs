using System;
using System.Collections.Generic;
using UnityEngine;

public class PinSpawner : MonoBehaviour
{
    [SerializeField] GameObject PinPrefab;
    Bounds PinBounds;
    Vector3 extents = Vector3.zero; // Extents: (0.33, 1.04, 0.33)
    readonly Vector3 MeshRendererOffset = new Vector3(-0.32f, 0.5f, 0.27f); // To fix imported mesh bad center
    public readonly List<GameObject> pins = new List<GameObject>();

    private TextAsset StageFile;
    private string[] lines;
    readonly char spawn = 'v', skip = 'x', center = 'c';
    [SerializeField] float HorizontalPinsSpace = 0.05f, VerticalPinsSpace = 0.05f;


    public void LoadPinsFromFile(string StageFilePath)
    {
        StageFile = Resources.Load(StageFilePath) as TextAsset;
        if (StageFile == null)
        {
            throw new Exception($"Stage file not found");
        }
        lines = StageFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        Vector3 SpawnLocation = Vector3.zero;
        Vector3 CenterLocation = Vector3.zero;
        for (int i = 0; i < lines.Length; ++i)
        {
            for (int j = 0; j < lines[i].Length; ++j)
            {
                char c = lines[i][j];
                if (c == spawn)
                {
                    pins.Add(InsPin(SpawnLocation));
                }
                else if (c == center)
                {
                    CenterLocation = SpawnLocation;
                    if (j + 1 < lines[i].Length)
                    {
                        if (lines[i][j + 1] != ' ')
                        {
                            continue;
                        }
                        else
                        {
                            Debug.Log($"Notice: Treated '{center}' as '{skip}'.");
                        }
                    }
                }
                else if (c != skip && c != ' ')
                {
                    throw new Exception($"Invalid character {c} in {StageFilePath}.");
                }
                SpawnLocation.x += extents.x + HorizontalPinsSpace;

            }
            SpawnLocation.z -= extents.z + VerticalPinsSpace;
            SpawnLocation.x = 0;
        }
        foreach (GameObject pin in pins)
        {
            pin.transform.position -= CenterLocation;
        }
    }

    private GameObject InsPin(Vector3 location)
    {
        location += MeshRendererOffset;
        GameObject ans = Instantiate(PinPrefab, location, Quaternion.identity);
        if (extents == Vector3.zero)
        {
            PinBounds = ans.GetComponent<MeshRenderer>().bounds;
            extents = PinBounds.extents;
        }
        return ans;
    }
}
