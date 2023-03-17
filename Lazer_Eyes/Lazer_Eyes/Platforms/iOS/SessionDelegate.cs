using System;
using ARKit;

namespace Lazer_Eyes.Platforms.iOS
{
    /// <summary>
    /// Defines callback for ARSession to process obstacles
    /// </summary>
    public class SessionDelegate : ARSessionDelegate
    {
        /// <summary>
        /// Callback executed when ARSession updates current frame.
        /// Main functionality of the application,
        /// which is to report information of the user's surroundings.
        /// </summary>
        /// <param name="session">ARSession object</param>
        /// <param name="frame">Current ARFrame</param>
        public override void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            LightDetector.CheckLightQuality((float)frame.LightEstimate.AmbientIntensity);
            ObjectDetector.ProcessFrame(session, frame);
            List<ARAnchor> anchors = AnchorProcessor.CreateAnchorsFromObjects(session, frame, ObjectDetector.DetectedObjects);
            anchors.AddRange(frame.Anchors);
            AnchorProcessor.ProcessAnchors(session, anchors);
            frame.Dispose();
        }
    }
}

