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
    public class WaterJet : Ability
    {
        private GameObject water;
        private AudioSource audioSource;
        private float duration = 0.1f;
        private bool cooldown = false;
        private float damageRate = 0.1f;
        private float additionalForce = 5f;

        public override void Start()
        {
            base.Name = "Water Jet";
            base.BodyPart = "Torso";
            base.Start();

            var spawnable = ModAPI.FindSpawnable("Liquid Outlet");
            var liquid = spawnable.Prefab.transform.Find("BleedingParticle").transform.Find("Trail").gameObject;

            water = Instantiate(liquid, Limb.transform);
            water.transform.localPosition = Vector3.zero;
            water.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.down);

            var waterParticleSystem = water.GetComponent<ParticleSystem>();
            waterParticleSystem.loop = true;
            waterParticleSystem.emissionRate = 50f;
            waterParticleSystem.playbackSpeed = 3f;

            var main = waterParticleSystem.main;
            main.startColor = new Color(1f, 1f, 1f);
            main.startSize = 5f;
            main.startDelay = 0f;
            main.startLifetime = 5f;
            main.gravityModifier = 0f;

            var shape = waterParticleSystem.shape;
            shape.enabled = true;

            var collision = waterParticleSystem.collision;
            collision.enabled = true;
            collision.lifetimeLossMultiplier = 0f;
            collision.quality = UnityEngine.ParticleSystemCollisionQuality.Low;

            var force = waterParticleSystem.forceOverLifetime;
            force.enabled = true;
            force.y = 5f;

            var noise = waterParticleSystem.noise;
            noise.enabled = false;
            noise.frequency = 0.25f;

            var velocityLimit = waterParticleSystem.limitVelocityOverLifetime;
            velocityLimit.enabled = false;

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = ResourceStorage.WaterJetClip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<WaterJet>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();
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
                    water.GetComponent<ParticleSystem>().Play();
                    audioSource.Play();
                }

                if (phys.IsBeingUsedContinuously())
                {
                    RaycastHit2D hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
                    if (hit.collider != null)
                    {
                        var limbHit = hit.collider.GetComponent<LimbBehaviour>();
                        if (limbHit != null && limbHit.Person != Limb.Person)
                        {
                            limbHit.Health -= damageRate;
                            if (!cooldown)
                            {
                                StartCoroutine(DamageCooldown(hit.point, limbHit));
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
                    water.GetComponent<ParticleSystem>().Stop();
                    audioSource.Stop();
                }
            }
        }

        public IEnumerator DamageCooldown(Vector2 hitPoint, LimbBehaviour limbHit)
        {
            cooldown = true;
            limbHit.SkinMaterialHandler.AddDamagePoint(DamageType.Blunt, hitPoint, UnityEngine.Random.Range(1f, 10f));
            yield return new WaitForSeconds(duration);
            cooldown = false;
        }
    }
}