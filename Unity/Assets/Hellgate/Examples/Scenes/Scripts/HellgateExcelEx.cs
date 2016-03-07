using UnityEngine;
using System.Collections;
using Hellgate;

[Excel ("avatar", "avatar", true)]
public class Avatar
{
    [Column (DataConstraints.PK)]
    private int idx;
    [Column (DataConstraints.PK)]
    private string name;
    private string description;
    private string attack;
    private string defence;
    private int speed;
}

[Excel ("monster", "monster")]
public class Monster
{
    private int idx;
    private string name;
    private string description;
}

