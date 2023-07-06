using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using UnityEngine;
using System;

#if WINDOWS_UWP
using Windows.Storage;
#endif

// Microsoft Mixed Reality Toolkit
using Microsoft.MixedReality.Toolkit;

public class DataLogger : MonoBehaviour
{
#region constants
    private string header;
    private string path;
#endregion

#region participant info
    [SerializeField]
    private string pid;
    #endregion

    #region data handling
    private StringBuilder data;
    private System.Threading.SemaphoreSlim fileSemaphore = new System.Threading.SemaphoreSlim(1);
#if WINDOWS_UWP
    private IStorageFile datafile;
#endif
#endregion

    // Start is called before the first frame update
    async void Start()
    {
        header = "timestamp," +
            "enabled," +
            "valid," +
            "camera_position_x," + "camera_position_y," + "camera_position_z," +
            "camera_rotation_x," + "camera_rotation_y," + "camera_rotation_z," +
            "head_direction_x," + "head_direction_y," + "head_direction_z," +
            "head_velocity_x," + "head_velocity_y," + "head_velocity_z," +
            "gaze_origin_x," + "gaze_origin_y," + "gaze_origin_z," +
            "gaze_direction_x," + "gaze_direction_y," + "gaze_direction_z," +
            "hit_name," +
            "hit_position_x," + "hit_position_y," + "hit_position_z," +
            "hit_normal_x," + "hit_normal_y," + "hit_normal_z," +
            "hit_distance_x," + "hit_distance_y," + "hit_distance_z," +
            "hit_texture_coords_x," + "hit_texture_coords_y";
        data = new StringBuilder();
        data.AppendLine(header);
        await CreateFile();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        string line = "";

        var gaze = CoreServices.InputSystem.EyeGazeProvider;
        line += gaze.Timestamp + ",";
        line += gaze.IsEyeTrackingEnabled + "," + gaze.IsEyeTrackingDataValid + ",";
        var camera = Camera.main.transform;
        line += camera.position.x + "," + camera.position.y + "," + camera.position.z + ",";
        line += camera.rotation.x + "," + camera.rotation.y + "," + camera.rotation.z + ",";
        if (gaze.IsEyeTrackingEnabledAndValid) {
            line += gaze.HeadMovementDirection.x + "," + gaze.HeadMovementDirection.y + "," + gaze.HeadMovementDirection.z + ",";
            line += gaze.HeadVelocity.x + "," + gaze.HeadVelocity.y + "," + gaze.HeadVelocity.z + ",";
            line += gaze.GazeOrigin.x + "," + gaze.GazeOrigin.y + "," + gaze.GazeOrigin.z + ",";
            line += gaze.GazeDirection.x + "," + gaze.GazeDirection.y + "," + gaze.GazeDirection.z + ",";
            line += gaze.HitInfo.transform.name + ",";
            line += gaze.HitPosition.x + "," + gaze.HitPosition.y + "," + gaze.HitPosition.z + ",";
            line += gaze.HitNormal.x + "," + gaze.HitNormal.y + "," + gaze.HitNormal.z + ",";
            line += gaze.HitInfo.distance + ",";
            line += gaze.HitInfo.textureCoord.x + "," + gaze.HitInfo.textureCoord.y + ",";
        }

        data.AppendLine(line);

        if (data.Length > 200) Flush();
    }

    async Task CreateFile()
    {
        string file = pid + ".csv";
        path += file;

#if WINDOWS_UWP
        StorageFolder fileParent = await KnownFolders.DocumentsLibrary.CreateFolderAsync("EyeData", CreationCollisionOption.OpenIfExists);
        path = fileParent.Path + "\\";
        datafile = await fileParent.CreateFileAsync(file, CreationCollisionOption.OpenIfExists);
#else
        path = Application.persistentDataPath;
#endif
        Debug.Log("Data logging to: " + path);
    }

    public async void Flush()
    {
#if WINDOWS_UWP
        await fileSemaphore.WaitAsync();
        try
        {
            await FileIO.AppendTextAsync(datafile, data.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
        finally{
            fileSemaphore.Release();
        }
#endif

        using(var sw = new StreamWriter(path, true))
        {
            sw.Write(data.ToString());
        }
        data.Clear();
    }

    private void OnDestroy()
    {
        if(data != null) Flush();
    }
}
