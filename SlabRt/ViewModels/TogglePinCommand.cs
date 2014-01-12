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
        
        public abstract Uri TileMediumImageUri { get; }
        public virtual Uri TileSmallImageUri { get { return null; } }
        public virtual Uri TileWideImageUri { get { return null; } }
        public virtual Uri TileLargeImageUri { get { return null; } }

        public virtual Uri LockScreenBadgeLogoUri { get { return null; } }
        public virtual bool LockScreenDisplayBadgeAndTileText { get { return false; } }
        
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
                    TileMediumImageUri,
                    TileSize.Default);
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                if (TileSmallImageUri != null)
                    secondaryTile.SmallLogo = TileSmallImageUri;
                if (TileWideImageUri != null)
                {
                    secondaryTile.WideLogo = TileWideImageUri;
                    secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
                }
                if (TileLargeImageUri != null)
                {
                    // You'll get an exception from Windows if you specify a large logo without also specifying a wide one.
                    if (TileWideImageUri == null)
                        throw new InvalidOperationException("To support a large tile you must also support a wide tile");
                    secondaryTile.VisualElements.Square310x310Logo = TileLargeImageUri;
                    secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
                }
                
                if (LockScreenBadgeLogoUri != null)
                {
                    secondaryTile.LockScreenBadgeLogo = LockScreenBadgeLogoUri;
                    secondaryTile.LockScreenDisplayBadgeAndTileText = LockScreenDisplayBadgeAndTileText;
                }

                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(parameter.GetElementRect(), Windows.UI.Popups.Placement.Above);

                NotifyChanged(() => IsAlreadyPinned);
                NotifyChanged(() => IsNotAlreadyPinned);
            }
            appBar.IsSticky = false;
        }
    }
}