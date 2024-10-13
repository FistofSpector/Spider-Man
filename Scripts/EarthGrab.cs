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
    public class EarthGrab : Ability
    {
        private bool isActive = false;
        private GameObject grabbedObject = null;
        private HoverboxBehaviour hoverbox;
        private float waitTime = 0.5f;
        private float lifetime = 10f;

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthGrab>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            isActive = !isActive;

            if (isActive)
            {
                ModAPI.Notify("Grabbed rock");
                RaycastHit2D hit = Physics2D.Raycast(limb.transform.position, -limb.transform.up);

                if (hit.collider != null)
                {
                    PhysicalBehaviour phys = hit.collider.gameObject.GetComponent<PhysicalBehaviour>();
                    if (phys)
                    {
                        grabbedObject = hit.collider.gameObject;
                        StartCoroutine(MoveObjectTowardsLimb());
                        StartCoroutine(GrabRock(grabbedObject));
                    }
                }
            }
            else
            {
                ModAPI.Notify("Dropped rock..");
                grabbedObject = null;
                Destroy(hoverbox, 3f);
                StopCoroutine(MoveObjectTowardsLimb());
            }
        }

        public override void Deactivate()
        {

        }

        // just move toward the position captured when the ability was activated.
        private IEnumerator MoveObjectTowardsLimb()
        {
            while (isActive && grabbedObject != null)
            {
                Vector3 offset = CalculateDirection() * 1f;
                Vector3 targetPosition = limb.transform.position + offset;
                grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, targetPosition, 1f * Time.deltaTime);
                yield return null;
            }
        }

        public IEnumerator GrabRock(GameObject rock)
        {
            yield return new WaitForSeconds(waitTime);
            
            hoverbox = grabbedObject.AddComponent<HoverboxBehaviour>();
            hoverbox.Activated = true;
            hoverbox.Rigidbody = grabbedObject.GetComponent<Rigidbody2D>();
            hoverbox.PhysicalBehaviour = grabbedObject.GetComponent<PhysicalBehaviour>();

            //Destroy(hoverbox, 3f);
        }
    }
}