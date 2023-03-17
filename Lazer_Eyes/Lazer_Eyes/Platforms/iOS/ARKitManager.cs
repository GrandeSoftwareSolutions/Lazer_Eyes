using System;
using ARKit;
namespace Lazer_Eyes.Platforms.iOS
{
    /// <summary>
    /// Sets the foundation for obstacle processing, configures ARSession 
    /// and defines callback for obstacle processing
    /// </summary>
    public class ARKitManager
    {
        ARSession session;
        public ARKitManager()
        {

            // Create a session configuration
            var configuration = new ARWorldTrackingConfiguration
            {
                PlaneDetection = ARPlaneDetection.Vertical | ARPlaneDetection.Horizontal,
                LightEstimationEnabled = true
            };

            //ARSceneReconstruction arscnReconstructionSetting = ARSceneReconstruction.None;
            //// Create a session configuration
            //if (ARWorldTrackingConfiguration.SupportsSceneReconstruction(ARSceneReconstruction.MeshWithClassification))
            //{
            //    arscnReconstructionSetting = ARSceneReconstruction.MeshWithClassification;
            //}
            //var configuration = new ARWorldTrackingConfiguration
            //{
            //    PlaneDetection = ARPlaneDetection.Vertical | ARPlaneDetection.Horizontal,
            //    LightEstimationEnabled = true,
            //    SceneReconstruction = arscnReconstructionSetting
            //};

            //configuration.MaximumNumberOfTrackedImages = 1;
            session = new ARSession
            {
                Delegate = new SessionDelegate()
            };
            session.Run(configuration, ARSessionRunOptions.ResetTracking);

        }
    }
}

