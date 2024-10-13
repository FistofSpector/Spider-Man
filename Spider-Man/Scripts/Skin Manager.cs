using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AvatarTLA
{
    public class SkinManager : MonoBehaviour
    {
        [SkipSerialisation]
        public class SkinData
        {
            public Texture2D Texture { get; set; }
            public List<AccessoryData> Accessories { get; set; } = new List<AccessoryData>();
        }

        [SkipSerialisation]
        public class AccessoryData
        {
            public LimbBehaviour Limb { get; set; }
            public Sprite Sprite { get; set; }
            public Vector2 Position { get; set; }
        }

        public int currentIndex = 0;
        [SkipSerialisation]
        public List<SkinData> skins = new List<SkinData>();
        [SkipSerialisation]
        public PersonBehaviour person;

        public void Start()
        {
            person = GetComponent<PersonBehaviour>();

            if (skins.Count > 0)
            {
                UpdateSkin();

                person.Limbs[0].gameObject.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton("Next Skin", "Next Skin", "Next Skin", new UnityAction[1]
                {
                    (UnityAction) (() =>
                    {
                        NextSkin();
                    })
                }));

                person.Limbs[0].gameObject.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton("Previous Skin", "Previous Skin", "Previous Skin", new UnityAction[1]
                {
                    (UnityAction) (() =>
                    {
                        PreviousSkin();
                    })
                }));
            }
        }

        public void AddSkin(Texture2D skinTexture)
        {
            skins.Add(new SkinData { Texture = skinTexture });
        }

        public void NextSkin()
        {
            currentIndex = (currentIndex + 1) % skins.Count;
            UpdateSkin();
        }

        public void PreviousSkin()
        {
            currentIndex = (currentIndex - 1 + skins.Count) % skins.Count;
            UpdateSkin();
        }

        public void RandomSkin()
        {
            currentIndex = UnityEngine.Random.Range(0, skins.Count);
            UpdateSkin();
        }

        public void UpdateSkin()
        {
            person.SetBodyTextures(skins[currentIndex].Texture);
            ClearAccessories(person);

            foreach (AccessoryData accessoryData in skins[currentIndex].Accessories)
            {
                AddAccessory(accessoryData.Limb, accessoryData.Sprite, accessoryData.Position);
            }

            ModAPI.Notify("Skin Index: " + (currentIndex + 1) + " / " + skins.Count);
        }

        public static void ClearAccessories(PersonBehaviour person)
        {
            foreach (LimbBehaviour limb in person.Limbs)
            {
                foreach (Transform child in limb.transform)
                {
                    if (child.name == "Accessory")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        public void AddAccessoryToSkin(Texture2D skinTexture, LimbBehaviour accessoryLimb, Sprite accessorySprite, Vector2 accessoryPosition)
        {
            SkinData skinData = skins.Find(skin => skin.Texture == skinTexture);

            if (skinData != null)
            {
                skinData.Accessories.Add(new AccessoryData { Limb = accessoryLimb, Sprite = accessorySprite, Position = accessoryPosition });
            }
        }

        public static void AddAccessory(LimbBehaviour accessoryLimb, Sprite accessorySprite, Vector2 accessoryPosition)
        {
            GameObject accessory = new GameObject("Accessory");
            accessory.transform.SetParent(accessoryLimb.transform, false);
            accessory.transform.localPosition = accessoryPosition;
            accessory.transform.localScale = new Vector2(1f, 1f);
            accessory.transform.localRotation = Quaternion.identity;

            SpriteRenderer accessoryRenderer = accessory.AddComponent<SpriteRenderer>();
            accessoryRenderer.sprite = accessorySprite;

            if (accessoryLimb.RoughClassification == LimbBehaviour.BodyPart.Torso)
            {
                accessoryRenderer.sortingLayerName = "Foreground";
                accessoryRenderer.sortingOrder = accessoryLimb.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            else if (accessoryLimb.RoughClassification == LimbBehaviour.BodyPart.Legs)
            {
                accessoryRenderer.sortingLayerName = accessoryLimb.GetComponent<SpriteRenderer>().sortingLayerName;
                accessoryRenderer.sortingOrder = accessoryLimb.GetComponent<SpriteRenderer>().sortingOrder;
            }
            else
            {
                accessoryRenderer.sortingLayerName = accessoryLimb.GetComponent<SpriteRenderer>().sortingLayerName;
                accessoryRenderer.sortingOrder = accessoryLimb.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }

            accessory.AddComponent<Optout>();
        }
    }
}

// Copyright 2024 belknight & TheFistofSpector. This item is not authorized for posting on Steam, except under the Steam account named belknight & TheFistofSpector