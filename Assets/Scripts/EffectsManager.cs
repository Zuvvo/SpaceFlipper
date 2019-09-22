using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public Gradient TrailDefaultColorGradient;
    public Gradient TrailHitColorGradient;


    public Light StrikerEffectPrefab;

    private static EffectsManager _instance;
    public static EffectsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EffectsManager>();
            }
            return _instance;
        }
    }

    public void InitStrikerEffect(Vector3 position)
    {
        Light light = Instantiate(StrikerEffectPrefab, position, Quaternion.identity);
        Destroy(light, 0.3f);
    }
}