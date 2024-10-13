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
    public class AbilityTemplate : Ability
    {
        private AudioSource audioSource;

        public override void Start()
        {
            base.Name = "Template";
            base.BodyPart = "Hands";
            base.Start();

            var prefab = Instantiate(Resources.Load<GameObject>("Prefabs/LightningToolEntity"));

            if (audioSource == null)
            {
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = ResourceStorage.LightningBoltClip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<AbilityTemplate>();
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
    }
}