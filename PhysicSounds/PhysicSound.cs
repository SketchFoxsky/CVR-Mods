using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sketch.PhysicSounds
{
    [AddComponentMenu("SketchFoxsky/ModdedCCK/PhysicSounds")]

    public class PhysicSound : MonoBehaviour
    {
        public AudioSource AudioSourceReference;
        public AudioClip[] MinCollisionAudio;
        public AudioClip[] MiddleCollisionAudio;
        public AudioClip[] MaxCollisionAudio;

        private void OnCollisionEnter(Collision collision)
        {
            var ColMag = collision.relativeVelocity.magnitude;

            if ((ColMag >= 0.1f) && (ColMag <= 4.9))
            {
                var clip = MinCollisionAudio[UnityEngine.Random.Range(0, MinCollisionAudio.Length)];
                Debug.Log("Small");
                AudioSourceReference.clip = clip;
                AudioSourceReference.Play();
            }
            if ((ColMag >= 5f) && (ColMag <= 10))
            {
                var clip = MiddleCollisionAudio[UnityEngine.Random.Range(0, MinCollisionAudio.Length)];
                Debug.Log("Mid");
                AudioSourceReference.clip = clip;
                AudioSourceReference.Play();
            }
            if (ColMag >= 10.01f)
            {
                var clip = MaxCollisionAudio[UnityEngine.Random.Range(0, MinCollisionAudio.Length)];
                Debug.Log("Large");
                AudioSourceReference.clip = clip;
                AudioSourceReference.Play();
            }
        }
    }
}

