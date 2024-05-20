using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class CameraMyCobot : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [SerializeField] private float Threshold = 96.4f;
    [SerializeField] private bool showProcessedImage = true;

    private Mat image;
    private Mat processImage = new Mat();

    Scalar red = new Scalar(0, 0, 255);

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2HSV);
        Scalar[] limits = getLimits(red);
        Cv2.InRange(processImage, limits[0], limits[1], processImage);

        if (output == null)
        {
            output = OpenCvSharp.Unity.MatToTexture(showProcessedImage ? processImage : image) ;
        }
        else
        {
            OpenCvSharp.Unity.MatToTexture(showProcessedImage ? processImage : image, output);
        }

        return true;
    }

    public static Scalar[] getLimits(Scalar color)
    {
        // Transform color to HSV
        Mat hsvColor = new Mat();
        Cv2.CvtColor(new Mat(1, 1, MatType.CV_8UC3, color), hsvColor, ColorConversionCodes.BGR2HSV);
        Vec3b hsv = hsvColor.Get<Vec3b>(0, 0);

        // Define range of color in HSV
        int tolerance = 30;
        Scalar lower = new Scalar(hsv[0] - tolerance, 100, 100);
        Scalar upper = new Scalar(hsv[0] + tolerance, 255, 255);

        return new Scalar[] { lower, upper };
    }
}
