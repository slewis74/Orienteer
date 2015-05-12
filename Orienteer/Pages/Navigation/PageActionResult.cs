using System;

namespace Orienteer.Pages.Navigation
{
    public class PageActionResult<T> : ActionResult, IPageActionResult
    {
        public PageActionResult(object parameter)
        {
            PageType = typeof(T);
            Parameter = parameter;
        }

        public Type PageType { get; set; }
        public object Parameter { get; set; }
    }
}