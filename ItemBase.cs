using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using BepInEx;
using UnityEngine;
using System.Reflection;

namespace ItemBase
{
    [BepInPlugin("com.xoxfaby.ItemBase", "BetterUI", "1.0.0.1")]
    public class ItemBase : BaseUnityPlugin
    {
        
    }

    public class BuffProvider
    {
        static List<BuffDef> buffDefs;
        static BuffProvider()
        {
            buffDefs = new List<BuffDef>();
            On.RoR2.ContentManager.SetContentPacks += ContentManager_SetContentPacks;
        }
        private static void ContentManager_SetContentPacks(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            ContentPack contentPack = new ContentPack();
            contentPack.buffDefs = buffDefs.ToArray();
            newContentPacks.Add(contentPack);
            orig(newContentPacks);
        }
        public static void AddBuff(BuffDef buffDef)
        {
            buffDefs.Add(buffDef);
        }
            
    }

    public class ItemProvider
    {
        List<ItemDef> itemDefs = new List<ItemDef>();
        AssetBundle bundle;
        public ItemProvider(AssetBundle bundle)
        {
            this.bundle = bundle;
            On.RoR2.ContentManager.SetContentPacks += ContentManager_SetContentPacks;
            ItemBase.print(String.Join("\n", bundle.GetAllAssetNames()));
        }

        private void ContentManager_SetContentPacks(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            ContentPack itemPack = new ContentPack();
            itemPack.itemDefs = itemDefs.ToArray();
            newContentPacks.Add(itemPack);
            orig(newContentPacks);
        }

        public Item AddItem(Item item)
        {
            item.Init(bundle);
            item.Hook();
            itemDefs.Add(item.itemDef);
            return item;
        }
    }
    public class Item
    {
        protected GameObject Prefab;
        protected Sprite Icon;
        protected ItemTemplate itemTemplate;

        public ItemDef itemDef;
        public void Init(AssetBundle bundle)
        {
            Prefab = bundle.LoadAsset<GameObject>($"Assets/Items/{itemTemplate.internalName}/prefab.prefab");
            Icon = bundle.LoadAsset<Sprite>($"Assets/Items/{itemTemplate.internalName}/icon.png");

            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemTemplate.internalName;
            itemDef.tier = itemTemplate.tier;
            itemDef.pickupModelPrefab = Prefab;
            itemDef.pickupIconSprite = Icon;
            itemDef.nameToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_NAME";
            itemDef.pickupToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_PICKUP";
            itemDef.descriptionToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_DESC";
            itemDef.loreToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_LORE";
            itemDef.tags = itemTemplate.tags ?? new ItemTag[] { };

            LanguageHelper.Add(itemDef.nameToken, itemTemplate.name);
            LanguageHelper.Add(itemDef.pickupToken, itemTemplate.pickupText);
            LanguageHelper.Add(itemDef.descriptionToken, itemTemplate.descriptionText);
            LanguageHelper.Add(itemDef.loreToken, itemTemplate.loreText);
        }

        public ItemIndex itemIndex
        {
            get { return itemDef.itemIndex; }
        }
        

        public virtual void Hook() { }

        public struct ItemTemplate
        {
            public string internalName;
            public string name;
            public string pickupText;
            public string descriptionText;
            public string loreText;
            public ItemTier tier;
            public ItemTag[] tags;
            public ItemDisplayRule[] itemDisplayRules;
        }
    }
}
