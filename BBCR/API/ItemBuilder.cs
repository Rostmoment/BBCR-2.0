using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBCR.API
{
    class ItemBuilder
    {
        private Sprite large;
        private Sprite small;
        private string name;
        private Items type;
        public ItemObject Build() => Build<Item>();
        public ItemObject Build<T>() where T : Item
        {
            ItemObject res = ScriptableObject.CreateInstance<ItemObject>();
            if (large == null && small == null) small = Texture2D.whiteTexture.ToSprite();
            if (large == null) large = small;
            if (small == null) small = large;
            res.itemSpriteSmall = small;
            res.itemSpriteLarge = large;
            res.itemType = type;
            res.nameKey = name;
            res.item = new GameObject(name).AddComponent<T>();
            return res;
        }
        public ItemBuilder SetEnum(string toSet) => SetEnum(toSet.ToEnum<Items>());
        public ItemBuilder SetEnum(Items toSet)
        {
            type = toSet;
            return this;
        }
        public ItemBuilder SetName(string toSet)
        {
            name = toSet;
            return this;
        }
        public ItemBuilder SetLargeSprite(Texture2D tex, float pixelsPerUnit = 1f) =>
            SetLargeSprite(tex.ToSprite(pixelsPerUnit));
        public ItemBuilder SetLargeSprite(Texture2D tex, Vector2 center, float pixelsPerUnit = 1f) =>
            SetLargeSprite(tex.ToSprite(center, pixelsPerUnit));
        public ItemBuilder SetLargeSprite(Sprite sprite)
        {
            large = sprite;
            return this;
        }
        public ItemBuilder SetSmallSprite(Texture2D tex, float pixelsPerUnit = 1f) =>
            SetSmallSprite(tex.ToSprite(pixelsPerUnit));
        public ItemBuilder SetSmallSprite(Texture2D tex, Vector2 center, float pixelsPerUnit = 1f) =>
            SetSmallSprite(tex.ToSprite(center, pixelsPerUnit));
        public ItemBuilder SetSmallSprite(Sprite sprite) 
        {
            small = sprite; 
            return this;
        }
        public ItemBuilder SetSprites(Sprite small, Sprite large) => SetLargeSprite(large).SetSmallSprite(small);
    }
}
