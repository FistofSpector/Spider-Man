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
    // Add a list for physical properties that raycast can detect. Add a lifetime for the debris.
    public class EarthThrow : Ability
    {
        private float throwForce = 50f;
        private float lifetime = 1f;

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthThrow>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            RaycastHit2D hit = Physics2D.Raycast(limb.transform.position, -limb.transform.up);
            if (hit.collider != null)
            {
                PhysicalBehaviour phys = hit.collider.GetComponent<PhysicalBehaviour>();
                if (phys)
                {
                    if (phys.Properties.name == "Rock")
                    {
                        Rigidbody2D rigidBody = hit.collider.GetComponent<Rigidbody2D>();

                        GameObject rock = Instantiate(ModAPI.FindSpawnable("Small Boulder").Prefab, hit.collider.transform.position, limb.transform.rotation);
                        DestroyableBehaviour destroyable = rock.GetComponent<DestroyableBehaviour>();
                        destroyable.Break();

                        HoverboxBehaviour hoverbox = hit.collider.GetComponent<HoverboxBehaviour>();
                        Destroy(hoverbox);

                        Vector2 direction = hit.point - (Vector2)limb.transform.position;
                        direction.Normalize();
                        rigidBody.AddForce(direction * throwForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        public override void Deactivate()
        {
            
        }
    }
}