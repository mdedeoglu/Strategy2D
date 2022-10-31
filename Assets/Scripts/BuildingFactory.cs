using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Strategy2D
{
    public class BuildingFactory : MonoBehaviour
    {
        public static class BuildFactory
        {
            public static Dictionary<string, Type> itemsByName;
            private static bool IsInitialized => itemsByName != null;


            public static void InitializeFactory()
            {
                if (IsInitialized)
                    return;
                var itemTypes = Assembly.GetAssembly(typeof(BuildingItem)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BuildingItem)));
                itemsByName = new Dictionary<string, Type>();

                foreach (var type in itemTypes)
                {
                    var tempEffect = Activator.CreateInstance(type) as Item;
                    itemsByName.Add(tempEffect.itemName, type);
                }
            }
            public static Item GetItem(string itemType)
            {
                InitializeFactory();
                if (itemsByName.ContainsKey(itemType))
                {
                    Type type = itemsByName[itemType];
                    var item = Activator.CreateInstance(type) as Item;
                    return item;
                }
                return null;
            }
            internal static IEnumerable<string> GetItemNames()
            {
                InitializeFactory();
                return itemsByName.Keys;
            }
        }
    }
}