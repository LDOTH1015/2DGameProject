using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManagerScript : Singleton<GameManagerScript>
{
    public int StageCount;
    public bool isStageStarted;

    private void Start()
    {
        StageCount = 9;
        isStageStarted = false;
    }


}
