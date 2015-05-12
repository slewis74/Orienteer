namespace Orienteer.Pages
{
    public interface IControllerLocator
    {
        /// <summary>
        /// Resolves an instance of the controller with the given name prefix.
        /// </summary>
        /// <remarks>
        /// A controller prefix of Customer will result in an instance of a CustomerController.
        /// </remarks>
        /// <param name="controllerPrefix">The controller's type name prefix.</param>
        /// <returns>An instance of the controller, or null if one could not be resolved.</returns>
        object Create(string controllerPrefix);
    }
}