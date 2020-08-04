using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRotation : MonoBehaviour
{
    Material mat;
    Vector2 Offset;
    public float Velocity;

    public ParticleSystem orbParticle;
    ParticleSystem.MainModule mainOrbParticle;

    private void Awake()
    {
        mainOrbParticle = orbParticle.main;
        mainOrbParticle.scalingMode = ParticleSystemScalingMode.Hierarchy;
    }

    void Start()
    {
        mat = GetComponent<Renderer>().materials[0];
        Offset = mat.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        Offset = new Vector2(Offset.x + Time.deltaTime * Velocity, Offset.y + Time.deltaTime * Velocity);
        mat.SetTextureOffset("_MainTex", Offset);
    }

    public void ChangeOrbColor(Color color)
    {
        mat.color = color;

        orbParticle.Stop();
        orbParticle.Clear();
        mainOrbParticle.startColor = color;
        orbParticle.Play();
    }
}
