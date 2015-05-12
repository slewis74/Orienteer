using System;

namespace Orienteer.Pages.Navigation
{
    public class ViewModelActionResult : ActionResult, IViewModelActionResult
    {
        public ViewModelActionResult(Func<object> viewModelFactory)
        {
            ViewModelInstance = viewModelFactory();
        }

        public object ViewModelInstance { get; private set; }
    }
}