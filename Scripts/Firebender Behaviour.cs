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
    public class FirebenderBehaviour : MonoBehaviour
    {
        public void Start()
        {
            var person = gameObject.GetComponent<PersonBehaviour>();
            var newProperties = GameObject.Instantiate(person.Limbs[0].PhysicalBehaviour.Properties);
            newProperties.Flammability = 0.1f;

            foreach (LimbBehaviour limb in person.Limbs)
            {
                limb.PhysicalBehaviour.Properties = newProperties;
            }
        }
    }
}