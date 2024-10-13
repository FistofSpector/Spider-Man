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
    public class MeteorShower : Ability, Messages.IUseContinuous
    {
        private bool isFinished = true;
        private float throwForce = 50f;
        private float spawnInterval = 0.1f;

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<MeteorShower>();
            ability.limb = limb;

            AbilityManager abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {

        }

        public override void Deactivate()
        {
            
        }

        public void UseContinuous(ActivationPropagation activation)
        {
            if (enabled)
            {
                if (isFinished == true)
                {
                    isFinished = false;
                    StartCoroutine(SpawnRock());
                }
            }
        }

        private IEnumerator SpawnRock()
        {
            RaycastHit2D hit = Physics2D.Raycast(limb.transform.position, -limb.transform.up);
            if (hit.collider != null)
            {
                Rigidbody2D rigidbody = hit.collider.GetComponent<Rigidbody2D>();
                if (rigidbody)
                {
                    float randomXOffset = UnityEngine.Random.Range(-10f, 10f);
                    Vector2 summonOffset = new Vector2(randomXOffset, 25f);
                    Vector2 summonPosition = (Vector2)limb.transform.position + summonOffset;

                    float randomScale = UnityEngine.Random.Range(0.5f, 3f);

                    GameObject rockPrefab = ModAPI.FindSpawnable("Small Boulder").Prefab;
                    GameObject rock = Instantiate(rockPrefab, summonPosition, Quaternion.identity);
                    rock.transform.localScale *= randomScale;

                    Rigidbody2D rockRigidbody = rock.GetComponent<Rigidbody2D>();

                    Vector2 direction = (hit.point - summonPosition).normalized;
                    float mass = rockRigidbody.mass;
                    Vector2 force = direction * mass * 50f;

                    rockRigidbody.AddForce(force, ForceMode2D.Impulse);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
            isFinished = true;
        }
    }
}