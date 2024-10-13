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
    public class WaterHeal : Ability
    {
        private GameObject water;
        private LightSprite glow;
        private AudioSource audioSource;
        private PersonBehaviour personTouching;
        private bool isActive = false;
        private float healRate = 1f;

        public override void Start()
        {
            base.Name = "Water Heal";
            base.BodyPart = "Hands";
            base.Start();

            var spawnable = ModAPI.FindSpawnable("Liquid Outlet");
            var liquid = spawnable.Prefab.transform.Find("BleedingParticle").transform.Find("Trail").gameObject;

            water = Instantiate(liquid, Limb.transform);
            water.transform.localPosition = Vector3.zero;
            water.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vector3.down);

            var waterParticleSystem = water.GetComponent<ParticleSystem>();
            waterParticleSystem.loop = true;
            waterParticleSystem.emissionRate = 5f;
            waterParticleSystem.playbackSpeed = 3f;

            var main = waterParticleSystem.main;
            main.startColor = new Color(1f, 1f, 1f);
            main.startSize = 5f;
            main.startDelay = 0f;
            main.startLifetime = 5f;
            main.gravityModifier = 0f;

            var shape = waterParticleSystem.shape;
            shape.enabled = false;

            var velocityLimit = waterParticleSystem.limitVelocityOverLifetime;
            velocityLimit.enabled = false;

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = null;
                audioSource.loop = false;
            }

            glow = ModAPI.CreateLight(Limb.transform, new Color(1f, 1f, 1f), 1f, 5f);
            glow.transform.SetParent(Limb.transform, false);
            glow.gameObject.SetActive(false);
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<WaterHeal>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            isActive = !isActive;
            if (isActive)
            {
                glow.gameObject.SetActive(true);
                water.GetComponent<ParticleSystem>().Play();
                audioSource.Play();
            }
            else
            {
                glow.gameObject.SetActive(false);
                water.GetComponent<ParticleSystem>().Stop();
                audioSource.Stop();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            glow.gameObject.SetActive(false);
            water.GetComponent<ParticleSystem>().Stop();
            audioSource.Stop();
        }

        public void Update()
        {
            if (Enabled && isActive && personTouching != null)
            {
                ModAPI.Notify("Healing Person!");
                personTouching.Braindead = false;
                personTouching.BrainDamaged = false;
                
                foreach (var limbTarget in personTouching.Limbs)
                {
                    limbTarget.Health += healRate;
                    limbTarget.CirculationBehaviour.HealBleeding();
                    limbTarget.HealBone();
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (Enabled && isActive)
            {
                var limbHit = collision.collider.GetComponent<LimbBehaviour>();
                if (limbHit != null)
                {
                    personTouching = limbHit.Person;
                }
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (Enabled && isActive)
            {
                var limbHit = collision.collider.GetComponent<LimbBehaviour>();
                if (limbHit != null && limbHit.Person == personTouching)
                {
                    personTouching = null;
                }
            }
        }
    }
}