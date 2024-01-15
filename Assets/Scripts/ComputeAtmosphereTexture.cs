using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ComputeAtmosphereTexture : MonoBehaviour
{
    const float maxFloat = 3.402823466e+38f;
    int textureSize = 2048;
    int numOutScatteringSteps = 10;
    float atmosphereRadius = 10;
    //float avgDensityHeight01;
    float densityFalloff = 4;
    Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(textureSize, textureSize);
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                const float planetRadius = 1;

                Vector2 uv = new Vector2(i / (float)textureSize, i / (float)textureSize);
                float height01 = uv.y;
                float angle = uv.x * Mathf.PI;
                //angle = (1-cos(angle))/2;
                //Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                float y = -2 * uv.x + 1;
                float x = Mathf.Sin(Mathf.Acos(y));
                Vector2 dir = new Vector2(x, y);

                Vector2 inPoint = new Vector2(0, Mathf.Lerp(planetRadius, atmosphereRadius, height01));
                float dstThroughAtmosphere = raySphere(new Vector3(), atmosphereRadius, new Vector3(inPoint.x, inPoint.y, 0), new Vector3(dir.x, dir.y, 0)).y;
                //Vector2 outPoint = inPoint + dir * raySphere(new Vector3(), atmosphereRadius, new Vector3(inPoint.x, inPoint.y, 0), new Vector3(dir.x, dir.y, 0)).y;
                //float outScattering = calculateOutScattering(inPoint, outPoint);
                float outScattering = opticalDepth(inPoint + dir * 0.0001f, dir, dstThroughAtmosphere - 0.0002f);

                tex.SetPixel(i, j, new Color(outScattering, outScattering, outScattering));
            }
        }
        File.WriteAllBytes(Application.dataPath + "/Scripts/atmosphereTexture.jpg", tex.EncodeToJPG());
    }
    float densityAtPoint(Vector2 densitySamplePoint)
    {
        float planetRadius = 1;
        Vector2 planetCentre = new Vector2(0, 0);

        float heightAboveSurface = (densitySamplePoint - planetCentre).magnitude - planetRadius;
        float height01 = heightAboveSurface / (atmosphereRadius - planetRadius);
        float localDensity = Mathf.Exp(-height01 * densityFalloff) * (1 - height01);
        return localDensity;
    }

    float opticalDepth(Vector2 rayOrigin, Vector2 rayDir, float rayLength)
    {
        int numOpticalDepthPoints = numOutScatteringSteps;

        Vector2 densitySamplePoint = rayOrigin;
        float stepSize = rayLength / (numOpticalDepthPoints - 1);
        float opticalDepth = 0;

        for (int i = 0; i < numOpticalDepthPoints; i++)
        {
            float localDensity = densityAtPoint(densitySamplePoint);
            opticalDepth += localDensity * stepSize;
            densitySamplePoint += rayDir * stepSize;
        }
        return opticalDepth;
    }
    Vector2 raySphere(Vector3 sphereCentre, float sphereRadius, Vector3 rayOrigin, Vector3 rayDir) {
		Vector3 offset = rayOrigin - sphereCentre;
		float a = 1; // Set to dot(rayDir, rayDir) if rayDir might not be normalized
		float b = 2 * Vector3.Dot(offset, rayDir);
		float c = Vector3.Dot(offset, offset) - sphereRadius * sphereRadius;
		float d = b * b - 4 * a * c; // Discriminant from quadratic formula

		// Number of intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0
		if (d > 0) {
			float s = Mathf.Sqrt(d);
			float dstToSphereNear =  Mathf.Max(0, (-b - s) / (2 * a));
			float dstToSphereFar = (-b + s) / (2 * a);

			// Ignore intersections that occur behind the ray
			if (dstToSphereFar >= 0) {
				return new Vector2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
			}
		}
		// Ray did not intersect sphere
		return new Vector2(maxFloat, 0);
	}
    // Update is called once per frame
    void Update()
    {

    }
}
