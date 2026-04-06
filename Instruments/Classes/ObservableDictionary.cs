using System.Collections;
using System.Collections.Specialized;

namespace LCLocalizationInterface.Instruments
{
    namespace Classes
    {
        /// <summary>
        /// Used only in the <see cref="LimbusRegistry.JsonTypes.ObservationLog"/>, huh
        /// </summary>
        [PropertyChanged.DoNotNotify] // PropertyChanged.Fody
        public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IDictionary where TKey : notnull
        {
            private readonly Dictionary<TKey, TValue> ActualDictionary;







            #region Constructors
            public ObservableDictionary()
                => ActualDictionary = [];

            public ObservableDictionary(IDictionary<TKey, TValue> SourceDictionary)
                => ActualDictionary = new Dictionary<TKey, TValue>(SourceDictionary);

            public ObservableDictionary(IEqualityComparer<TKey> Comparer)
                => ActualDictionary = new Dictionary<TKey, TValue>(Comparer);

            public ObservableDictionary(int Capacity)
                => ActualDictionary = new Dictionary<TKey, TValue>(Capacity);

            public ObservableDictionary(IDictionary<TKey, TValue> SourceDictionary, IEqualityComparer<TKey> Comparer)
                => ActualDictionary = new Dictionary<TKey, TValue>(SourceDictionary, Comparer);
                
            public ObservableDictionary(int Capacity, IEqualityComparer<TKey> Comparer)
                => ActualDictionary = new Dictionary<TKey, TValue>(Capacity, Comparer);
            #endregion







            #region INotify(Collection|Property)Changed events
            public event NotifyCollectionChangedEventHandler? CollectionChanged;
            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs Args) => CollectionChanged?.Invoke(this, Args);

            protected virtual void FullOnPropertyChanged()
            {
                void OnPropertyChanged(string PropertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                OnPropertyChanged("Item[]");
            }
            #endregion







            #region IDictionary<TKey, TValue> interface

            public TValue this[TKey Key]
            {
                get => ActualDictionary[Key];
                set
                {
                    if (ActualDictionary.TryGetValue(Key, out var OldValue))
                    {
                        if (!Equals(OldValue, value))
                        {
                            ActualDictionary[Key] = value;
                            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                new KeyValuePair<TKey, TValue>(Key, value),
                                new KeyValuePair<TKey, TValue>(Key, OldValue)));
                            FullOnPropertyChanged();
                        }
                    }
                    else
                    {
                        this.Add(Key, value);
                    }
                }
            }

            public ICollection<TKey> Keys => ActualDictionary.Keys;
            public ICollection<TValue> Values => ActualDictionary.Values;

            public void Add(TKey Key, TValue Value)
            {
                ArgumentNullException.ThrowIfNull(Key);

                ActualDictionary.Add(Key, Value);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    new KeyValuePair<TKey, TValue>(Key, Value)
                ));
                FullOnPropertyChanged();
            }

            public bool Remove(TKey Key)
            {
                ArgumentNullException.ThrowIfNull(Key);

                if (ActualDictionary.TryGetValue(Key, out TValue? Value))
                {
                    ActualDictionary.Remove(Key);

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        new KeyValuePair<TKey, TValue>(Key, Value)
                    ));
                    FullOnPropertyChanged();

                    return true;
                }
                return false;
            }

            public bool ContainsKey(TKey Key) => ActualDictionary.ContainsKey(Key);
            public bool TryGetValue(TKey Key, out TValue Value) => ActualDictionary.TryGetValue(Key, out Value!);
            public void Clear()
            {
                if (ActualDictionary.Count > 0)
                {
                    ActualDictionary.Clear();

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Reset
                    ));
                    FullOnPropertyChanged();
                }
            }








            #region ICollection<KeyValuePair<TKey, TValue>> interface (From IDictionary<TKey, TValue> interface)
            public int Count => ActualDictionary.Count;
            public bool IsReadOnly => false;

            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> Item)
                => Add(Item.Key, Item.Value);

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> Item)
                => ActualDictionary.Contains(Item);

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] Array, int ArrayIndex)
                => ((ICollection<KeyValuePair<TKey, TValue>>)ActualDictionary).CopyTo(Array, ArrayIndex);

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
                => Remove(item.Key);

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ActualDictionary.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ActualDictionary.GetEnumerator();
            #endregion

            #endregion







            #region IDictionary interface (Without generic types)
            bool IDictionary.IsFixedSize => false;
            bool IDictionary.IsReadOnly => false;
            ICollection IDictionary.Keys => ActualDictionary.Keys;
            ICollection IDictionary.Values => ActualDictionary.Values;

            private void AssertIDictionaryTypes(object Key, object? Value)
            {
                ArgumentNullException.ThrowIfNull(Key);

                if (Key is not TKey)
                {
                    throw new ArgumentException($"Key must be of type {typeof(TKey)}");
                }

                if (Value is not TValue | Value is null && default(TValue) is not null)
                {
                    throw new ArgumentException($"Value must be of type {typeof(TValue)}");
                }
            }
            object? IDictionary.this[object Key]
            {
                get => Key is TKey TargetTypeKey && ActualDictionary.TryGetValue(TargetTypeKey, out TValue? Value) ? Value : null;
                set
                {
                    AssertIDictionaryTypes(Key, value);
                    this[(TKey)Key] = (TValue)value!;
                }
            }

            void IDictionary.Add(object Key, object? Value)
            {
                AssertIDictionaryTypes(Key, Value);
                Add((TKey)Key, (TValue)Value!);
            }

            bool IDictionary.Contains(object Key) => Key is TKey TargetTypeValue && ContainsKey(TargetTypeValue);
            void IDictionary.Remove(object Key) { if (Key is TKey TargetTypeValue) Remove(TargetTypeValue); }

            IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(ActualDictionary.GetEnumerator());

            private class DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> Enumerator) : IDictionaryEnumerator
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> InternalEnumerator = Enumerator;

                public DictionaryEntry Entry => new(InternalEnumerator.Current.Key, InternalEnumerator.Current.Value);
                public object Key => InternalEnumerator.Current.Key;
                public object? Value => InternalEnumerator.Current.Value;
                public object Current => Entry;
                public bool MoveNext() => InternalEnumerator.MoveNext();
                public void Reset() => InternalEnumerator.Reset();
            }




            #region ICollection interface (From IDictionary interface (Without generic types))
            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => ((ICollection)ActualDictionary).SyncRoot;
            void ICollection.CopyTo(Array Array, int Index) => ((ICollection)ActualDictionary).CopyTo(Array, Index);
            #endregion

            #endregion
        }
    }
}