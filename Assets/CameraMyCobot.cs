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

    Scalar red = new Scalar(255, 0, 0);

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2HSV);
        Scalar[] limits = getLimits(red);
        Cv2.InRange(processImage, limits[0], limits[1], processImage);

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
            Cv2.Rectangle(processImage, biggestRect, red, 2);
            Cv2.PutText(processImage, "Cube", new Point(biggestRect.X, biggestRect.Y - 10), HersheyFonts.HersheySimplex, 0.5, red, 2);
            Cv2.PutText(image, "Cube", new Point(biggestRect.X, biggestRect.Y - 10), HersheyFonts.HersheySimplex, 0.5, red, 2);
            Point center = getCenterFromImage(biggestRect);
            Debug.Log("Cube detected at: " + center);
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

    public Scalar[] getLimits(Scalar color)
    {
        // Transform color to HSV
        Mat hsvColor = new Mat();
        Cv2.CvtColor(new Mat(1, 1, MatType.CV_8UC3, color), hsvColor, ColorConversionCodes.BGR2HSV);
        Vec3b hsv = hsvColor.Get<Vec3b>(0, 0);

        // Define range of color in HSV
        Scalar lower = new Scalar(hsv[2] - Threshold, 70, 170);
        Scalar upper = new Scalar(hsv[2] + Threshold, 255, 255);

        return new Scalar[] { lower, upper };
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
