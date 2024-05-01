using System.Collections;
using UnityEngine;
using Photon.Pun;

public class CubeManager : MonoBehaviourPun
{
    private ParticleSystem myParticleSystem;

    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        if (myParticleSystem == null)
        {
            Debug.LogError("No ParticleSystem found on the GameObject.");
        }
    }

    public void StartParticleSystem()
    {
        if (myParticleSystem.isPlaying)
        {
            myParticleSystem.Stop();
        }
        photonView.RPC("ActivateParticleSystemRPC", RpcTarget.All);
    }

    [PunRPC]
    private void ActivateParticleSystemRPC()
    {
        StartCoroutine(ActivateParticleSystem());
    }

    private IEnumerator ActivateParticleSystem()
    {
        myParticleSystem.Play();
        yield return new WaitForSeconds(5);
        myParticleSystem.Stop();
    }
}
