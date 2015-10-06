using System;

namespace Orienteer.Messages
{
    public class PageNavigationRequestEventArgs : EventArgs
    {
        public PageNavigationRequestEventArgs(Type viewType, object parameter, string target = null)
        {
            ViewType = viewType;
            Parameter = parameter;
            Target = target;
        }

        public Type ViewType { get; private set; }
        public object Parameter { get; private set; }
        public string Target { get; set; }
    }
}