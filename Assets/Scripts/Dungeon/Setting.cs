using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting: MonoBehaviour
{
    public static int Seed;
    public static bool UseSeed;
    public static int DungeonSize;

    private void Start()
    {
        Seed = 0;
        UseSeed = false;
        DungeonSize = 40;
    }

    public void SetSeed(string seedString)
    {
        if (seedString == null) return;
        if (int.TryParse(seedString, out var seed))
        {
            Seed = seed;
        }

    }
    
    public void SetSize(string sizeString)
    {
        if (sizeString == null) return;
        if (int.TryParse(sizeString, out var size))
        {
            DungeonSize = size;
        }

       
    }

    public void ShouldUseSeed(bool use)
    {
        UseSeed = use;
    }

    public void ChangeScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
