using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Classes.Betting
{
    public abstract class BetEvent
    {

        //LOL Events are defined as One Entity Performing an action on another entity
        //At least one entity is a champion
        //In this class setup the SuperiorEntity is the one that Does the Action to the inferior entity
        //The Entities can be a Champion, Team, turret
        public BetEntity SuperiorEntity
        {
            get;
            set;
        }
        public BetEntity InferiorEntity
        {
            get;
            set;
        }
        public BetEntity[] AssistingEntities
        {
            get;
            set;
        }
        public BetEntity[] UnrelatedEntities
        {
            get;
            set;
        }

        public BetEvent(BetEntity superior, BetEntity inferior)
        {
            this.SuperiorEntity = superior;
            this.InferiorEntity = inferior;
        }
    }
}