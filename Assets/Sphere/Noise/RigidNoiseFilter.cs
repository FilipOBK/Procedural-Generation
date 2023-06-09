using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings.RigidNoiseSettings settings;

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseVal = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1.0f;
        float weight = 1.0f;

        for (int i = 0; i < settings.numLayers; i++)
        {

            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            noiseVal += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }

        noiseVal = Mathf.Max(0, noiseVal - settings.minValue);
        return noiseVal * settings.strength;
    }
}
