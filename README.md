
#Concepts

##MVC and MVVM-C

##Controller Actions

##Getting data

##Navigation Stack persistence

#Sample app
Provided is a sample application implemented in Xamarin Forms (there is also a sample for Windows Phone based on the same app).  The app displays a catalogue of Artists, Albums and Tracks.  It isn't supposed to be a fully functioning music app, it's simply using a domain that should be reasonably familiar to illustrate some navigation techniques.

##Nuget Packages
The samples use 
- Autofac
- Slab
	- PresentationBus
	- [Xamarin Forms 1.4]
- Slab.Autofac

##Application class
In the Xamarin Forms app the App class has been updated as follows.

        private IContainer _container;

        /// <summary>
        /// Constructs the application, including IoC container setup.
        /// </summary>
        /// <param name="callingAssembly">The calling assembly, which will be scanned for Modules for the IoC configuration.</param>
        public App(Assembly callingAssembly)
        {
            LetThereBeIoC(callingAssembly);

            // The root page of your application
            var slabNavigationPage = _container.Resolve<SlabNavigationPage>();
            MainPage = slabNavigationPage;

            slabNavigationPage.DoStartup();
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[] { this.GetType().GetTypeInfo().Assembly, callingAssembly });

            _container = builder.Build();
        }

This code scans the PCL DLL and the native app dll (callingAssembly) for Autofac Modules to register, so platform specific registrations are added to the Modules in the native projects.

The sample is also using simple page style navigation so it also assigns the SlabNavigationPage to be the MainPage.  This page derives from NavigationPage, but includes some monitoring of the navigation and controls storage of the navigation stack. 

##IoC and Autofac
Default modules
The forms sample contains a typical Autofac Module configuration, I'm not going to explain each one in line by line detail, but let's look at some of the key components.

###Controller Module
These registrations are for the components required to locate, create and invoke the controller actions.

###PresenationBusModule and PresentationBusSubscriptionModule
The first of these two modules registers the PresentationBus itself, the second hooks into the container activation events and subscribes any object that implements IHandlePresentationEvents to the bus as it gets activated.

###ViewModel Module
Registers the ViewModels and Commands.

###Navigation Module
Registers the navigator and the components required to locate and create Views.

The NavigationStack registration consists of a couple of parameters that are important to the functioning of the application.

- **defaultRoute**: is the REST style route to the default controller action.
- **alwaysStartFromDefaultRoute**: controls whether the application always starts from the defaultRoute or whether it remembers the navigation stack and restores it on restart.  It is important when setting this to false to ensure there is a **NavigationStackStorage** component registered (which will be a platform specific class and registration) and to call **PropertiesAutowired** on the NavigationStack registration, so the Storage instance gets assigned.

##ViewLocator
To configure the provided ViewLocator, you must provide a base namespace for the Views and ViewModels.  The locator assumes a parallel heirarchy exists for the Views and ViewModels.  In the sample the Views and ViewModels are in the same folder, so the same values are used for both base namespaces, however you don't have to do it this way if you prefer to have separate folder structures.






#Windows Phone
Navigation Frame

#Windows Store
NavigationFrame


#Looking ahead
Master/Detail