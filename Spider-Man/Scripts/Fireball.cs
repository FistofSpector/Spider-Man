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
    public class Fireball : Ability
    {
        public override void Start()
        {
            base.name = "Fire Ball";
            base.Start();

            GameObject prefab = ModAPI.FindSpawnable("Excalibur").Prefab;
            ExcaliburBehaviour excaliburPrefab = prefab.GetComponent<ExcaliburBehaviour>();
            ExcaliburBehaviour excalibur = limb.gameObject.AddComponent<ExcaliburBehaviour>();

            var audioSource = limb.gameObject.AddComponent<AudioSource>();
            audioSource.clip = prefab.GetComponent<AudioSource>().clip;

            excalibur.ChargeAudioSource = audioSource;
            excalibur.FireBallPrefab = excaliburPrefab.FireBallPrefab;
            excalibur.SpellChargeCurve = excaliburPrefab.SpellChargeCurve;
            excalibur.SpellChargeGlow = excaliburPrefab.SpellChargeGlow;
            excalibur.SpellChargeParticleSystem = excaliburPrefab.SpellChargeParticleSystem;
            excalibur.SpellForce = 1f;

            var muzzle = new GameObject("ExcaliburMuzzle");
            muzzle.transform.parent = limb.transform;

            // Vector2 direction = CalculateDirection();
            // if (direction == Vector2.left)
            // {
            //     muzzle.transform.localPosition = new Vector3(0.25f, 0f, 0f);
            //     muzzle.transform.localRotation = Quaternion.identity;
            // }
            // else
            // {
            //     muzzle.transform.localPosition = new Vector3(0.25f, 0f, 0f);
            //     muzzle.transform.localRotation = Quaternion.Euler(0, 0, 0);
            // }

            excalibur.SpawnPoint = muzzle.transform;
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<Fireball>();
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