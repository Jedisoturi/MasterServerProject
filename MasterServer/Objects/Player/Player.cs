﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Player
{
    public Player()
    {
        achievements = new bool[Enum.GetNames(typeof(Achievement)).Length];
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Level { get; set; }
    public DateTime CreationTime { get; set; }
    public bool[] achievements;
}