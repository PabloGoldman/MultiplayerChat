using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public string playerName;
    public string playerDescription;

    [Net] public int life;
    [Net] public int damage;
    [Net] public int points;


}
