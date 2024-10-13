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
    public class MeteorShower : Ability
    {
        private AudioSource audioSource;
        private bool isActive;
        private float throwForce = 25f;
        private float spawnInterval = 0.5f;

        public override void Start()
        {
            base.Name = "Meteor Shower";
            base.BodyPart = "Hands";
            base.Start();

            if (audioSource == null)
            {
                Limb.gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                audioSource = Limb.gameObject.AddComponent<AudioSource>();
                audioSource.clip = ResourceStorage.MeteorShowerClip;
                audioSource.loop = true;
            }
        }

        public static void AddAbility(LimbBehaviour limb)
        {
            var ability = limb.gameObject.GetOrAddComponent<MeteorShower>();
            ability.Limb = limb;

            var abilityManager = limb.gameObject.GetOrAddComponent<AbilityManager>();
            abilityManager.AddAbility(ability);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public void Update()
        {
            if (Enabled)
            {
                var phys = Limb.GetComponent<PhysicalBehaviour>();
                if (phys.StartedBeingUsedContinuously())
                {
                    audioSource.Play();
                }

                if (phys.IsBeingUsedContinuously())
                {
                    if (!isActive)
                    {
                        isActive = true;
                        StartCoroutine(SpawnRock());
                    }
                }

                if (phys.StoppedBeingUsedContinuously())
                {
                    audioSource.Stop();
                }
            }
        }

        public IEnumerator SpawnRock()
        {
            int layerToIgnore = LayerMask.GetMask("Debris");

            var hit = Physics2D.Raycast(Limb.transform.position, -Limb.transform.up, 250f, ~layerToIgnore);
            if (hit.collider != null)
            {
                var physHit = hit.collider.GetComponent<PhysicalBehaviour>();
                if (physHit != null)
                {
                    var randomXOffset = UnityEngine.Random.Range(15f, 15f);
                    var summonOffset = new Vector2(randomXOffset, 30f);
                    var summonPosition = (Vector2)Limb.transform.position + summonOffset;

                    var randomScale = UnityEngine.Random.Range(0.5f, 3f);

                    var rockPrefab = ModAPI.FindSpawnable("Small Boulder").Prefab;
                    var rock = Instantiate(rockPrefab, summonPosition, Quaternion.identity);
                    rock.transform.localScale *= randomScale;

                    var prefab = Instantiate(Resources.Load<GameObject>("Prefabs/PyroPowerEntity"));
                    var flamePrefab = prefab.transform.GetChild(0);
                    var flame = Instantiate(flamePrefab.gameObject, rock.transform);

                    var flameParticleSystem = flame.GetComponent<ParticleSystem>();
                    flameParticleSystem.GetComponent<ParticleSystem>().startSize = rock.transform.localScale.x;
                    flameParticleSystem.GetComponent<ParticleSystem>().emissionRate = 5f;
                    flameParticleSystem.GetComponent<ParticleSystem>().Play();

                    var rockRigidbody = rock.GetComponent<Rigidbody2D>();
                    var offset = CalculateDirection() * 1f;
                    var direction = (hit.point - summonPosition).normalized;
                    var mass = rockRigidbody.mass;
                    var force = direction * mass * throwForce;

                    rockRigidbody.AddForce(force, ForceMode2D.Impulse);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
            isActive = false;
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