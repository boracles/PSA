using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceController : MonoBehaviour
{
    public Audience audience;

    public void SetAudience(AudienceType _audienceType)
    {
        audience.audienceType = _audienceType;
    }

    public Audience GetAudience()
    {
        return audience;
    }
}
