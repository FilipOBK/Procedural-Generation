using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings shapeSettings)
    {
        this.settings = shapeSettings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for(int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        // Do first layer manually
        if(noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        // Do rest of layers
        for(int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].firstLayerMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);
    }
}
