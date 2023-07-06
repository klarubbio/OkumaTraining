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
        Debug.Log("enable eye tracking ? " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Gaze info: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid + 
            " " + CoreServices.InputSystem.EyeGazeProvider.HitInfo);

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
            Debug.Log("no hit found");
        }
    }
}
