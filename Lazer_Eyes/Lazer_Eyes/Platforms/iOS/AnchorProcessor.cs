using System;
using ARKit;
using CoreGraphics;
using Microsoft.Maui.Controls;

namespace Lazer_Eyes.Platforms.iOS
{
    /// <summary>
    /// Processes anchors in the ARSession and anchors created from detected objects.
    /// Used to find anchor with highest priority to report as obstacle to the user
    /// </summary>
    public static class AnchorProcessor
    {
        /// <summary>
        /// Singleton object holding app settings
        /// </summary>
        static readonly Settings settings = Settings.Get();

        /// <summary>
        /// Finds distance to every anchor and chooses the closest one as obstacle
        /// Avoids using anchors that represent the floor because that is not helpful to the user
        /// in navigating their environment.
        /// Also ignores anchors without name to only report identified objects
        /// </summary>
        /// <param name="session">Current ARSession</param>
        /// <param name="anchors">List of anchors; conists of detected objects and planes such as walls, windows, etc.</param>
        public static void ProcessAnchors(ARSession session, List<ARAnchor> anchors)
        {

            ARAnchor obstacleAnchor = null;
            double? obstacleAnchorDistance = null;
            string? obstacleAnchorObjectType = null;

            foreach (ARAnchor anchor in anchors)
            {
                // System.Diagnostics.Debug.WriteLine($"anchor: {anchor}");

                string anchorObjectType;
                if (anchor is ARPlaneAnchor)
                {
                    anchorObjectType = ((ARPlaneAnchor)anchor).Classification.ToString();
                    if (anchorObjectType.ToLower() == "floor" || anchorObjectType.ToLower() == "none")
                    {
                        continue;
                    }
                }
                else
                {
                    //TODO Can add custom logic here to select anchors of objects we care about
                    anchorObjectType = anchor.Name;
                }

                double anchorDistance = GetDistanceToAnchor(session, anchor);
                if (anchorDistance < settings.GetDistanceThreshold())
                {
                    if (obstacleAnchor == null || anchorDistance < obstacleAnchorDistance)
                    {
                        obstacleAnchor = anchor;
                        obstacleAnchorDistance = anchorDistance;
                        obstacleAnchorObjectType = anchorObjectType;
                    }
                }
            }

            LidarUtils.CurrentObstacle.Distance = obstacleAnchorDistance;
            LidarUtils.CurrentObstacle.ObstacleName = obstacleAnchorObjectType;
            //System.Diagnostics.Debug.WriteLine($"Obstacle Name: {LidarUtils.CurrentObstacle.ObstacleName}, Obstacle Distance: {LidarUtils.CurrentObstacle.Distance}");
        }

        /// <summary>
        /// Gets distance in meters from phone camera to specified anchor.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public static double GetDistanceToAnchor(ARSession session, ARAnchor anchor)
        {
            var cameraPosition = session.CurrentFrame.Camera.Transform.Column3;
            var anchorPosition = anchor.Transform.Column3;
            // here’s a line connecting the two points, which might be useful for other things
            var cameraToAnchor = cameraPosition - anchorPosition;
            // and here’s just the scalar distance
            var distance = cameraToAnchor.Length();
            return distance;
        }

        /// <summary>
        /// Creates an anchor from a detected object and their estimated position.
        /// Uses the object bounding box in 2D and creates a raycast query to get the objects coordinates in 3D space.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="frame"></param>
        /// <param name="DetectedObjects">Tuple consisting of object name first and then the bounding box of its position in the image.</param>
        /// <returns>List of anchors created from identified objects.</returns>
        public static List<ARAnchor> CreateAnchorsFromObjects(ARSession session, ARFrame frame, List<(string, CGRect)> DetectedObjects)
        {
            List<ARAnchor> anchors = new();

            foreach((string name, CGRect bbox) in DetectedObjects)
            {
                CGPoint centroid = new(bbox.GetMidX(), bbox.GetMidY());
                ARRaycastQuery query = frame.CreateRaycastQuery(point: centroid, target: ARRaycastTarget.EstimatedPlane, alignment: ARRaycastTargetAlignment.Any);
                ARRaycastResult[] queryResults = session.Raycast(query);
                //System.Diagnostics.Debug.WriteLine(queryResults.Length);
                if (queryResults.Length > 0)
                {
                    var result = queryResults[0];
                    if (result.Anchor == null){
                        ARAnchor anchor = new(name, result.WorldTransform);
                        //System.Diagnostics.Debug.WriteLine($"added anchor: {anchor}");

                        anchors.Add(anchor);
                    }
                }
            }
            return anchors;
        }
    }
}

