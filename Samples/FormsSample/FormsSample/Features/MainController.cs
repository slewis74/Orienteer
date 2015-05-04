﻿using System;
using FormsSample.Home;
using Slab.Pages;
using Slab.Pages.Navigation;

namespace FormsSample.Features
{
    public class MainController : Controller
    {
        private readonly Func<HomeViewModel> _homeViewModelFactory;

        public MainController(Func<HomeViewModel> homeViewModelFactory)
        {
            _homeViewModelFactory = homeViewModelFactory;
        }

        public ActionResult Home()
        {
            return new ViewModelActionResult(_homeViewModelFactory);
        }
         
    }
}