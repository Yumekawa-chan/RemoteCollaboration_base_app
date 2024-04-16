using UnityEngine;
using System.Collections;

public class ShowParticlesOnTouch : MonoBehaviour
{
    public ParticleSystem touchParticles;

    private void Start()
    {
        touchParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("rightHand"))
        {
            touchParticles.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("rightHand"))
        {
            StartCoroutine(StopParticlesAfterDelay(5f));
        }
    }

    private IEnumerator StopParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        touchParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
