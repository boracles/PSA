using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum AudienceType
    {
        FOCUS, NONFOCUS
    }
    
    [System.Serializable]
    public class Audience
    {
        public AudienceType audienceType;

        public Audience(Audience audience)
        {
            audienceType = audience.audienceType;
        }
    }

