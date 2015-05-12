using System;

namespace Orienteer.Requests
{
    public class ViewModelNavigationRequestEventArgs : EventArgs
    {
        public ViewModelNavigationRequestEventArgs(object viewModel, string target = null)
        {
            ViewModel = viewModel;
            Target = target;
        }

        public object ViewModel { get; set; }
        public string Target { get; set; }
    }
}