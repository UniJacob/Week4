using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    /* Stage File Parse */
    [Tooltip("Path from 'Resources' folder")]
    [SerializeField] string StageFileNames = "stage1,stage2";
    TextAsset StageFile;
    string[] stageNames;
    int currentStage = 0;
    readonly char spawn = 'v', skip = 'x', center = 'c';

    /* Ball Related */
    [SerializeField] GameObject Ball;
    [SerializeField] Vector3 BallDefaultPosition;
    bool[] HasFallen;

    /* Pin Related */
    [SerializeField] GameObject PinPrefab;
    [Tooltip("How many degrees the y-axis of the pin should be from its starting position for" +
        "it to be considered as fallen")]
    [SerializeField] float FallTreshold = 30;
    [Tooltip("Extra space between adjacent pins")]
    [SerializeField] float HorizontalPinsSpace = 0.05f, VerticalPinsSpace = 0.05f;
    List<GameObject> pins = new List<GameObject>();
    Vector3 PinExtents = Vector3.zero; // Extents: (0.33, 1.04, 0.33)
    readonly Vector3 MeshRendererOffset = new Vector3(-0.32f, 0.5f, 0.27f); // To fix imported mesh bad center
    Vector3[,] DefaultPinsPosAndRot;


    /* Stage Settings */
    [Tooltip("Time to topple all of the pins after the ball was thrown")]
    [SerializeField] float Timer = 10;
    [SerializeField] int fontSize = 16;
    [SerializeField] Color TextColor = Color.white;
    [SerializeField] int NumberOfTriesPerStage = 2;
    float TimeLeft;
    readonly int xLeft = 25;
    readonly int widthLetters = 20;
    int CurrTry = 1;
    int score = 0;

    private void Awake()
    {
        Ball.transform.position = BallDefaultPosition;
        TimeLeft = Timer;
        NumberOfTriesPerStage = Math.Max(1, NumberOfTriesPerStage);
        stageNames = StageFileNames.Split(',');
        SetStage();
    }
    private void SetStage()
    {
        LoadPinsFromFile(stageNames[currentStage]);
        HasFallen = new bool[pins.Count];
        DefaultPinsPosAndRot = new Vector3[2, pins.Count];
        SavePosAndRot();
    }

    private void Update()
    {
        if (!Ball.GetComponent<DragAndLaunch>().WasReleased)
        {
            return;
        }
        int i = 0;
        foreach (GameObject pin in pins)
        {
            if (!HasFallen[i])
            {
                var tmp = Vector3.Angle(pin.transform.up, Vector3.up);
                if (tmp > FallTreshold) // If pin has fallen
                {
                    HasFallen[i] = true;
                    if (++score == pins.Count)
                    {
                        ThrowFinished(true);
                        return;
                    }
                }
            }
            ++i;
        }
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0)
        {
            TimeLeft = 0;
            ThrowFinished(false);
        }
    }
    private void LoadPinsFromFile(string StageFilePath)
    {
        StageFile = Resources.Load(StageFilePath) as TextAsset;
        if (StageFile == null)
        {
            throw new Exception($"Stage file not found");
        }
        string[] lines = StageFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

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
                SpawnLocation.x += PinExtents.x + HorizontalPinsSpace;

            }
            SpawnLocation.z -= PinExtents.z + VerticalPinsSpace;
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
        if (PinExtents == Vector3.zero)
        {
            var PinBounds = ans.GetComponent<MeshRenderer>().bounds;
            PinExtents = PinBounds.extents;
        }
        return ans;
    }
    private void SavePosAndRot()
    {
        int i = 0;
        foreach (GameObject pin in pins)
        {
            DefaultPinsPosAndRot[0, i] = pin.transform.position;
            DefaultPinsPosAndRot[1, i] = pin.transform.rotation.eulerAngles;
            ++i;
        }
    }
    private void ThrowFinished(bool won)
    {
        Ball.transform.position = BallDefaultPosition;
        Ball.GetComponent<DragAndLaunch>().Restart();
        TimeLeft = Timer;
        if (!won && CurrTry < NumberOfTriesPerStage)
        {
            Debug.Log("TRY AGAIN");
            ++CurrTry;
            ReplacePins(false);
            return;
        }
        if (won)
        {
            Debug.Log("WON");
            Win();
        }
        else
        {
            Debug.Log("LOST");
            ReplacePins(true);
        }
        score = 0;
        CurrTry = 1;
    }
    private void ReplacePins(bool restart)
    {
        int i = 0;
        foreach (GameObject pin in pins)
        {
            if (restart)
            {
                HasFallen[i] = false;
                pin.SetActive(true);
            }
            else if (HasFallen[i])
            {
                pin.SetActive(false);
            }
            pin.transform.position = DefaultPinsPosAndRot[0, i];
            Quaternion q = new Quaternion();
            q.eulerAngles = DefaultPinsPosAndRot[1, i];
            pin.transform.rotation = q;

            var rb = pin.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            ++i;
        }
    }
    private void Win()
    {
        ++currentStage;
        if (currentStage >= stageNames.Length)
        {
            SceneManager.LoadScene("YouWin");
        }
        else
        {
            foreach (GameObject pin in pins)
            {
                Destroy(pin);
            }
            pins = new List<GameObject>();
            SetStage();
        }
    }
    void OnGUI()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        fontStyle.fontSize = fontSize;
        fontStyle.normal.textColor = TextColor;
        int width = fontSize * widthLetters;
        int lineHeight = (int)(fontSize * 1.5);
        GUI.Label(new Rect(xLeft, 0, width, lineHeight),
            "Stage Number: " + (currentStage + 1) + " / " + stageNames.Length, fontStyle);
        GUI.Label(new Rect(xLeft, 1 * lineHeight, width, lineHeight),
            "Fallen Pins: " + score + " / " + pins.Count, fontStyle);
        GUI.Label(new Rect(xLeft, 2 * lineHeight, width, lineHeight),
            "Time Left: " + (TimeLeft).ToString("F2") + "s", fontStyle);
        GUI.Label(new Rect(xLeft, 3 * lineHeight, width, lineHeight),
            "Try Number: " + (CurrTry) + " / " + NumberOfTriesPerStage, fontStyle);
    }
}
