#Orienteer
Orienteer is an MVVM and Navigation framework for mobile applications built using XAML.

##Concepts

###Navigation
The larger part of what Orienteer is about is filling the navigation void in MVVM.  The model is based on the same principles as the Magellan navigation framework for WPF, but with some adjustments to suit the mobile environment.

###Controller Actions
Like Magellan, navigation is based on MVC and the use of Controller Actions.  The controller actions return ViewModelActionResults, i.e Orienteer uses the ViewModel first approach to describing the desired target.

Here are some example actions

```csharp
    public async Task<ActionResult> ShowAll()
    {
        var artists = await _musicProvider.GetArtists();
        return new ViewModelActionResult(() => _artistsViewModelFactory(artists));
    }

    public async Task<ActionResult> ShowArtist(string name)
    {
        var artists = await _musicProvider.GetArtists();
        var artist = artists.SingleOrDefault(a => a.Name == name);

        if (artist == null)
            return await ShowAll();

        if (artist.Albums.Count == 1)
            return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single()));
        
		return new ViewModelActionResult(() => _artistViewModelFactory(artist));
    } 
```

So the action could be as simple as ShowAll, where you just need to retrieve data and hand it to a ViewModel, or it can contain more complicated routing logic like ShowArtist, where we're making decisions based on the data that was retrieved.  This could of course also include decisions based on who the user is, what permissions they have (for Line of Business style apps) or any other requirements you may have. 

###Getting data
As you may have noticed from the above code snippets, my preference is always towards retrieving the data and handing it to the ViewModel, as a dependency.  There are still scenarios where the ViewModel may need to subsequently load data, e.g. based on a user action.  

For this, the Navigator provided by Orienteer also includes methods to retrieve data.  These can be used by the ViewModels to abstract local or remote data retrieval, e.g they might wrap a REST API call or read some cached data from the device.

A controller action used for retrieving data returns a DataActionResult, for example

```csharp
	public ActionResult SearchForSuggestions(string searchText)
	{
	    return new DataActionResult<SearchResult[]>(GetSearchResults(searchText));
	}
```
And to call that controller action, the GetData call would be

```csharp
    var result = navigator.GetData<SearchController, SearchResult[]>(c => c.SearchForSuggestions(queryText));
```

###Navigation Stack persistence
Internally Orienteer uses a string to represent the calls to the controller actions.  These strings look much like the URLs you'd use to navigate to a controller action in ASP.NET MVC.

This has 2 key purposes, firstly the stack of strings can be stored and used to re-instate the app upon a restart.  Secondly, the string can be used as a deep-link into the application, e.g. for creating secondary tiles in Windows Store and Phone apps.

###MVVM and Async
MVVM is a critical part of using XAML, and Orienteer has a few components for making life easier.

Something that often causes trouble in client applications is triggering Property or Collection changed events from a background tread, e.g you've finished loading some data and need to update the UI.  To assist with this scenario Orienteer include BindableBase and DispatchingObservableCollection, both of which marshal the events back to the thread on which the object was created, because that should always be the UI thread.

DispatchingObservableCollection also has batching behaviour, so you can do **Large Updates** without causing the bound list to redraw after every individual Add. 

##Sample app
Provided is a sample application implemented in Xamarin Forms (there is also a sample for Windows Phone based on the same app).  The app displays a catalogue of Artists, Albums and Tracks.  It isn't supposed to be a fully functioning music app, it's simply using a domain that should be reasonably familiar to illustrate some navigation techniques.

###Nuget Packages
The samples use 
- Autofac
- Orienteer
	- PresentationBus
	- [Xamarin Forms 1.4]
- Orienteer.Autofac

###Application class
####Xamarin Forms
In the Xamarin Forms app the App class has been updated as follows.

```csharp
        private IContainer _container;

        /// <summary>
        /// Constructs the application, including IoC container setup.
        /// </summary>
        /// <param name="callingAssembly">The calling assembly, which will be scanned for Modules for the IoC configuration.</param>
        public App(Assembly callingAssembly)
        {
            LetThereBeIoC(callingAssembly);

            // The root page of your application
            var navigationPage = _container.Resolve<OrienteerNavigationPage>();
            MainPage = navigationPage;

            navigationPage.DoStartup();
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[] { this.GetType().GetTypeInfo().Assembly, callingAssembly });

            _container = builder.Build();
        }
```

This code scans the PCL DLL and the native app dll (callingAssembly) for Autofac Modules to register, so platform specific registrations are added to the Modules in the native projects.

The sample is also using simple page style navigation so it also assigns the OrienteerNavigationPage to be the MainPage.  This page derives from NavigationPage, but includes some monitoring of the navigation and controls storage of the navigation stack.

####Windows Phone
In the Windows Phone app, InitializePhoneApplication method has been modified as follows

```csharp
        private IContainer _container;

        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            LetThereBeIoC(typeof(App).Assembly);

            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            RootFrame.Navigated += CheckForResetNavigation;

            InitFrameAdapter();

            DoRescan();

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        private async Task InitFrameAdapter()
        {
            var adapter = _container.Resolve<IPhoneApplicationFrameAdapter>();
            adapter.PhoneApplicationFrame = RootFrame;
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[]
            {
                callingAssembly
            });

            _container = builder.Build();
        } 
```

The IoC setup here is very similar to the Xamarin Forms sample.  The main difference is the way we hook into the underlying navigation provided by Windows Phone.  Here we use an adapter that connects to the standard PhoneApplicationFrame and then monitors/controls the navigation.  See my [blog post](http://blog.shannonlewis.me/2015/07/orienteer-winphone/) for other config details.

###IoC and Autofac
Default modules
The forms sample contains a typical Autofac Module configuration, I'm not going to explain each one in line by line detail, but let's look at some of the key components.

####Controller Module
These registrations are for the components required to locate, create and invoke the controller actions.

####PresenationBusModule and PresentationBusSubscriptionModule
The first of these two modules registers the PresentationBus itself, the second hooks into the container activation events and subscribes any object that implements IHandlePresentationEvents to the bus as it gets activated.

####ViewModel Module
Registers the ViewModels and Commands.

####Navigation Module
Registers the navigator and the components required to locate and create Views.

The NavigationStack registration consists of a couple of parameters that are important to the functioning of the application.

- **defaultRoute**: is the REST style route to the default controller action.
- **alwaysStartFromDefaultRoute**: controls whether the application always starts from the defaultRoute or whether it remembers the navigation stack and restores it on restart.  It is important when setting this to false to ensure there is a **NavigationStackStorage** component registered (which will be a platform specific class and registration) and to call **PropertiesAutowired** on the NavigationStack registration, so the Storage instance gets assigned.

All of the samples are setup to restore the navigation stack, here's an example Autofac module registration

```csharp
	    builder
	        .RegisterType<NavigationStack>()
	        .As<INavigationStack>()
	        .WithParameter("defaultRoute", DefaultRoute)
	        .WithParameter("alwaysStartFromDefaultRoute", false)
	        .InstancePerLifetimeScope()
	        .PropertiesAutowired();
```

###ViewLocator
To configure the provided ViewLocator, you must provide a base namespace for the Views and ViewModels.  The locator assumes a parallel heirarchy exists for the Views and ViewModels.  In the sample the Views and ViewModels are in the same folder, so the same values are used for both base namespaces, however you don't have to do it this way if you prefer to have separate folder structures.

###Pages and ViewModels
From here you can start adding the Views (Pages) and VieWModels for your application.