using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUICanvas : MonoBehaviour
{

    public TextMeshProUGUI timerUI;

    private GameTimer global_timer_script;

    private string prev_text;

    void Start()
    {
        GameObject global_timer = GameObject.Find("GlobalTimer");
        global_timer_script = global_timer.GetComponent<GameTimer>();
    }


    void Update()
    {
        //write remaining time info on player's canvas
        if(prev_text != global_timer_script.local_timer)
        {
            timerUI.text = global_timer_script.local_timer;
            prev_text = global_timer_script.local_timer;
        }
        
    }
}
