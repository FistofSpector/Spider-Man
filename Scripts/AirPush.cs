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
        private float throwForce = 100f;
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
            var ability = limb.gameObject.GetOrAddComponent<AirPush>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            RaycastHit2D hit = Physics2D.Raycast(limb.transform.position, -limb.transform.up);
            if (hit.collider != null)
            {
                Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    LimbBehaviour rbLimb = rb.GetComponent<LimbBehaviour>();
                    if (rbLimb == null || rbLimb.Person != limb.Person)
                    {
                        HoverboxBehaviour hoverbox = hit.collider.GetComponent<HoverboxBehaviour>();
                        if (hoverbox != null)
                        {
                            Destroy(hoverbox);
                        }

                        PhysicalBehaviour phys = hit.collider.GetComponent<PhysicalBehaviour>();
                        if (phys != null)
                        {
                            if (phys.OnFire)
                            {
                                phys.Extinguish();
                            }
                        }

                        ModAPI.CreateParticleEffect("Vapor", limb.transform.position);
                        smoke.GetComponent<ParticleSystem>().Play();

                        Vector2 direction = (hit.point - (Vector2)limb.transform.position).normalized;
                        float mass = rb.mass;
                        Vector2 force = direction * mass * throwForce;
                        rb.AddForce(force, ForceMode2D.Impulse);
                    }
                }
            }
        }

        public override void Deactivate()
        {
            
        }
    }
}