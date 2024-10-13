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
    public class FireComet : Ability
    {
        private AudioSource audioSource;
        private ExcaliburBehaviour excalibur;
        private GameObject muzzle;

        public override void Start()
        {
            base.Name = "Fire Comet";
            base.BodyPart = "Hands";
            base.Start();

            muzzle = new GameObject("Muzzle");
            muzzle.transform.parent = Limb.transform;
            muzzle.transform.localRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(0f, 0f, -90f));

            var direction = -Limb.transform.up;
            var spawnOffset = (Vector2)direction * 0.5f;
            var spawnPosition = (Vector2)Limb.transform.position + spawnOffset;
            muzzle.transform.localPosition = spawnOffset;

            if (audioSource == null)
            {
                var audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = prefab.GetComponent<AudioSource>().clip;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<FireComet>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            var prefab = ModAPI.FindSpawnable("Excalibur").Prefab;
            var excaliburPrefab = prefab.GetComponent<ExcaliburBehaviour>();
            excalibur = Limb.gameObject.GetOrAddComponent<ExcaliburBehaviour>();

            excalibur.ChargeAudioSource = audioSource;
            excalibur.FireBallPrefab = excaliburPrefab.FireBallPrefab;
            excalibur.SpellChargeCurve = excaliburPrefab.SpellChargeCurve;
            excalibur.SpellChargeGlow = excaliburPrefab.SpellChargeGlow;
            excalibur.SpellChargeParticleSystem = excaliburPrefab.SpellChargeParticleSystem;
            excalibur.SpellForce = 1f;
            excalibur.SpawnPoint = muzzle.transform;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Destroy(excalibur);
        }
    }
}