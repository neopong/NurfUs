using NurfUsMatchPuller.Classes.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Models.API
{
    public class Participant
    {
        public bool Bot { get; set; }
        public long ChampionId { get; set; }
        public Mastery[] Masteries { get; set; }
        public int ParticipantId { get; set; }
        public long ProfileIconId { get; set; }
        public Rune[] Runes { get; set; }
        public long Spell1Id { get; set; }
        public long Spell2Id { get; set; }
        public ParticipantStats Stats { get; set; }
        public long SummonerId { get; set; }
        public string SummonerName { get; set; }
        public long TeamId { get; set; }
    }
}