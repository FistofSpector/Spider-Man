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
        private float lifetime = 3f;

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthSummon>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            if (limb.IsOnFloor)
            {
                Vector2 summonOffset = CalculateDirection() * UnityEngine.Random.Range(1f, 2f);
                Vector2 summonPosition = (Vector2)limb.transform.position + summonOffset;

                GameObject rock = Instantiate(ModAPI.FindSpawnable("Small Boulder").Prefab, summonPosition, Quaternion.identity);
                DestroyableBehaviour destroyable = rock.GetComponent<DestroyableBehaviour>();

                Transform dust = destroyable.DebrisPrefab.transform.GetChild(0);
                Instantiate(dust.gameObject, summonPosition, Quaternion.identity);

                Rigidbody2D rigidBody = rock.GetComponent<Rigidbody2D>();
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
                ModAPI.Notify("Person is not grounded..");
            }
        }

        public override void Deactivate()
        {

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
    }
}