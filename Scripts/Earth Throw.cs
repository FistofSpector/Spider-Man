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
    // Add a list for physical properties that raycast can detect.
    public class EarthThrow : Ability
    {
        private float throwForce = 50f;
        private float lifetime = 1f;

        public override void Start()
        {
            base.Name = "Earth Throw";
            base.BodyPart = "Hands";
            base.Start();
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthThrow>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();
            
            var hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Debris"))
                {
                    return;
                }

                var phys = hit.collider.GetComponent<PhysicalBehaviour>();
                if (phys)
                {
                    if (phys.Properties.name == "Rock")
                    {
                        var rigidBody = hit.collider.GetComponent<Rigidbody2D>();

                        var rock = Instantiate(ModAPI.FindSpawnable("Small Boulder").Prefab, hit.collider.transform.position, Limb.transform.rotation);
                        var destroyable = rock.GetComponent<DestroyableBehaviour>();
                        destroyable.Break();

                        var hoverbox = hit.collider.GetComponent<HoverboxBehaviour>();
                        Destroy(hoverbox);

                        var direction = hit.point - (Vector2)Limb.transform.position;
                        direction.Normalize();
                        rigidBody.AddForce(direction * throwForce, ForceMode2D.Impulse);
                    }
                }
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}