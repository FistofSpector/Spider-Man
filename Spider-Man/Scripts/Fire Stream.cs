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
    public class FireStream : Ability
    {
        private Color flameColor;
        private PyroPowerToolEntity flameBehaviour;
        private GameObject flame;
        private AudioSource audioSource;

        public override void Start()
        {
            base.Name = "Fire Stream";
            base.BodyPart = "Hands";
            base.Start();

            var prefab = Instantiate(Resources.Load<GameObject>("Prefabs/PyroPowerEntity"));
            var flameBehaviour = prefab.GetComponent<PyroPowerToolEntity>();
            var flamePrefab = prefab.transform.GetChild(0);

            flame = Instantiate(flamePrefab.gameObject, Limb.transform);
            flame.transform.localPosition = Vector3.zero;
            flame.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.down);

            flame.transform.GetChild(0).gameObject.SetActive(false);
            flame.transform.GetChild(2).gameObject.SetActive(false);
            flame.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().startColor = flameColor;

            var colorOverLifetime = flame.GetComponent<ParticleSystem>().colorOverLifetime;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(flameColor);

            var flameParticleSystem = flame.GetComponent<ParticleSystem>();
            var main = flameParticleSystem.main;
            flameParticleSystem.emissionRate = 100f;
            flameParticleSystem.gravityModifier = 0f;
            flameParticleSystem.startSize = 1f;
            flameParticleSystem.startLifetime = 1.5f;

            main.startSize = new ParticleSystem.MinMaxCurve(1f);

            var force = flameParticleSystem.forceOverLifetime;
            force.enabled = true;
            force.y = 15f;

            var velocityLimit = flameParticleSystem.limitVelocityOverLifetime;
            velocityLimit.enabled = false;

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = flameBehaviour.AudioSource.clip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb, Color color)
        {
            var ability = limb.gameObject.GetOrAddComponent<FireStream>();
            ability.Limb = limb;
            ability.flameColor = color;

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
                    flame.GetComponent<ParticleSystem>().Play();
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
                            var physHit = hit.collider.GetComponent<PhysicalBehaviour>();
                            if (physHit != null)
                            {
                                if (!physHit.OnFire)
                                physHit.Ignite();
                            }
                        }
                    }
                }

                if (phys.StoppedBeingUsedContinuously())
                {
                    flame.GetComponent<ParticleSystem>().Stop();
                    audioSource.Stop();
                }
            }
        }
    }
}