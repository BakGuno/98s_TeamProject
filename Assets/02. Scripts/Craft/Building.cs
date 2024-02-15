using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum Type
    {
        Normal,
        Wall,
        Foundation,
        Pillar
    }

    public Type type;
}
