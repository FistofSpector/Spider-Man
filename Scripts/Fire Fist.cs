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
    public class FireFist : Ability
    {
        private bool isActive = false;
        private GameObject flame;
        private GameObject embers;
        private AudioSource audioSource;

        public override void Start()
        {
            base.Name = "Fire Fist";
            base.BodyPart = "Hands";
            base.Start();
            
            var prefab = Instantiate(Resources.Load<GameObject>("Prefabs/PyroPowerEntity"));
            var flameBehaviour = prefab.GetComponent<PyroPowerToolEntity>();
            var flamePrefab = prefab.transform.GetChild(0);
            var embersPrefab = flamePrefab.GetChild(2);

            embers = Instantiate(embersPrefab.gameObject, Limb.transform);
            flame = Instantiate(flamePrefab.gameObject, Limb.transform);
            
            flame.GetComponent<ParticleSystem>().startSize = 1f;
            flame.GetComponent<ParticleSystem>().emissionRate = 5f;

            flame.transform.GetChild(0).GetComponent<ParticleSystem>().startSize = 1f;

            embers.GetComponent<ParticleSystem>().gravityModifier = 0f;
            embers.GetComponent<ParticleSystem>().emissionRate = 5f;

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = flameBehaviour.AudioSource.clip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<FireFist>();
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
                audioSource.Play();
                flame.GetComponent<ParticleSystem>().Play();
                embers.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                Deactivate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            audioSource.Stop();
            flame.GetComponent<ParticleSystem>().Stop();
            embers.GetComponent<ParticleSystem>().Stop();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (Enabled && isActive)
            {
                var hitPhys = collision.collider.GetComponent<PhysicalBehaviour>();
                if (hitPhys)
                {
                    hitPhys.Ignite();
                }
            }
        }
    }
}