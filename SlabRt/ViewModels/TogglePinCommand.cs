using System;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Slab.ViewModels;

namespace SlabRt.ViewModels
{
    public abstract class TogglePinCommand : Command<FrameworkElement>
    {
        public abstract string AppbarTileId { get; }
        public abstract string TileTitle { get; }
        public abstract Uri TileImageUri { get; }
        public abstract string ActivationArguments { get; }

        public bool IsAlreadyPinned { get { return SecondaryTile.Exists(AppbarTileId); } }
        public bool IsNotAlreadyPinned { get { return !IsAlreadyPinned; } }

        public async override void Execute(FrameworkElement parameter)
        {
            var parent = (FrameworkElement)parameter.Parent;
            while ((parent is AppBar) == false)
            {
                parent = (FrameworkElement)parent.Parent;
            }
            var appBar = (AppBar)parent;

            appBar.IsSticky = true;

            if (IsAlreadyPinned)
            {
                var secondaryTile = new SecondaryTile(AppbarTileId);
                var isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(parameter.GetElementRect(), Windows.UI.Popups.Placement.Above);

                NotifyChanged(() => IsAlreadyPinned);
                NotifyChanged(() => IsNotAlreadyPinned);
            }
            else
            {
                var secondaryTile = new SecondaryTile(AppbarTileId,
                    TileTitle,
                    ActivationArguments,
                    TileImageUri,
                    TileSize.Default);

                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(parameter.GetElementRect(), Windows.UI.Popups.Placement.Above);

                NotifyChanged(() => IsAlreadyPinned);
                NotifyChanged(() => IsNotAlreadyPinned);
            }
            appBar.IsSticky = false;
        }
    }
}