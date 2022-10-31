using System;
using System.Collections.Generic;
using UnityEngine;

namespace Strategy2D
{
    public abstract class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemSprite;
        public Sprite itemIcon;
        public int itemWidth;
        public int itemHeight;
        public int positionX;
        public int positionY;
        public int maxHealthPoint;
        public int healthPoint;
        public bool isProductive = false;

    }
}