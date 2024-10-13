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
        private GameObject smoke;

        public override void Start()
        {
            base.Start();

            GameObject prefab = ModAPI.FindSpawnable("Particle Projector").Prefab;
            Transform smokePrefab = prefab.transform.GetChild(0);

            smoke = Instantiate(smokePrefab.gameObject, limb.transform);
            smoke.transform.rotation = limb.transform.rotation * Quaternion.Euler(0, 0, 180f);

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
        }
        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<AirJump>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            
        }
    }
}