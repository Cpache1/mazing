using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLHelperClasses
{
    public class RFData 
    {
        public float idleTime { get; set; }
        public float score { get; set; }
        public float botDistanceTraveled { get; set; }

        public float botPositionX { get; set; }
        public float botPositionY { get; set; }
        public float botRotation { get; set; }
        public float botSpeed { get; set; }
        public float botRotationSpeed { get; set; }
        public float botViewAngle { get; set; }
        public float botViewRadius { get; set; }
        public float botSearching { get; set; }
        public float botSearchTurns { get; set; }
        public float botHearingRadius { get; set; }
        public float botHearingProbability { get; set; }
        public float botHealth { get; set; }
        public float botFrustration { get; set; }
        public float botRiskTakingFactor { get; set; }
        public float botTakingRiskyPath { get; set; }
        public float botSeeingPlayer { get; set; }
        public float botChasingPlayer { get; set; }

        public float botDistanceFromPlayer { get; set; }
        public float cursorDistanceFromPlayer { get; set; }
        public float cursorDistanceFromBot { get; set; }
        public float playerDistanceTravelled { get; set; }
        public float playerPositionX { get; set; }
        public float playerPositionY { get; set; }
        public float playerRotation { get; set; }
        public float playerHealth { get; set; }
        public float playerIsDashing { get; set; }
        public float playerTriesDashOnCD { get; set; }

        public float dashPressed { get; set; }
        public float cursorDistanceTraveled { get; set; }
        public float cursorPositionX { get; set; }
        public float cursorPositionY { get; set; }
        public float playerTriesToFireOnCD { get; set; }
        public float playerTriesToBombOnCD { get; set; }
        public float shotsFired { get; set; }
        public float bombDropped { get; set; }
        public float gunReloading { get; set; }
        public float bombReloading { get; set; }

        public float playerBurning { get; set; }
        public float playerHealing { get; set; }
        public float playerDeltaHealth { get; set; }
        public float playerDied { get; set; }
        public float botLostPlayer { get; set; }
        public float botSpottedPlayer { get; set; }
        public float botBurning { get; set; }
        public float botDeltaHealth { get; set; }
        public float botDied { get; set; }

        public float onScreenFires { get; set; }
        public float onScreenBullets { get; set; }

        public float keyPressCount { get; set; }
        public float gen_timePassed { get; set; }
        public float gen_inputIntensity { get; set; }
        public float gen_inputDiversity { get; set; }
        public float gen_activity { get; set; }
        public float gen_score { get; set; }

    }

    public class RFPrediction
    {
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
