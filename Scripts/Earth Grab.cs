using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// When grabbing a rock it'll hover and rotate around the limb. The collision between each limb and the rock becomes disabled while grabbed.
// The player can then switch the ability on the hands to Earth Throw to hit the rock that is in the cycle. The cycle can hold a max of 3 rocks and must move with the limb.

namespace AvatarTLA
{
    public class EarthGrab : Ability
    {
        private bool isActive;
        private GameObject grabbedObject = null;
        private HoverboxBehaviour hoverbox;
        private float waitTime = 0.5f;
        private float lifetime = 10f;

        public override void Start()
        {
            base.Name = "Earth Grab";
            base.BodyPart = "Hands";
            base.Start();
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<EarthGrab>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();

            isActive = !isActive;

            if (isActive)
            {
                ModAPI.Notify("Grabbed rock");

                var hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up);
                if (hit.collider != null)
                {
                    var phys = hit.collider.gameObject.GetComponent<PhysicalBehaviour>();
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
            base.Deactivate();
        }

        // Move toward the position captured when the ability was activated.
        private IEnumerator MoveObjectTowardsLimb()
        {
            while (isActive && grabbedObject != null)
            {
                var offset = CalculateDirection() * 1f;
                var targetPosition = Limb.transform.position + offset;
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