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

        public Audience(Audience audiecne)
        {
            audienceType = audiecne.audienceType;
        }
    }

