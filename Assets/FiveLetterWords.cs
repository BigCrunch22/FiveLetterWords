using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using KModkit;

public class FiveLetterWords : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public AudioClip[] SFX;

    public KMSelectable[] Selectables;
    public GameObject[] Disabler;

    public TextMesh[] WordDex;
    public TextAsset FiverData;

    private int[] TheValues = { 0, 0, 0 };
    private string[] TheNames = { "", "", "" };

    // Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        Selectables[0].OnInteract += delegate () { Number0(); return false; };
        Selectables[1].OnInteract += delegate () { Number1(); return false; };
        Selectables[2].OnInteract += delegate () { Number2(); return false; };
    }

    void Start()
    {
        Module.OnActivate += BombAnswer;
    }

    void BombAnswer()
    {
        string[] TheAnswer = JsonConvert.DeserializeObject<string[]>(FiverData.text).Shuffle();
        for (int i = 0; i < 3; i++)
        {
            TheNames[i] = TheAnswer[i];
            WordDex[i].text = TheAnswer[i];
            for (int j = 0; j < 5; j++)
            {
                if (Bomb.GetIndicators().Count() > 3 || Bomb.GetPortCount() > 3 || Bomb.GetBatteryCount() > 3 || Bomb.GetPortPlates().Count() > 3)
                {
                    if (TheAnswer[i][j].ToString() == "E" || TheAnswer[i][j].ToString() == "S" || TheAnswer[i][j].ToString() == "X")
                    {
                        TheValues[i] = TheValues[i] + 1;
                    }

                    else if (TheAnswer[i][j].ToString() == "A" || TheAnswer[i][j].ToString() == "K" || TheAnswer[i][j].ToString() == "N")
                    {
                        TheValues[i] = TheValues[i] + 2;
                    }

                    else if (TheAnswer[i][j].ToString() == "C" || TheAnswer[i][j].ToString() == "H" || TheAnswer[i][j].ToString() == "V" || TheAnswer[i][j].ToString() == "Z")
                    {
                        TheValues[i] = TheValues[i] + 3;
                    }

                    else if (TheAnswer[i][j].ToString() == "T" || TheAnswer[i][j].ToString() == "Y")
                    {
                        TheValues[i] = TheValues[i] + 4;
                    }

                    else if (TheAnswer[i][j].ToString() == "F" || TheAnswer[i][j].ToString() == "P" || TheAnswer[i][j].ToString() == "R" || TheAnswer[i][j].ToString() == "W")
                    {
                        TheValues[i] = TheValues[i] + 5;
                    }

                    else if (TheAnswer[i][j].ToString() == "B" || TheAnswer[i][j].ToString() == "G" || TheAnswer[i][j].ToString() == "L")
                    {
                        TheValues[i] = TheValues[i] + 6;
                    }

                    else if (TheAnswer[i][j].ToString() == "I")
                    {
                        TheValues[i] = TheValues[i] + 7;
                    }

                    else if (TheAnswer[i][j].ToString() == "D" || TheAnswer[i][j].ToString() == "J" || TheAnswer[i][j].ToString() == "M")
                    {
                        TheValues[i] = TheValues[i] + 8;
                    }

                    else
                    {
                        TheValues[i] = TheValues[i] + 9;
                    }

                }

                else
                {
                    if (TheAnswer[i][j].ToString() == "F" || TheAnswer[i][j].ToString() == "K" || TheAnswer[i][j].ToString() == "Y")
                    {
                        TheValues[i] = TheValues[i] + 1;
                    }

                    else if (TheAnswer[i][j].ToString() == "N" || TheAnswer[i][j].ToString() == "W" || TheAnswer[i][j].ToString() == "X" || TheAnswer[i][j].ToString() == "Y")
                    {
                        TheValues[i] = TheValues[i] + 2;
                    }

                    else if (TheAnswer[i][j].ToString() == "H" || TheAnswer[i][j].ToString() == "L" || TheAnswer[i][j].ToString() == "Q")
                    {
                        TheValues[i] = TheValues[i] + 3;
                    }

                    else if (TheAnswer[i][j].ToString() == "A" || TheAnswer[i][j].ToString() == "C")
                    {
                        TheValues[i] = TheValues[i] + 4;
                    }

                    else if (TheAnswer[i][j].ToString() == "G" || TheAnswer[i][j].ToString() == "S" || TheAnswer[i][j].ToString() == "U")
                    {
                        TheValues[i] = TheValues[i] + 5;
                    }

                    else if (TheAnswer[i][j].ToString() == "D" || TheAnswer[i][j].ToString() == "I" || TheAnswer[i][j].ToString() == "V")
                    {
                        TheValues[i] = TheValues[i] + 6;
                    }

                    else if (TheAnswer[i][j].ToString() == "J" || TheAnswer[i][j].ToString() == "M" || TheAnswer[i][j].ToString() == "O")
                    {
                        TheValues[i] = TheValues[i] + 7;
                    }

                    else if (TheAnswer[i][j].ToString() == "B" || TheAnswer[i][j].ToString() == "P" || TheAnswer[i][j].ToString() == "R")
                    {
                        TheValues[i] = TheValues[i] + 8;
                    }

                    else
                    {
                        TheValues[i] = TheValues[i] + 9;
                    }
                }
            }
        }

        if ((TheValues[0] == TheValues[1]) || (TheValues[0] == TheValues[2]) || (TheValues[1] == TheValues[2]))
        {
            TheValues[0] = 0; TheValues[1] = 0; TheValues[2] = 0;
            BombAnswer();
        }

        else
        {
            Debug.LogFormat("[Five Letter Words #{0}] The words are: {1}", moduleId, string.Join(", ", TheNames.Select(x => x.ToString()).ToArray()));
            Debug.LogFormat("[Five Letter Words #{0}] The word scores are: {1}", moduleId, string.Join(", ", TheValues.Select(x => x.ToString()).ToArray()));
        }
    }

    void Number0()
    {
        Debug.LogFormat("[Five Letter Words #{0}] You pressed {1} when the last digits of the bomb were {2}", moduleId, TheNames[0], (((int)Bomb.GetTime()) % 60).ToString());
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((TheValues[0] > TheValues[1]) && (TheValues[0] > TheValues[2]))
        {
            if (TheValues[1] < TheValues[2])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[1])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }

            else if (TheValues[2] < TheValues[1])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[2])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }
        }

        else
        {
            StartCoroutine(RouletteWrong());
        }
    }

    void Number1()
    {
        Debug.LogFormat("[Five Letter Words #{0}] You pressed {1} when the last digits of the bomb were {2}", moduleId, TheNames[1], (((int)Bomb.GetTime()) % 60).ToString());
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((TheValues[1] > TheValues[0]) && (TheValues[1] > TheValues[2]))
        {
            if (TheValues[0] < TheValues[2])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[0])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }

            else if (TheValues[2] < TheValues[0])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[2])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }
        }

        else
        {
            StartCoroutine(RouletteWrong());
        }
    }

    void Number2()
    {
        Debug.LogFormat("[Five Letter Words #{0}] You pressed {1} when the last digits of the bomb were {2}", moduleId, TheNames[2], (((int)Bomb.GetTime()) % 60).ToString());
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((TheValues[2] > TheValues[0]) && (TheValues[2] > TheValues[1]))
        {
            if (TheValues[0] < TheValues[1])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[0])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }

            else if (TheValues[1] < TheValues[0])
            {
                if (((int)Bomb.GetTime()) % 60 == TheValues[1])
                {
                    StartCoroutine(RouletteCheck());
                }

                else
                {
                    StartCoroutine(RouletteWrong());
                }
            }
        }

        else
        {
            StartCoroutine(RouletteWrong());
        }
    }

    IEnumerator RouletteCheck()
    {
        foreach (GameObject d in Disabler)
            d.SetActive(false);

        string[] TheAnswer = JsonConvert.DeserializeObject<string[]>(FiverData.text).Shuffle();
        for (int i = 0; i < 100; i++)
        {
            foreach (TextMesh t in WordDex)
                t.text = TheAnswer.PickRandom();
            yield return new WaitForSecondsRealtime(0.01f);
        }

        foreach (TextMesh t in WordDex)
        {
            t.text = "YES!";
            t.color = Color.green;
        }
        Module.HandlePass();
        ModuleSolved = true;
        Audio.PlaySoundAtTransform(SFX[2].name, transform);
        Debug.LogFormat("[Five Letter Words #{0}] Correct! Module solved.", moduleId);
    }

    IEnumerator RouletteWrong()
    {
        foreach (GameObject d in Disabler)
            d.SetActive(false);

        string[] TheAnswer = JsonConvert.DeserializeObject<string[]>(FiverData.text).Shuffle();
        for (int i = 0; i < 100; i++)
        {
            foreach (TextMesh t in WordDex)
                t.text = TheAnswer.PickRandom();
            yield return new WaitForSecondsRealtime(0.01f);
        }

        foreach (TextMesh t in WordDex)
        {
            t.text = "NO!";
            t.color = Color.red;
        }

        Audio.PlaySoundAtTransform(SFX[1].name, transform);
        yield return new WaitForSecondsRealtime(0.5f);

        Module.HandleStrike();
        Debug.LogFormat("[Five Letter Words #{0}] Incorrect! Strike! Module resetting...", moduleId);

        foreach (TextMesh t in WordDex)
        {
            t.color = Color.white;
        }

        string[] TheAnswerGenerates = JsonConvert.DeserializeObject<string[]>(FiverData.text).Shuffle();
        for (int i = 0; i < 50; i++)
        {
            foreach (TextMesh t in WordDex)
                t.text = TheAnswerGenerates.PickRandom();
            yield return new WaitForSecondsRealtime(0.01f);
        }

        foreach (GameObject d in Disabler)
            d.SetActive(true);

        TheValues[0] = 0; TheValues[1] = 0; TheValues[2] = 0;
        BombAnswer();
    }

    // Twitch Plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} press <top/middle/bottom> <#> [Presses the button in that position when the last two digits of the timer are #.]";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string input)
    {
        var cmd = input.ToLowerInvariant().Split(' ').ToArray();
        if (cmd.Length != 3)
            yield break;
        var numbers = Enumerable.Range(0, 60).Select(x => x.ToString()).ToArray();
        var positions = new string[] { "top", "middle", "bottom" };
        if (cmd[0] != "press" || !positions.Contains(cmd[1]) || !numbers.Contains(cmd[2]))
            yield break;
        while ((((int)Bomb.GetTime()) % 60) != Array.IndexOf(numbers, cmd[2]))
            yield return "trycancel The command to perform the action was cancelled due to a cancel request.";
        yield return null;
        yield return "solve";
        yield return "strike";
        Selectables[Array.IndexOf(positions, cmd[1])].OnInteract();
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        var mx = Array.IndexOf(TheValues, TheValues.Max());
        var mn = TheValues.Min();
        while ((((int)Bomb.GetTime()) % 60) != mn)
            yield return null;
        Selectables[mx].OnInteract();
        while (!ModuleSolved)
        {
            yield return null;
            yield return new WaitForSeconds(.1f);
        }
    }
}
