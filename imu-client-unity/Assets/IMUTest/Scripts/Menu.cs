using System.Collections.Generic;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class Menu : MonoBehaviour
    {
        private const float MinimumDistanceBetweenItems = 0.2f;
        
        [Header("Components")]
        public List<MenuItem> MenuItems;
        public Camera Camera;
        
        [Header("Run-time Data")]
        private int _itemIndex;

        private void Awake()
        {
            ResetItems();
        }

        public void ResetItems()
        {
            _itemIndex = 0;
            
            for (var i = 0; i < MenuItems.Count; i++)
            {
                var item = MenuItems[i];
                
                item.Hide();
            }
        }

        public bool CanSpawnItemAtPosition(Vector3 position)
        {
            // If all items have been spawned, don't allow spawning a new item.
            if (_itemIndex >= MenuItems.Count) return false;
            
            for (var i = 0; i < MenuItems.Count; i++)
            {
                var item = MenuItems[i];
                
                var distance = Vector3.Distance(item.transform.position, position);
                
                if (distance < MinimumDistanceBetweenItems)
                {
                    return false;
                }
            }

            return true;
        }

        public void SpawnItemAtPosition(Vector3 position)
        {
            var item = MenuItems[_itemIndex];
            
            var rotation = Quaternion.LookRotation((Camera.transform.position - position).normalized, Vector3.up);
            item.Spawn(position, rotation);
            
            _itemIndex++;
        }
    }
}
