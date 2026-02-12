using UnityEngine;
using System.Collections.Generic;
using UXF;
using VIVE.OpenXR;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.EyeTracker;

public class EyeTracking : Tracker
{
    public Transform tracker;
    public override string MeasurementDescriptor => "eye_tracking";

    public override IEnumerable<string> CustomHeader =>
        new string[] { 
            "left_gaze_origin_x",
            "left_gaze_origin_y",
            "left_gaze_origin_z",
            "left_gaze_direction_x",
            "left_gaze_direction_y",
            "left_gaze_direction_z",
            "right_gaze_origin_x",
            "right_gaze_origin_y",
            "right_gaze_origin_z",
            "right_gaze_direction_x",
            "right_gaze_direction_y",
            "right_gaze_direction_z",
            "combined_gaze_origin_x",
            "combined_gaze_origin_y",
            "combined_gaze_origin_z",
            "combined_gaze_direction_x",
            "combined_gaze_direction_y",
            "combined_gaze_direction_z",
            "eye_openness_left",
            "eye_openness_right",
            "pupil_diameter_left",
            "pupil_diameter_right",
            "focus_point_x",
            "focus_point_y",
            "focus_point_z",
            "focus_distance",
            "focus_object",
        };
    protected override UXFDataRow GetCurrentValues()
    {
        UXFDataRow row = new UXFDataRow();
        RecordEyeGaze(row);
        RecordEyeData(row);
        RecordEyeFocus(row);
        return row;
    }
    public void RegisterSelf()
    {
        Session.instance.trackedObjects.Add(this);
    }
    void RecordEyeGaze(UXFDataRow row)
    {
        XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes);

        XrSingleEyeGazeDataHTC leftGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
        if(leftGaze.isValid)
        {
            Vector3 leftOrigin = leftGaze.gazePose.position.ToUnityVector();
            row.Add(("left_gaze_origin_x", leftOrigin.x));
            row.Add(("left_gaze_origin_y", leftOrigin.y));
            row.Add(("left_gaze_origin_z", leftOrigin.z));

            Vector3 leftDirection = leftGaze.gazePose.orientation.ToUnityQuaternion().eulerAngles;
            row.Add(("left_gaze_direction_x", leftDirection.x));
            row.Add(("left_gaze_direction_y", leftDirection.y));
            row.Add(("left_gaze_direction_z", leftDirection.z));
        }
        else
        {
            row.Add(("left_gaze_origin_x", float.NaN));
            row.Add(("left_gaze_origin_y", float.NaN));
            row.Add(("left_gaze_origin_z", float.NaN));
            row.Add(("left_gaze_direction_x", float.NaN));
            row.Add(("left_gaze_direction_y", float.NaN));
            row.Add(("left_gaze_direction_z", float.NaN));
        }

        XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
        if(rightGaze.isValid)
        {
            Vector3 rightOrigin = rightGaze.gazePose.position.ToUnityVector();
            row.Add(("right_gaze_origin_x", rightOrigin.x));
            row.Add(("right_gaze_origin_y", rightOrigin.y));
            row.Add(("right_gaze_origin_z", rightOrigin.z));

            Vector3 rightDirection = rightGaze.gazePose.orientation.ToUnityQuaternion().eulerAngles;
            row.Add(("right_gaze_direction_x", rightDirection.x));
            row.Add(("right_gaze_direction_y", rightDirection.y));
            row.Add(("right_gaze_direction_z", rightDirection.z));
        }
        else
        {
            row.Add(("right_gaze_origin_x", float.NaN));
            row.Add(("right_gaze_origin_y", float.NaN));
            row.Add(("right_gaze_origin_z", float.NaN));
            row.Add(("right_gaze_direction_x", float.NaN));
            row.Add(("right_gaze_direction_y", float.NaN));
            row.Add(("right_gaze_direction_z", float.NaN));
        }
    }

    void RecordEyeData(UXFDataRow row)
    {
        //Record Pupil Info
        XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] out_pupils);
        
        XrSingleEyePupilDataHTC rightPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
        if(rightPupil.isDiameterValid){
            float rightPupilDiameter = rightPupil.pupilDiameter;
            row.Add(("pupil_diameter_right", rightPupilDiameter));
        }
        else
        {
            row.Add(("pupil_diameter_right", float.NaN));
        }

        XrSingleEyePupilDataHTC leftPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
        if(leftPupil.isDiameterValid){
            float leftPupilDiameter = leftPupil.pupilDiameter;
            row.Add(("pupil_diameter_left", leftPupilDiameter));
        }
        else
        {
            row.Add(("pupil_diameter_left", float.NaN));
        }

        //Record Eye Openness
        XR_HTC_eye_tracker.Interop.GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] out_geometrics);

        XrSingleEyeGeometricDataHTC rightGeometric = out_geometrics[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
        if(rightGeometric.isValid)
        {
            float rightEyeOpenness = rightGeometric.eyeOpenness;
            row.Add(("eye_openness_right", rightEyeOpenness));
        }
        else
        {
            row.Add(("eye_openness_right", float.NaN));
        }

        XrSingleEyeGeometricDataHTC leftGeometric = out_geometrics[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
        if(leftGeometric.isValid)
        {
            float leftEyeOpenness = leftGeometric.eyeOpenness;
            row.Add(("eye_openness_left", leftEyeOpenness));
        }
        else
        {
            row.Add(("eye_openness_left", float.NaN));
        }   
    }

    void RecordEyeFocus(UXFDataRow row)
    {
        Vector3 leftOrigin = Vector3.zero;
        Vector3 rightOrigin = Vector3.zero;

        Vector3 leftDirection = Vector3.zero;
        Vector3 rightDirection = Vector3.zero;

        Vector3 combinedOrigin = Vector3.zero;
        Vector3 combinedDirection = Vector3.zero;

        // Get raw individual eye data
        XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes);

        XrSingleEyeGazeDataHTC leftGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
        if (leftGaze.isValid)
        {
            leftOrigin = leftGaze.gazePose.position.ToUnityVector();

            // 🔧 FIX: use rotation * forward instead of eulerAngles
            leftDirection = leftGaze.gazePose.orientation.ToUnityQuaternion() * Vector3.forward;
        }

        XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
        if (rightGaze.isValid)
        {
            rightOrigin = rightGaze.gazePose.position.ToUnityVector();

            // 🔧 FIX: use rotation * forward instead of eulerAngles
            rightDirection = rightGaze.gazePose.orientation.ToUnityQuaternion() * Vector3.forward;
        }

        // Calculate combined eye data
        if (leftOrigin != Vector3.zero && rightOrigin != Vector3.zero)
            combinedOrigin = (leftOrigin + rightOrigin) / 2.0f;

        row.Add(("combined_gaze_origin_x", combinedOrigin.x));
        row.Add(("combined_gaze_origin_y", combinedOrigin.y));
        row.Add(("combined_gaze_origin_z", combinedOrigin.z));

        if (leftDirection != Vector3.zero && rightDirection != Vector3.zero)
            combinedDirection = leftDirection.normalized;//(leftDirection.normalized + rightDirection.normalized).normalized;

        row.Add(("combined_gaze_direction_x", combinedDirection.x));
        row.Add(("combined_gaze_direction_y", combinedDirection.y));
        row.Add(("combined_gaze_direction_z", combinedDirection.z));

        Transform hmd = Camera.main.transform;

        // Convert tracking-space gaze to world-space
        Vector3 worldOrigin = hmd.TransformPoint(combinedOrigin);
        Vector3 worldDirection = hmd.TransformDirection(combinedDirection);

        RaycastHit hit;
        if (Physics.Raycast(worldOrigin, worldDirection, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(worldOrigin, worldDirection * hit.distance, Color.yellow);
            Debug.Log("Currently looking at: " + hit.collider.gameObject.name);
            tracker.position = hit.point;

            row.Add(("focus_point_x", hit.point.x));
            row.Add(("focus_point_y", hit.point.y));
            row.Add(("focus_point_z", hit.point.z));
            row.Add(("focus_distance", hit.distance));
            row.Add(("focus_object", hit.collider.gameObject.name));
        }

        else
        {
            row.Add(("focus_point_x", float.NaN));
            row.Add(("focus_point_y", float.NaN));
            row.Add(("focus_point_z", float.NaN));
            row.Add(("focus_distance", float.NaN));
            row.Add(("focus_object", "None"));
        }
    }

}
