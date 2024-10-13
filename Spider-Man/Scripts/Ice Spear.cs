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
    public class IceSpear : Ability
    {
        private float additionalForce = 25f;

        public override void Start()
        {
            base.Name = "Ice Spear";
            base.BodyPart = "Hands";
            base.Start();
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<IceSpear>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();
            
            RaycastHit2D hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
            if (hit.collider != null)
            {
                var physHit = hit.collider.GetComponent<PhysicalBehaviour>();
                if (physHit != null)
                {
                    var prefab = ModAPI.FindSpawnable("Knife").Prefab;

                    var direction = -Limb.transform.up;
                    var spawnOffset = (Vector2)direction * 0.5f;
                    var spawnPosition = (Vector2)Limb.transform.position + spawnOffset;

                    var iceSpear = Instantiate(prefab, spawnPosition, Quaternion.identity);
                    iceSpear.GetComponent<SpriteRenderer>().sprite = ResourceStorage.IceSpearList[UnityEngine.Random.Range(0, ResourceStorage.IceSpearList.Count)];
                    iceSpear.FixColliders();

                    iceSpear.GetComponent<PhysicalBehaviour>().PlayClipOnce(ResourceStorage.IceSpearClip);

                    var rb = iceSpear.GetComponent<Rigidbody2D>();
                    var force = direction.normalized * rb.mass * additionalForce;

                    var rotation = Quaternion.LookRotation(Vector3.forward, direction);
                    iceSpear.transform.rotation = rotation;
                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}