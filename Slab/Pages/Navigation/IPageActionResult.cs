using System;

namespace Slab.Pages.Navigation
{
    public interface IPageActionResult
    {
        Type PageType { get; set; }
        object Parameter { get; set; }
    }
}