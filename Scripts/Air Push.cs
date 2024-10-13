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
    public class AirPush : Ability
    {
        private AudioSource audioSource;
        private GameObject smoke;
        private float additionalForce = 200f;

        public override void Start()
        {
            base.Name = "Air Push";
            base.BodyPart = "Hands";
            base.Start();

            var prefab = ModAPI.FindSpawnable("Particle Projector").Prefab;
            var smokePrefab = prefab.transform.GetChild(0);

            smoke = Instantiate(smokePrefab.gameObject, Limb.transform);
            smoke.transform.localPosition = Vector3.zero;
            smoke.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.down);

            var particleSystem = smoke.GetComponent<ParticleSystem>();
            particleSystem.loop = true;
            particleSystem.playbackSpeed = 10f;
            particleSystem.gravityModifier = 0f;
            particleSystem.emissionRate = 25f;
            particleSystem.startSpeed = 1f;
            particleSystem.startSize = 1f;
            particleSystem.startLifetime = 5f;
            particleSystem.startRotation = 0f;

            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.duration = 3f;

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = ResourceStorage.AirPushClip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<AirPush>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            ModAPI.CreateParticleEffect("Vapor", Limb.transform.position);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public void Update()
        {
            if (Enabled)
            {
                var phys = Limb.GetComponent<PhysicalBehaviour>();
                if (phys.StartedBeingUsedContinuously())
                {
                    smoke.GetComponent<ParticleSystem>().Play();
                    audioSource.Play();
                }

                if (phys.IsBeingUsedContinuously())
                {
                    var hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
                    if (hit.collider != null)
                    {
                        var limbHit = hit.collider.GetComponent<LimbBehaviour>();
                        if (limbHit == null || limbHit.Person != Limb.Person)
                        {
                            var hoverbox = hit.collider.GetComponent<HoverboxBehaviour>();
                            if (hoverbox != null)
                            {
                                Destroy(hoverbox);
                            }

                            var physHit = hit.collider.GetComponent<PhysicalBehaviour>();
                            if (physHit != null)
                            {
                                if (physHit.OnFire)
                                {
                                    physHit.Extinguish();
                                }
                            }

                            var rbHit = hit.collider.GetComponent<Rigidbody2D>();
                            var direction = (hit.point - (Vector2)Limb.transform.position).normalized;
                            var force = direction * rbHit.mass * additionalForce;
                            rbHit.AddForce(force, ForceMode2D.Force);
                        }
                    }
                }

                if (phys.StoppedBeingUsedContinuously())
                {
                    smoke.GetComponent<ParticleSystem>().Stop();
                    audioSource.Stop();
                }
            }
        }
    }
}