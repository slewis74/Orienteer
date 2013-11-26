using System;
using System.Reflection;
using System.Windows.Input;
using Slab.Data;

namespace Slab.ViewModels
{
	public abstract class Command : BindableBase, ICommand
	{
		public event EventHandler CanExecuteChanged;

		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		public abstract void Execute(object parameter);

		protected void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged == null)
				return;
			CanExecuteChanged(this, EventArgs.Empty);
		}
	}

	public abstract class Command<T> : Command
	{
	    public virtual bool CanExecute(T parameter)
		{
			return true;
		}

		public override bool CanExecute(object parameter)
		{
		    if (parameter == null && typeof(T).GetTypeInfo().IsClass == false)
		        return false;
			return CanExecute((T)parameter);
		}

		public override void Execute(object parameter)
		{
            if (CanExecute(parameter) == false)
                return;
            Execute((T)parameter);
		}

		public abstract void Execute(T parameter);
	}
}