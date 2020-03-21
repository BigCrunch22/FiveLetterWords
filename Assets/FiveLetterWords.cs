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

    public KMSelectable[] Buttons;
    public GameObject[] Disabler;

    public TextMesh[] Displays;
    public TextAsset WordBank;

    private int[] WordValues = { 0, 0, 0 };

    // Logging
    static int moduleIdCounter = 1;
    int moduleId;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        Buttons[0].OnInteract += delegate () { PressButton0(); return false; };
        Buttons[1].OnInteract += delegate () { PressButton1(); return false; };
        Buttons[2].OnInteract += delegate () { PressButton2(); return false; };
        Module.OnActivate += GenerateAnswer;
    }

    void GenerateAnswer()
    {
        string[] AllWords = JsonConvert.DeserializeObject<string[]>(WordBank.text).Shuffle();
        string[] Ordinals = new string[] { "first", "second", "third" };
        TryAgain:
        WordValues[0] = 0; WordValues[1] = 0; WordValues[2] = 0;
        for (int i = 0; i < 3; i++)
        {
            Displays[i].text = AllWords[i];

            for (int j = 0; j < 5; j++)
            {
                if (Bomb.GetIndicators().Count() > 3 || Bomb.GetPortCount() > 3 || Bomb.GetBatteryCount() > 3 || Bomb.GetPortPlates().Count() > 3)
                {
                    if (i == 0 && j == 0)
                        Debug.LogFormat("[Five Letter Words #{0}] Using the first table.", moduleId);
                    if (AllWords[i][j].ToString() == "E" || AllWords[i][j].ToString() == "S" || AllWords[i][j].ToString() == "X")
                        WordValues[i] = WordValues[i] + 1;
                    else if (AllWords[i][j].ToString() == "A" || AllWords[i][j].ToString() == "K" || AllWords[i][j].ToString() == "N")
                        WordValues[i] = WordValues[i] + 2;
                    else if (AllWords[i][j].ToString() == "C" || AllWords[i][j].ToString() == "H" || AllWords[i][j].ToString() == "V" || AllWords[i][j].ToString() == "Z")
                        WordValues[i] = WordValues[i] + 3;
                    else if (AllWords[i][j].ToString() == "T" || AllWords[i][j].ToString() == "Y")
                        WordValues[i] = WordValues[i] + 4;
                    else if (AllWords[i][j].ToString() == "F" || AllWords[i][j].ToString() == "P" || AllWords[i][j].ToString() == "R" || AllWords[i][j].ToString() == "W")
                        WordValues[i] = WordValues[i] + 5;
                    else if (AllWords[i][j].ToString() == "B" || AllWords[i][j].ToString() == "G" || AllWords[i][j].ToString() == "L")
                        WordValues[i] = WordValues[i] + 6;
                    else if (AllWords[i][j].ToString() == "I")
                        WordValues[i] = WordValues[i] + 7;
                    else if (AllWords[i][j].ToString() == "D" || AllWords[i][j].ToString() == "J" || AllWords[i][j].ToString() == "M")
                        WordValues[i] = WordValues[i] + 8;
                    else
                        WordValues[i] = WordValues[i] + 9;
                }
                else
                {
                    if (i == 0 && j == 0)
                        Debug.LogFormat("[Five Letter Words #{0}] Using the second table.", moduleId);
                    if (AllWords[i][j].ToString() == "F" || AllWords[i][j].ToString() == "K" || AllWords[i][j].ToString() == "Y")
                        WordValues[i] = WordValues[i] + 1;
                    else if (AllWords[i][j].ToString() == "N" || AllWords[i][j].ToString() == "W" || AllWords[i][j].ToString() == "X" || AllWords[i][j].ToString() == "Y")
                        WordValues[i] = WordValues[i] + 2;
                    else if (AllWords[i][j].ToString() == "H" || AllWords[i][j].ToString() == "L" || AllWords[i][j].ToString() == "Q")
                        WordValues[i] = WordValues[i] + 3;
                    else if (AllWords[i][j].ToString() == "A" || AllWords[i][j].ToString() == "C")
                        WordValues[i] = WordValues[i] + 4;
                    else if (AllWords[i][j].ToString() == "G" || AllWords[i][j].ToString() == "S" || AllWords[i][j].ToString() == "U")
                        WordValues[i] = WordValues[i] + 5;
                    else if (AllWords[i][j].ToString() == "D" || AllWords[i][j].ToString() == "I" || AllWords[i][j].ToString() == "V")
                        WordValues[i] = WordValues[i] + 6;
                    else if (AllWords[i][j].ToString() == "J" || AllWords[i][j].ToString() == "M" || AllWords[i][j].ToString() == "O")
                        WordValues[i] = WordValues[i] + 7;
                    else if (AllWords[i][j].ToString() == "B" || AllWords[i][j].ToString() == "P" || AllWords[i][j].ToString() == "R")
                        WordValues[i] = WordValues[i] + 8;
                    else
                        WordValues[i] = WordValues[i] + 9;
                }
            }
            Debug.LogFormat("[Five Letter Words #{0}] The {1} display says {2}.", moduleId, Ordinals[i], Displays[i].text.ToLowerInvariant());
        }
        if ((WordValues[0] == WordValues[1]) || (WordValues[0] == WordValues[2]) || (WordValues[1] == WordValues[2]))
            goto TryAgain;
        else
        {
            Debug.LogFormat("[Five Letter Words #{0}] The word scores are: {1}", moduleId, string.Join(", ", WordValues.Select(x => x.ToString()).ToArray()));
            Debug.LogFormat("[Five Letter Words #{0}] You need to press {1} when the seconds part of the timer says {2}.", moduleId, Displays[Array.IndexOf(WordValues, WordValues.Max())].text.ToLowerInvariant(), WordValues.Min());
        }
    }

    void PressButton0()
    {
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((WordValues[0] > WordValues[1]) && (WordValues[0] > WordValues[2]))
        {
            if (WordValues[1] < WordValues[2])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[1])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
            else if (WordValues[2] < WordValues[1])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[2])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
        }
        else
            StartCoroutine(RouletteWrong());
    }

    void PressButton1()
    {
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((WordValues[1] > WordValues[0]) && (WordValues[1] > WordValues[2]))
        {
            if (WordValues[0] < WordValues[2])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[0])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
            else if (WordValues[2] < WordValues[0])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[2])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
        }
        else
            StartCoroutine(RouletteWrong());
    }

    void PressButton2()
    {
        Audio.PlaySoundAtTransform(SFX[0].name, transform);
        if ((WordValues[2] > WordValues[0]) && (WordValues[2] > WordValues[1]))
        {
            if (WordValues[0] < WordValues[1])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[0])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
            else if (WordValues[1] < WordValues[0])
            {
                if (((int)Bomb.GetTime()) % 60 == WordValues[1])
                    StartCoroutine(RouletteCheck());
                else
                    StartCoroutine(RouletteWrong());
            }
        }
        else
            StartCoroutine(RouletteWrong());
    }

    IEnumerator RouletteCheck()
    {
        foreach (GameObject i in Disabler)
            i.SetActive(false);
        string[] AllWords = JsonConvert.DeserializeObject<string[]>(WordBank.text).Shuffle();
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 3; j++)
                Displays[j].text = AllWords[UnityEngine.Random.Range(0, AllWords.Length)];
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for (int i = 0; i < 3; i++)
        {
            Displays[i].text = "YES!";
            Displays[i].color = Color.green;
        }
        Module.HandlePass();
        Debug.LogFormat("[Five Letter Words #{0}] Module solved!", moduleId);
        Audio.PlaySoundAtTransform(SFX[2].name, transform);
    }

    IEnumerator RouletteWrong()
    {
        for (int i = 0; i < 3; i++)
            Disabler[i].SetActive(false);
        string[] AllWords = JsonConvert.DeserializeObject<string[]>(WordBank.text).Shuffle();
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 3; j++)
                Displays[j].text = AllWords[UnityEngine.Random.Range(0, AllWords.Length)];
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for (int i = 0; i < 3; i++)
        {
            Displays[i].text = "NO!";
            Displays[i].color = Color.red;
        }
        Audio.PlaySoundAtTransform(SFX[1].name, transform);
        yield return new WaitForSecondsRealtime(0.5f);
        Module.HandleStrike();
        Debug.LogFormat("[Five Letter Words #{0}] Strike! Resetting...", moduleId);
        foreach (TextMesh Display in Displays)
            Display.color= Color.white;
        string[] TheAnswerGenerates = JsonConvert.DeserializeObject<string[]>(WordBank.text).Shuffle();
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 3; j++)
                Displays[j].text = TheAnswerGenerates[UnityEngine.Random.Range(0, TheAnswerGenerates.Length)];
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for (int i = 0; i < 3; i++)
            Disabler[i].SetActive(true);
        for (int i = 0; i < 3; i++)
            WordValues[i] = 0;
        GenerateAnswer();
    }
}
