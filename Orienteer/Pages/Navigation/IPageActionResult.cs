using System;

namespace Orienteer.Pages.Navigation
{
    public interface IPageActionResult
    {
        Type PageType { get; set; }
        object Parameter { get; set; }
    }
}