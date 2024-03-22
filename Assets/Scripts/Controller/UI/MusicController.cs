using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource audio;

    public Image imgMusicOnBtn; 

    public Sprite MusicOn;
    public Sprite MusicOff;

    //CONTINUE
    //speicher Mute/Unmute in playersettings und lade zu begin

    //toggelt die music an aus und rendert button
    public void MusicToggle(){
        if(audio.mute){
            //Aktiviert die audio
            audio.mute = false;

            //läd den button auf unmute
            imgMusicOnBtn.sprite = MusicOn;
        }else{
             //deaktiviert die audio
            audio.mute = true;

            //läd den button auf mute
            imgMusicOnBtn.sprite = MusicOff;
        }
    }
}
