using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Microsoft Mixed Reality Toolkit
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using Microsoft.MixedReality.OpenXR;

public class FollowEyeGaze : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled) Debug.Log("Eye tracking is not enabled.");
    }

    // Update is called once per frame
    void Update()
    {

        if (CoreServices.InputSystem.EyeGazeProvider.HitInfo.raycastValid)
        {
            gameObject.transform.position = CoreServices.InputSystem.EyeGazeProvider.HitPosition;
            if (CoreServices.InputSystem.EyeGazeProvider.HitInfo.transform.tag == "Target")
            {
                Debug.Log("hit position found " + CoreServices.InputSystem.EyeGazeProvider.HitInfo.transform.name);
            }
        }
        else
        {
            gameObject.transform.position = gameObject.transform.position = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin +
            CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized * 0.5f;
        }
    }
}
