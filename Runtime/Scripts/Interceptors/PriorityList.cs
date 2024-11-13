using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KindMen.Uxios.Interceptors
{
    /// <summary>
    /// A priority-based list where items can be added with an associated priority.
    /// Items with the same priority are stored in the order they are added, and
    /// iteration occurs in ascending priority order.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the priority list.</typeparam>
    public class PriorityList<T> : IEnumerable<T>
    {
        private readonly SortedList<int, List<T>> buckets = new();

        /// <summary>
        /// Adds an item to the list with the specified priority. Throws an exception if the item already exists.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="priority">The priority level of the item. Defaults to 0 if not specified.</param>
        /// <exception cref="InvalidOperationException">Thrown if the item already exists in the list.</exception>
        public void Add(T item, int priority = 0)
        {
            // Check for duplicates in all buckets
            if (buckets.Any(bucket => bucket.Value.Contains(item)))
            {
                throw new InvalidOperationException("Item already exists in the PriorityList.");
            }

            // Add the item to the specified priority bucket
            if (!buckets.ContainsKey(priority))
            {
                buckets[priority] = new List<T>();
            }
            buckets[priority].Add(item);
        }

        /// <summary>
        /// Attempts to add an item to the list with the specified priority. 
        /// Does nothing and returns false if the item already exists.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="priority">The priority level of the item. Defaults to 0 if not specified.</param>
        /// <returns>True if the item was added successfully; false if the item already exists.</returns>
        public bool TryAdd(T item, int priority = 0)
        {
            // Do nothing if item is already present
            if (buckets.Any(bucket => bucket.Value.Contains(item)))
            {
                return false;
            }

            // Add the item to the specified priority bucket
            if (!buckets.ContainsKey(priority))
            {
                buckets[priority] = new List<T>();
            }
            buckets[priority].Add(item);

            return true;
        }

        /// <summary>
        /// Removes the specified item from the list if it exists.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed; false otherwise.</returns>
        public bool Remove(T item) => buckets.Values.Any(bucket => bucket.Remove(item));
        
        /// <summary>
        /// Removes all items from the list, clearing all priority buckets.
        /// </summary>
        public void Clear()
        {
            buckets.Clear();
        }

        /// <summary>
        /// Gets a value indicating whether the list is empty.
        /// </summary>
        public bool IsEmpty => !buckets.Any(b => b.Value.Count > 0);
 
        /// <summary>
        /// Returns an enumerator that iterates through all items in the list in ascending priority order.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return buckets.SelectMany(bucket => bucket.Value).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all items in the list in ascending priority order.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}