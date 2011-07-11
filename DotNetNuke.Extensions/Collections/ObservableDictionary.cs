using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DotNetNuke.Extensions.Collections
{
    /// <summary>
    /// An observable dictionary that allows a subscriber to be notified of changes to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">Dictionary Key Type.</typeparam>
    /// <typeparam name="TValue">Dictionary Value Type.</typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constants
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private const string KeysName = "Keys";
        private const string ValuesName = "Values";
        #endregion

        #region Protected Properties
        private IDictionary<TKey, TValue> _Dictionary;
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _Dictionary; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public ObservableDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(comparer);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ObservableDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="comparer">The comparer.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }
        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
        ///   
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return Dictionary.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if key was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");

            TValue value;
            Dictionary.TryGetValue(key, out value);
            var removed = Dictionary.Remove(key);
            if (removed) OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            return removed;
        }

        /// <summary>
        /// Tries the retrieving the value. Returns true if fetched successfully, otherwise false.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</returns>
        public ICollection<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>The element with the specified key.</returns>
        ///   
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">key is null.</exception>
        ///   
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and key is not found.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                Insert(key, value, false);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get { return Dictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }


        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region General Members
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                        foreach (var item in items) Dictionary.Add(item);
                }
                else
                    _Dictionary = new Dictionary<TKey, TValue>(items);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null) throw new ArgumentNullException("key");

            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add) throw new ArgumentException("An item with the same key has already been added.");
                if (Equals(item, value)) return;
                Dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        private void OnPropertyChanged()
        {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="changedItem">The changed item.</param>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="newItems">The new items.</param>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }
        #endregion
    }
}
