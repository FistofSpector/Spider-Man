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
    public class EarthSummon : Ability
    {
        private HoverboxBehaviour hoverbox;
        private float summonForce = 9f;
        private float waitTime = 0.5f;
        private float lifetime = 5f;

        public override void Start()
        {
            base.Name = "Earth Summon";
            base.BodyPart = "Feet";
            base.Start();
        }
        
        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthSummon>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            if (Limb.IsOnFloor)
            {
                var summonOffset = CalculateDirection() * UnityEngine.Random.Range(1f, 2f);
                var summonPosition = (Vector2)Limb.transform.position + summonOffset;

                var rock = Instantiate(ModAPI.FindSpawnable("Small Boulder").Prefab, summonPosition, Quaternion.identity);
                var destroyable = rock.GetComponent<DestroyableBehaviour>();
                var dust = destroyable.DebrisPrefab.transform.GetChild(0);
                Instantiate(dust.gameObject, summonPosition, Quaternion.identity);
                
                var rigidBody = rock.GetComponent<Rigidbody2D>();
                rigidBody.AddForce(Vector2.up * summonForce, ForceMode2D.Impulse);

                hoverbox = rock.GetComponent<HoverboxBehaviour>();
                if (hoverbox == null)
                {
                    hoverbox = rock.AddComponent<HoverboxBehaviour>();
                }

                hoverbox.enabled = false;
                StartCoroutine(RiseRock(rock));
            }
            else
            {
                ModAPI.Notify("Person must be grounded!");
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public IEnumerator RiseRock(GameObject rock)
        {
            yield return new WaitForSeconds(waitTime);

            if (hoverbox != null)
            {
                hoverbox.enabled = true;
                hoverbox.Activated = true;
                hoverbox.Rigidbody = rock.GetComponent<Rigidbody2D>();
                hoverbox.PhysicalBehaviour = rock.GetComponent<PhysicalBehaviour>();

                Destroy(hoverbox, lifetime);
            }
        }

        public Vector2 CalculateDirection()
        {
            if (Limb.transform.root.localScale.x < 0)
            {
                return Vector2.left;
            }

            return Vector2.right;
        }
    }
}