using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public AlienHand alienHand;

    public int alienDamage;

    private void Start()
    {
       alienHand.damage = alienDamage;
    }
}
