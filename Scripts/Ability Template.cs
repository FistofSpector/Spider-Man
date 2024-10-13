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
        public override void Start()
        {
            base.Name = "Template";
            base.BodyPart = "Hands";
            base.Start();
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

// Copyright 2024 belknight & TheFistofSpector. This item is not authorized for posting on Steam, except under the Steam account named belknight & TheFistofSpector