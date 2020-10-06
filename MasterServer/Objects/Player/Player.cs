using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterServer
{
    public class Player
    {
        public Player()
        {
            Achievements = new bool[Enum.GetNames(typeof(Achievement)).Length];
            for (int i = 0; i < Achievements.Length; i++)
                Achievements[i] = false;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public DateTime CreationTime { get; set; }
        public bool[] Achievements { get; set; }
    }
}
