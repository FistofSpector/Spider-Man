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
    public class AbilityManager : MonoBehaviour
    {
        public List<Ability> abilities = new List<Ability>();
        public int currentIndex = 0;

        public void Start()
        {
            var limb = gameObject.GetComponent<LimbBehaviour>();

            abilities[currentIndex].Enabled = true;

            limb.gameObject.GetOrAddComponent<UseEventTrigger>().Action = () =>
            {
                if (abilities[currentIndex].Enabled)
                {
                    abilities[currentIndex].Activate();
                }
            };

            limb.gameObject.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton("Switch Ability", "Switch Ability", "Switch Ability", new UnityAction[1]
            {
                (UnityAction) (() =>
                {
                    SwitchAbility();
                })
            }));
        }

        public void AddAbility(Ability ability)
        {
            abilities.Add(ability);
        }

        public void SwitchAbility()
        {
            ModAPI.Notify("Ability Equipped: " + abilities[currentIndex].Name);

            abilities[currentIndex].Deactivate();
            abilities[currentIndex].Enabled = false;
            
            currentIndex = (currentIndex - 1 + abilities.Count) % abilities.Count;
            abilities[currentIndex].Enabled = true;
            
            ModAPI.Notify("Ability Index: " + (currentIndex + 1) + " / " + abilities.Count);
        }
    }
}