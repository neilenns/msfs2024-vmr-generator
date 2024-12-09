// <copyright file="RangeObservableCollection.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements an observable collection that supports adding a range of objects without firing
    /// a change event for each one.
    /// </summary>
    /// <typeparam name="T">The type contained in the collection.</typeparam>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool suppressNotification = false;

        /// <summary>
        /// Adds a list of elements to the collection, suppressing change notifications
        /// until all items in the range are added.
        /// </summary>
        /// <param name="list">The list of items to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public void AddRange(IEnumerable<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);

            this.suppressNotification = true;

            foreach (T item in list)
            {
                this.Add(item);
            }

            this.suppressNotification = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc/>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
