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
    public abstract class Ability : MonoBehaviour
    {
        public string Name;
        public string BodyPart; // Head, Torso, Hands, Feet, or Passive
        public LimbBehaviour Limb;
        public bool Enabled = false;

        public float CooldownDuration = 0f;
        public bool IsOnCooldown = false;

        public virtual void Start()
        {
            var grip = Limb.GetComponent<LimbBehaviour>().GripBehaviour;
            if (grip != null)
            {
                Destroy(grip);
            }
        }

        public virtual void Activate()
        {
            if (IsOnCooldown && !Enabled) return;
            StartCoroutine(Cooldown());
        }

        public virtual void Deactivate(){}

        private IEnumerator Cooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(CooldownDuration);
            IsOnCooldown = false;
        }
    }
}

// Copyright 2024 belknight & TheFistofSpector. This item is not authorized for posting on Steam, except under the Steam account named belknight & TheFistofSpector