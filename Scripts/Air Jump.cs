using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarTLA
{
    public class AirJump : Ability
    {
        private AudioSource audioSource;
        private GameObject smoke;

        public override void Start()
        {
            base.Name = "Air Jump";
            base.BodyPart = "Feet";
            base.Start();

            var prefab = ModAPI.FindSpawnable("Particle Projector").Prefab;
            var smokePrefab = prefab.transform.GetChild(0);

            smoke = Instantiate(smokePrefab.gameObject, Limb.transform);
            smoke.transform.rotation = Limb.transform.rotation * Quaternion.Euler(0, 0, 180f);

            ParticleSystem particleSystem = smoke.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.duration = 3f;
            particleSystem.loop = false;
            particleSystem.playbackSpeed = 15f;
            particleSystem.gravityModifier = 0f;
            particleSystem.emissionRate = 25f;
            particleSystem.startSpeed = 1f;
            particleSystem.startSize = 1f;
            particleSystem.startLifetime = 5f;
            particleSystem.startRotation = 0f;


            if (audioSource == null)
            {
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = ResourceStorage.AirJumpClip;
            }
        }
        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<AirJump>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            smoke.GetComponent<ParticleSystem>().Play();
            audioSource.Play();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}