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

        public override void Start()
        {
            base.name = "Fire Fist";
            base.bodyPart = "Arms"
            base.Start();
            
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Prefabs/PyroPowerEntity"));
            Transform flamePrefab = prefab.transform.GetChild(0);
            Transform embersPrefab = flamePrefab.GetChild(2);

            embers = Instantiate(embersPrefab.gameObject, limb.transform);
            flame = Instantiate(flamePrefab.gameObject, limb.transform);
            
            flame.GetComponent<ParticleSystem>().startSize = 1f;
            flame.GetComponent<ParticleSystem>().emissionRate = 5f;

            flame.transform.GetChild(0).GetComponent<ParticleSystem>().startSize = 1f;

            embers.GetComponent<ParticleSystem>().gravityModifier = 0f;
            embers.GetComponent<ParticleSystem>().emissionRate = 5f;
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<FireFist>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }
        
        public override void Activate()
        {
            isActive = !isActive;
            if (isActive)
            {
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
            flame.GetComponent<ParticleSystem>().Stop();
            embers.GetComponent<ParticleSystem>().Stop();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled && isActive)
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