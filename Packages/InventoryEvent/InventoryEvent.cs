using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;

namespace Feddas.Inventory
{
    /// <summary> Collection of items required before performing an action </summary>
    [Serializable]
    public class InventoryRequired
    {
        [Tooltip("0 to n items required to be in itemSource for this event to trigger.")]
        public List<ItemStack> RequiredItems;

        [Tooltip("What to do when RequiredItems match. Payload is always the Inventory of the object that collided with this.")]
        public UnityEvent<Inventory> ActionOnMatch;
    }

    public enum ItemSource
    {
        [Tooltip("The pool of items used to check if RequiredItems are satisfied is taken from the gameobject that has collided with this one. Used for door objects taking keys from the player. Or a key requiring coins to buy.")]
        Collider,

        [Tooltip("The pool of items used to check if RequiredItems are satisfied is taken from the Inventory on this gameobject. Used to kill the player when they collect too many curses.")]
        This,
    }

    public class InventoryEvent : MonoBehaviour
    {
        [Tooltip("Which pool of items will be used to check if RequiredItems are satisfied. Its own inventory, or the inventory of the gameobject it collided with.")]
        [SerializeField]
        private ItemSource itemSource;

        [Tooltip("itemSource must have **at least** the items listed here for events to be executed.")]
        public List<InventoryRequired> EventByInventoryMatch;

        // void Start() { }
        // void Update() { }

        void OnCollisionEnter2D(Collision2D collision)
        {
            OnTriggerEnter2D(collision.collider);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            Inventory inventoryCollidedInto = collider.gameObject.GetComponent<Inventory>();

            if (inventoryCollidedInto == null) // the object collided into must have an inventory
            {
                return;
            }

            StartCoroutine(decideInventorySource(inventoryCollidedInto));
        }

        IEnumerator decideInventorySource(Inventory inventoryCollidedInto)
        {
            Inventory inventorySource;
            switch (itemSource)
            {
                case ItemSource.Collider: // use the inventory collided into
                    inventorySource = inventoryCollidedInto;
                    break;
                case ItemSource.This:
                    yield return null; // wait a frame for items from collider to merge with this
                    inventorySource = this.GetComponent<Inventory>();
                    break;
                default:
                    throw new NotImplementedException(itemSource.ToString() + " isn't implemented.");
            }

            if (inventorySource != null) // There must be an inventory to satisfy RequiredItems
            {
                // Determine which, if any, InventoryEvent to raise
                InvokeEventUsing(inventorySource, inventoryCollidedInto);
            }
        }

        /// <summary> Waits 2 frames before SetActive to false. Those 2 frames give Inventory.cs a chance to merge inventory lists after 1 frame. </summary>
        public void SetActiveFalse()
        {
            SetActiveFalse(this.gameObject);
        }

        /// <param name="targetGameObject"> sets a specific gameobject to false. </param>
        public void SetActiveFalse(GameObject targetGameObject)
        {
            StartCoroutine(SetActiveIn2Frames(targetGameObject, false));
        }

        private IEnumerator SetActiveIn2Frames(GameObject targetGameObject, bool isActive)
        {
            yield return null;
            yield return null;

            targetGameObject.SetActive(isActive);
        }

        private void InvokeEventUsing(Inventory inventorySource, Inventory inventoryCollidedInto)
        {
            // Use SelectMany to duplicate each (s)tack.Item, (s)tack.Quantity times in a flat list of CollectableItems
            var itemsAvailable = inventorySource.MyInventory.SelectMany(s => Enumerable.Repeat(s.Item, s.Quantity));

            // filter down EventByInventoryMatch to only those that have their requirements met by the object collided with, inventoryEnablingEvent.
            var availableEvents = EventByInventoryMatch
                .Where(currentEvent => hasInventoryFor(currentEvent, itemsAvailable)).ToList();

            if (availableEvents.Count == 0) // InventoryEvent to raise
                return;

            // futher filter by chosing the single InventoryEvent that has the most items required met.
            var maxInventory = availableEvents
                .OrderByDescending(inventoryEvent => inventoryEvent.RequiredItems.Sum(i => i.Quantity))
                .First();

            maxInventory.ActionOnMatch.Invoke(inventoryCollidedInto);
        }

        /// <summary> Determines if an inventory contains the requirements to trigger this event </summary>
        private bool hasInventoryFor(InventoryRequired currentEvent, IEnumerable<CollectableItems> itemsAvailable)
        {
            var itemsRequired = currentEvent.RequiredItems.SelectMany(s => Enumerable.Repeat(s.Item, s.Quantity));
            var remainingRequired = itemsRequired.ToList();

            // All items required should intersect inside of available items  http://stackoverflow.com/questions/1520642/does-net-have-a-way-to-check-if-list-a-contains-all-items-in-list-b
            var intersect = itemsAvailable.Where(remainingRequired.Remove);

            // note: intersect.Count() causes remainingRequired.Remove to be performed. This means only the first count of intersect.Count() is correct. subsequent runs will use a mutated and incorrect remainingRequired.
            return intersect.Count() == itemsRequired.Count();
        }
    }
}
