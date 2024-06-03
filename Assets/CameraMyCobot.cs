using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class CameraMyCobot : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [Range(0.0f, 255.0f)] public float Threshold;
    [SerializeField] private bool showProcessedImage = true;

    private Mat image;
    private Mat processImage = new Mat();

    static public Point movement;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2HSV);
        Mat redColorFilter = getLimits(image);

        Cv2.BitwiseAnd(processImage, processImage, processImage, redColorFilter);
        // Convert that to mat object
        Cv2.CvtColor(processImage, processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.Binary);

        Point[][] contours = new Point[1][];
        Cv2.FindContours(processImage, out contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        List<OpenCvSharp.Rect> rects = new List<OpenCvSharp.Rect>();
        foreach (Point[] contour in contours)
        {
            OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
            rects.Add(rect);
        }

        OpenCvSharp.Rect biggestRect = getBiggestRect(rects);

        if (checkForCube(biggestRect))
        {
            Cv2.Rectangle(processImage, biggestRect, new Scalar(255, 0, 0), 2);
            Cv2.PutText(processImage, "Red object", new Point(biggestRect.X, biggestRect.Y - 10), HersheyFonts.HersheySimplex, 0.5, new Scalar(0, 0, 255), 2);
            Cv2.PutText(image, "Red object", new Point(biggestRect.X, biggestRect.Y - 10), HersheyFonts.HersheySimplex, 0.5, new Scalar(0, 0, 255), 2);
            Point center = getCenterFromImage(biggestRect);
            CameraMyCobot.movement = center;
            //Debug.Log("Cube detected at: " + center);
        } 
        else
        {
            CameraMyCobot.movement = new Point(0, 0);
        }

        if (output == null)
        {
            output = OpenCvSharp.Unity.MatToTexture(showProcessedImage ? processImage : image);
        }
        else
        {
            OpenCvSharp.Unity.MatToTexture(showProcessedImage ? processImage : image, output);
        }

        return true;
    }

    public Mat getLimits(Mat img_original)
    {
        Mat mask0 = new Mat();
        Mat mask1 = new Mat();

        Mat img = img_original.Clone();

        Scalar lowerred = new Scalar(0, 50, 50);
        Scalar upperred = new Scalar(10, 255, 255);
        Cv2.InRange(img, lowerred, upperred, mask0);

        lowerred = new Scalar(170, 50, 50);
        upperred = new Scalar(180, 255, 255);
        Cv2.InRange(img, lowerred, upperred, mask1);

        // join masks
        Cv2.Add(mask0, mask1, mask0); // mask0 + mask1 = mask0

        return mask0;
    }

    public OpenCvSharp.Rect getBiggestRect(List<OpenCvSharp.Rect> rects)
    {
        OpenCvSharp.Rect biggestRect = new OpenCvSharp.Rect();
        int biggestArea = 0;
        foreach (OpenCvSharp.Rect rect in rects)
        {
            int area = rect.Width * rect.Height;
            if (area > biggestArea)
            {
                biggestArea = area;
                biggestRect = rect;
            }
        }
        return biggestRect;
    }

    public bool checkForCube(OpenCvSharp.Rect biggestRect)
    {
        // Check if it's a physical cube inside of the camera and not something else
        return biggestRect.Width > 50 && biggestRect.Height > 50;
    }

    public Point getCenterFromImage(OpenCvSharp.Rect rect)
    {
        int x = rect.X + rect.Width / 2;
        int y = rect.Y + rect.Height / 2;

        // Convert to be in the middle of the screen
        x -= processImage.Width / 2;
        y -= processImage.Height / 2;

        return new Point(x, y);
    }
}
