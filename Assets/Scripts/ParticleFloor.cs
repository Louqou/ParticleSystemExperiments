using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFloor : MonoBehaviour
{
    public float pushSpeed = 10f;
    public int xLength = 50;
    public int yLength = 50;
    private new ParticleSystem particleSystem;
    private List<ParticleSystem.Particle> triggerParticles;

    private void Start()
    {
        triggerParticles = new List<ParticleSystem.Particle>();
        particleSystem = GetComponent<ParticleSystem>();

        int numParticles = xLength * yLength;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.maxParticles = numParticles;
        particleSystem.Emit(numParticles);
        particleSystem.GetParticles(particles);

        // Lets assume particle x y dimension is 1x1
        for (int i = 0; i < xLength; i++) {
            for (int j = 0; j < yLength; j++) {
                particles[(i * yLength) + j].position = new Vector3(i, j);
                particles[(i * yLength) + j].randomValue = 0f;
            }
        }

        particleSystem.SetParticles(particles, numParticles);
    }

    private void OnParticleTrigger()
    {
        ParticlesInsideTrigger();
        ParticlesOutsideTrigger();
    }

    private void ParticlesInsideTrigger()
    {
        int numParticles = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, triggerParticles);
        float pushAmount = -pushSpeed * Time.deltaTime;
        for (int i = 0; i < numParticles; i++) {
            ParticleSystem.Particle particle = triggerParticles[i];
            particle.position = new Vector3(particle.position.x, particle.position.y, particle.position.z + pushAmount);
            particle.randomValue += pushAmount;
            triggerParticles[i] = particle;
        }

        particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, triggerParticles);
    }

    private void ParticlesOutsideTrigger()
    {
        int numParticles = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, triggerParticles);
        float pushAmount = pushSpeed * Time.deltaTime;
        for (int i = 0; i < numParticles; i++) {
            ParticleSystem.Particle particle = triggerParticles[i];
            float movedAmount = particle.randomValue;
            if (movedAmount != 0) {
                float dist = Mathf.Min(pushAmount, -movedAmount);
                particle.position = new Vector3(
                    triggerParticles[i].position.x,
                    triggerParticles[i].position.y,
                    triggerParticles[i].position.z + dist);
                particle.randomValue += dist;
                triggerParticles[i] = particle;
            }
        }

        particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, triggerParticles);
    }
}
