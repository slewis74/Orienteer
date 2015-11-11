using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Orienteer.Data
{
    public abstract class Bindable : DispatchesToUIThread, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            NotifyChanged(new[] { propertyName });
            return true;
        }

        protected void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyChanged(new [] { propertyName });
        }

		protected void NotifyChanged<T>(params Expression<Func<T>>[] expressions)
		{
			if (PropertyChanged == null)
				return;

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

        protected async void NotifyChanged(params string[] propertyNames)
        {
            if (PropertyChanged == null || propertyNames == null || propertyNames.Any() == false)
                return;

            await DispatchCall(
                () =>
                {
                    foreach (var propertyName in propertyNames)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                });
        }

        protected virtual async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await DispatchCall(
                () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }
    }
}
