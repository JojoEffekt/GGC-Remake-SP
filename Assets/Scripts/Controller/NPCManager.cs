using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// managed npcs die im cafe bedient werden
public class NPCManager : MonoBehaviour
{
    /*
    liste die die aktuellen npcs beinhaltet

    list die die aktuell bedienbaren npc beinhaltet sowie die zeit wie lange sie noch bedienbar sind

    random generierung von npcs anhand von der bewertung des cafes

    berechne das grid (get)

    berechne alle möglichen sitzpositionen (get)

    erzeugt neuen npc
    npc sucht nach tisch mit stuhl (leer) -> npc geht dahin
        warte 30 sec -> gehe aus cafe
    */


    //sucht im gesamten spielfeld nach einem freien platz für ein npc wo der spieler sich
    //an einem tisch setzten kann, gibt eine position neben einen stuhl wieder
    public static int[] getNPCPosition(){
        return new int[]{0,0};
    }
}
