﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Orienteer.Data
{
    /// <summary>
    /// ObservableCollection that automatically dispatches change events to the thread the collection was created on,
    /// which should be the UI thread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DispatchingObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressChangeEvents;

        public DispatchingObservableCollection()
        {
        }

        public DispatchingObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        public void StartLargeUpdate()
        {
            _suppressChangeEvents = true;
        }

        public void CompleteLargeUpdate()
        {
            _suppressChangeEvents = false;
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> items)
        {
            StartLargeUpdate();
            foreach (var item in items)
            {
                Add(item);
            }
            CompleteLargeUpdate();
        }

        public void Replace(IEnumerable<T> items)
        {
            StartLargeUpdate();
            Clear();
            foreach (var item in items)
            {
                Add(item);
            }
            CompleteLargeUpdate();
        }

        protected override async void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressChangeEvents)
                return;

            await DispatchCall(() => RaiseCollectionChanged(e));
        }

        private void RaiseCollectionChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_suppressChangeEvents)
                return;

            await DispatchCall(() => RaisePropertyChanged(e));
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }

        protected void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyChanged(new[] { propertyName });
        }

        protected void NotifyChanged<TPropertyType>(params Expression<Func<TPropertyType>>[] expressions)
        {
            var propertyNames = new List<string>();

            foreach (var expression in expressions)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null)
                {
                    var lambdaExpression = expression as LambdaExpression;
                    if (lambdaExpression != null)
                    {
                        memberExpression = lambdaExpression.Body as MemberExpression;
                    }
                }
                propertyNames.Add(memberExpression.Member.Name);
            }

            NotifyChanged(propertyNames.ToArray());
        }

        protected void NotifyChanged(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Any() == false)
                return;

            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        protected async Task DispatchCall(Action action)
        {
            await UIDispatcher.Execute(action);
        }
    }
}