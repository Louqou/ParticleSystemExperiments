using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class ParticleAttract : MonoBehaviour
{
    public Transform target;
    public float attractForce;

    private new ParticleSystem particleSystem;
    private Vector3 targetPos;
    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local
            || particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Custom) {
            targetPos = particleSystem.transform.InverseTransformPoint(target.position);
        }
        else {
            targetPos = target.transform.position;
        }
    }

    private void LateUpdate()
    {
        if (particles == null || particles.Length < particleSystem.main.maxParticles) {
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        }

        particleSystem.GetParticles(particles);

        float impulse = attractForce * Time.deltaTime;
        for (int i = 0; i < particles.Length; i++) {
            Vector3 direction = Vector3.Normalize(targetPos - particles[i].position);
            Vector3 changeInDirection = direction * impulse;
            particles[i].velocity += changeInDirection;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }
}
