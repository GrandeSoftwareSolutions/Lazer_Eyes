using ARKit;
using CoreVideo;
using UIKit;
using Vision;
using ImageIO;
using Foundation;
using CoreML;
using CoreGraphics;
using Microsoft.Maui.Controls;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Lazer_Eyes.Platforms.iOS
{
    /// <summary>
    /// Detects objects from an image frame
    /// </summary>
    public static class ObjectDetector
    {
        /// <summary>
        /// Image information
        /// </summary>
        public static CVPixelBuffer? CurrentBuffer;
        /// <summary>
        /// Size of image
        /// </summary>
        public static CGSize ImageSize;

        /// <summary>
        /// Number of largest objects to detect, value is limited to 
        /// avoid reporting unneccesary smaller objects thhat do not greatly impact the user's navigation
        /// </summary>
        private const int LARGEST_OBSERVATIONS_LIMIT = 5;

        /// <summary>
        /// Contains a tuple of the most import objects (name, bbox).
        /// </summary>
        public static List<(string name, CGRect bbox)> DetectedObjects = new();

        /// <summary>
        /// Model used for object detection.
        /// </summary>
        private static MLModel? model;

        /// <summary>
        /// Request for object detection to Vision Framework to be initialized with settings
        /// such as camera orientation, model, callback method.
        /// </summary>
        private static VNCoreMLRequest classificationRequest;

        /// <summary>
        /// Constructor for object detector, loads model and creates request to Vision framework.
        /// </summary>
        static ObjectDetector()
        {
            LoadMLModel();
            CreateRequest();
        }

        /// <summary>
        /// Create image processing request to vision framework for object detection.
        /// </summary>
        private static void CreateRequest()
        { 

            classificationRequest = ((Func<VNCoreMLRequest>)(() =>
            {
                try
                {
                    VNCoreMLModel vnmodel = VNCoreMLModel.FromMLModel(model, out NSError mlErr);
                    VNCoreMLRequest request = new VNCoreMLRequest(model: vnmodel,
                        completionHandler: (request, error) =>
                        {
                            ProcessClassifications(request, error);
                        });

                    // Crop input images to square area at center, matching the way the ML model was trained.
                    request.ImageCropAndScaleOption = VNImageCropAndScaleOption.ScaleFill;

                    // Use CPU for Vision processing to ensure that there are adequate GPU resources for rendering.
                    //request.UsesCpuOnly = true; //TODO is necessary?
                    return request;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load Vision ML model: {ex.Message}");
                    throw;
                }

            }))();
        }

        /// <summary>
        /// Loads ML Model for use in object detection.
        /// Requires a model that returns a bounding box.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void LoadMLModel()
        {
            System.Diagnostics.Debug.WriteLine($"MODEL LOADING");

            
            var modelUrl = NSBundle.MainBundle.GetUrlForResource("MobileNetV2_SSDLite", "mlmodel");
            System.Diagnostics.Debug.WriteLine($"Bundle {string.Join(separator: ' ', NSBundle._AllBundles.ToList())}");

            var compiledUrl = MLModel.CompileModel(modelUrl, out NSError err);
            if (err != null)
            {
                throw new Exception(err.ToString());
            }
            model = MLModel.Create(compiledUrl, out NSError mlErr);
            System.Diagnostics.Debug.WriteLine($"MODEL LOADED: {model}");
        }

        /// <summary>
        /// Detects objects in input frame.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="frame"></param>
        public static void ProcessFrame(ARSession session, ARFrame frame)
        {
            // Do not enqueue other buffers for processing while another Vision task is still running.
            // The camera stream has only a finite amount of buffers available; holding too many buffers for analysis would starve the camera.
            if (CurrentBuffer == null)
            {
                // Retain the image buffer for Vision processing.
                CurrentBuffer = frame.CapturedImage;
                ImageSize = frame.Camera.ImageResolution;
                ClassifyCurrentImage();

            }
            return;
        }

        /// <summary>
        /// Detects objects in current image from frame.
        /// Designed for perfomance as it processes only one frame at a time,
        /// when finished it clears current buffer to allow processing for other frames.
        /// </summary>
        private static async void ClassifyCurrentImage()
        {
            // Most computer vision tasks are not rotation agnostic so it is important to pass in the orientation of the image with respect to device.
            CGImagePropertyOrientation orientation = (CGImagePropertyOrientation)UIDevice.CurrentDevice.Orientation;

            VNImageRequestHandler requestHandler = new VNImageRequestHandler(CurrentBuffer, orientation, imageOptions: new VNImageOptions());

            await Task.Run(() =>
            {
                try
                {
                    requestHandler.Perform(new[] { classificationRequest }, out NSError Err);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"\"Error: Vision request failed with error : {ex.Message}");
                }
                finally
                {
                    CurrentBuffer = null;
                }
            });
        }

        /// <summary>
        /// Begins processing object classifications from Vision image processing requests.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="error"></param>
        private static void ProcessClassifications(VNRequest request, NSError error)
        {
            var observations = request.GetResults<VNRecognizedObjectObservation>().OrderByDescending(x => x.Confidence).ToList();
            // temp debug code
            //foreach (var o in observations)
            //{
            //    System.Diagnostics.Debug.WriteLine($"{o.Labels[0].Identifier} == {o.Labels[0].Confidence} == {o.BoundingBox}");
            //}

            GetLargestBboxObservations(observations);
            
        }

        /// <summary>
        /// Resizes detected object bounding box to retrieve coordinates for use in a racast query to get depth information 
        /// </summary>
        /// <param name="observation">Detected object</param>
        private static CGRect ResizeBbox(VNRecognizedObjectObservation observation)
        {
            CGRect currentBbox = observation.BoundingBox;
            currentBbox.Y = 1 - currentBbox.Y;

            /*            
            NFloat width = ImageSize.Width;
            NFloat height = ImageSize.Height;
            CGAffineTransform scale = CGAffineTransform.MakeIdentity();
            scale.Scale(sx: width, sy: height);
            return CGAffineTransform.CGRectApplyAffineTransform(currentBbox, scale);
            */

            return currentBbox;
        }

        /// <summary>
        /// Gets objects with largest bounding box,
        /// we prioritize larger objects and we naively assume they are of most interest to the user.
        /// </summary>
        /// <param name="observations"></param>
        private static void GetLargestBboxObservations(List<VNRecognizedObjectObservation> observations)
        {
            observations.Sort(CompareObservationBboxes);

            List<(string name, CGRect bbox)> largestObservations = new();
            int i = 0;
            while(i < observations.Count && i < LARGEST_OBSERVATIONS_LIMIT)
            {
                var bbox = ResizeBbox(observations[i]);

                largestObservations.Add((observations[i].Labels[0].Identifier, bbox));
                i++;
            }

            DetectedObjects = largestObservations;

        }

        /// <summary>
        /// Compares areas of two bounding boxes and returns larger one.
        /// </summary>
        /// <param name="obs1"></param>
        /// <param name="obs2"></param>
        /// <returns></returns>
        private static int CompareObservationBboxes(VNRecognizedObjectObservation obs1, VNRecognizedObjectObservation obs2)
        {
            var area1 = obs1.BoundingBox.Width * obs1.BoundingBox.Height;
            var area2 = obs2.BoundingBox.Width * obs2.BoundingBox.Height;

            if(area1 == area2) { return 0; }
            else if (area1 < area2) { return -1; }
            else { return 1; }
        }
    }
}
