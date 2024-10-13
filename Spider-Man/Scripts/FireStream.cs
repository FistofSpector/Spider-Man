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
        private GameObject prefab;
        private FlamethrowerBehaviour flamePrefab;
        private GameObject muzzle;
        private AudioSource audioSource;

        public override void Start()
        {
            base.name = "Fire Stream";
            base.Start();
            
            prefab = ModAPI.FindSpawnable("Flamethrower").Prefab;
            flamePrefab = prefab.GetComponent<FlamethrowerBehaviour>();

            audioSource = limb.gameObject.AddComponent<AudioSource>();
            audioSource.clip = flamePrefab.GetComponent<AudioSource>().clip;
            audioSource.loop = true;

            muzzle = new GameObject("Muzzle");
            muzzle.transform.parent = limb.transform;

            var direction = CalculateDirection();
            if (direction == Vector2.left)
            {
                muzzle.transform.localPosition = new Vector3(0.25f, 0f, 0f);
                muzzle.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                muzzle.transform.localPosition = new Vector3(0.25f, 0f, 0f);
                muzzle.transform.localRotation = Quaternion.identity;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<FireStream>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            FlamethrowerBehaviour flame = limb.gameObject.GetOrAddComponent<FlamethrowerBehaviour>();
            
            flame.particlePrefab = flamePrefab.particlePrefab;
            flame.Effect = FlamethrowerBehaviour.SprayEffect.Ignite;
            flame.IgnitionSound = flamePrefab.IgnitionSound;
            flame.IgnitionSource = audioSource;
            flame.Point = limb.transform.position;
            flame.Range = flamePrefab.Range;
            flame.Angle = flamePrefab.Angle;
            flame.RayCount = flamePrefab.RayCount;
            flame.LayerMask = flamePrefab.LayerMask;
            flame.Muzzle = muzzle.transform;
        }

        public override void Deactivate()
        {
            
        }
    }
}