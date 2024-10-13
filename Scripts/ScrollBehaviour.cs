// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;

// namespace AvatarTLA
// {
//     public class ScrollBehaviour : MonoBehaviour, Messages.IOnGripped
//     {
//         public List<Ability> abilities = new List<Ability>();
//         private Ability equippedAbility;
//         private GripBehaviour gripper;

//         public void Start()
//         {
//             var useEventTrigger = gameObject.GetOrAddComponent<UseEventTrigger>();
//             useEventTrigger.Action = () =>
//             {
//                 if (equippedAbility)
//                 {
//                     ApplyAbility(equippedAbility);
//                 }
//             };
//         }

//         public void AddAbilityToScroll(Ability ability)
//         {
//             abilities.Add(ability);

//             gameObject.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton(ability.name + " Ability", ability.name + "Ability", ability.name + "Ability", new UnityAction[1]
//             {
//                 (UnityAction) (() =>
//                 {
//                     equippedAbility = ability;
//                 })
//             }));
//         }

//         public void ApplyAbility(Ability ability)
//         {
//             if (gripper != null)
//             {
//                 if (gripper.CurrentlyHolding == gameObject.GetComponent<PhysicalBehaviour>())
//                 {
//                     foreach (var limb in gripper.GetComponent<PersonBehaviour>().Limbs)
//                     {
//                         if (equippedAbility.BodyPart == "Active")
//                         {
//                             equippedAbility.AddAbility(gripper.GetComponent<PersonBehaviour>());
//                             return;
//                         }
//                         else if (equippedAbility.BodyPart == "Head")
//                         {
//                             equippedAbility.AddAbility(limb.Person.Limbs[0]);
//                             return;
//                         }
//                         else if (equippedAbility.BodyPart == "Body")
//                         {
//                             equippedAbility.AddAbility(limb.Person.Limbs[1]);
//                             return;
//                         }
//                         else if (equippedAbility.BodyPart == "Hands")
//                         {
//                             equippedAbility.AddAbility(limb.Person.Limbs[11]);
//                             equippedAbility.AddAbility(limb.Person.Limbs[13]);
//                             return;
//                         }
//                         else if (equippedAbility.BodyPart == "Feet")
//                         {
//                             equippedAbility.AddAbility(limb.Person.Limbs[6]);
//                             equippedAbility.AddAbility(limb.Person.Limbs[9]);
//                             return;
//                         }
//                     }
//                 }
//             }
//         }

//         public void OnGripped(GripBehaviour gripper)
//         {
//             this.gripper = gripper.GetComponent<PersonBehaviour>();
//         }
//     }
// }