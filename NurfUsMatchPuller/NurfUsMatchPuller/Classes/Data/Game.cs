//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NurfUsMatchPuller.Classes.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Game
    {
        public long GameId { get; set; }
        public string GameMode { get; set; }
        public int GameQueueConfigId { get; set; }
        public long GameStartTime { get; set; }
        public string GameType { get; set; }
        public int MapId { get; set; }
        public string EncryptionKey { get; set; }
        public string PlatformId { get; set; }
        public Nullable<int> SearchId { get; set; }
        public bool CurrentGame { get; set; }
        public System.DateTime DateStamp { get; set; }
    }
}