using System.Collections;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private new ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("No ParticleSystem found on the GameObject.");
        }
    }

    public void StartParticleSystem()
    {
        if (particleSystem.isPlaying)
        {
            particleSystem.Stop();
        }
        StartCoroutine(ActivateParticleSystem());
    }

    private IEnumerator ActivateParticleSystem()
    {
        particleSystem.Play();
        yield return new WaitForSeconds(5);
        particleSystem.Stop();
    }
}
