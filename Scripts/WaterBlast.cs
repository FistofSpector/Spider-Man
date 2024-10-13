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
    public class WaterBlast : Ability
    {
        private GameObject water;
        private float damage = 0.1f;
        private bool isActive = false;

        public override void Start()
        {
            base.Name = "Water Stream";
            base.BodyPart = "Torso";
            base.Start();

            var spawnable = ModAPI.FindSpawnable("Liquid Outlet");
            var liquid = spawnable.Prefab.transform.Find("BleedingParticle").gameObject;

            // Using the class-level water variable
            water = Instantiate(liquid.transform.Find("Trail").gameObject, Limb.transform);
            water.transform.localRotation = Limb.transform.rotation * Quaternion.Euler(0f, 0f, -180f);

            // Trail Particle Effect
            var waterParticleSystem = water.GetComponent<ParticleSystem>();
            waterParticleSystem.emissionRate = 1f;
            waterParticleSystem.playbackSpeed = 5f;

            // Access and modify the main module of the particle system
            var main = waterParticleSystem.main;
            main.startColor = new Color(1f, 1f, 1f);
            main.startSize = 5f;
            main.startSpeed = 5f;
            main.startDelay = 0f;
            main.startLifetime = 10f;
            main.gravityModifier = 0f;

            var shape = waterParticleSystem.shape;
            shape.enabled = false;

            var force = waterParticleSystem.forceOverLifetime;
            force.enabled = true;
            force.y = 1f;

            // Modify the noise module
            var noise = waterParticleSystem.noise;
            noise.enabled = true;
            noise.frequency = 0.25f;

            // Disable the velocity limit module
            var velocityLimit = waterParticleSystem.limitVelocityOverLifetime;
            velocityLimit.enabled = false;
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<WaterBlast>();
            ability.Limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();
            isActive = !isActive;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            water.GetComponent<ParticleSystem>().Stop();
        }

        public void Update()
        {
            if (Enabled)
            {
                if (gameObject.GetComponent<PhysicalBehaviour>().IsBeingUsedContinuously())
                {
                    water.GetComponent<ParticleSystem>().Play();

                    RaycastHit2D hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
                    if (hit.collider != null)
                    {
                        LimbBehaviour limbHit = hit.collider.GetComponent<LimbBehaviour>();
                        if (limbHit != null)
                        {
                            limbHit.Health -= damage;
                        }
                    }
                }
                else
                {
                    water.GetComponent<ParticleSystem>().Stop();
                }
            }
        }
    }
}