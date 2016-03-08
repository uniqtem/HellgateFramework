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
    private Monster monster;
}

[Excel ("monster", "monster")]
public class Monster
{
    [Column (typeof (Avatar), "idx")]
    private int idx;
    [Column (typeof (Avatar), "name")]
    private string name;
    private string description;
}
