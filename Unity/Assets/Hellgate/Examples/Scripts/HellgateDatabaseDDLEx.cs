//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

/*
[Table ("table_sample1", true)]
public class HellgateSampleTableEx
{
    [Column (DataConstraints.AI)]
    private int idx;
    [Column (new DataConstraints[] {
        DataConstraints.NOTNULL, DataConstraints.UNIQUE
    })]
    private string column1;
    protected float column2;
    // public will not be added to this column.
    public string temp;
}

[Table (true)]
public class Table_sample2
{
    [Column (DataConstraints.AI)]
    private int idx;
    private string column1;
    private float column2;
    private bool column3;
}
//*/