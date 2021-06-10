using UnityEngine;
using System.Collections;

public class StoryObject : MonoBehaviour {

  public void SetVisible(int visible)
  {
    Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
    for (int i = 0; i < renderers.Length; ++i) {
      if (visible == 0) {
        renderers[i].enabled = false;
      } else {
        renderers[i].enabled = true;
      }
    }
  }
  public void PlayAnimation(object[] args)
  {
    if (args.Length < 2) return;
    string animName = (string)args[0];
    float speed = (float)args[1];
    if (null != gameObject.GetComponent<Animation>()) {
      if (null != gameObject.GetComponent<Animation>()[animName]) {
        gameObject.GetComponent<Animation>()[animName].speed = speed;
        gameObject.GetComponent<Animation>().Play(animName);
      }else {
        Debug.LogError(string.Format("StoryObject PlayAnimation: object {0} can't find anim {1}", gameObject.name, animName));
      }
    }
  }
  public void PlayParticle()
  {
    ParticleSystem[] pss = gameObject.GetComponentsInChildren<ParticleSystem>();
    foreach (ParticleSystem ps in pss) {
      ps.Play();
    }
  }
  public void StopParticle()
  {
    ParticleSystem[] pss = gameObject.GetComponentsInChildren<ParticleSystem>();
    foreach (ParticleSystem ps in pss) {
      ps.Stop();
    }
  }

  public void PlaySound(int index)
  {
    AudioSource[] audios = gameObject.GetComponents<AudioSource>();
    if (null != audios && index >= 0 && index < audios.Length) {
      audios[index].Play();
    }
  }

  public void StopSound(int index)
  {
    AudioSource[] audios = gameObject.GetComponents<AudioSource>();
    if (null != audios && index >= 0 && index < audios.Length) {
      audios[index].Stop();
    }
  }
}
