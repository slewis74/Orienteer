using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Shell;
using Orienteer.Xaml.ViewModels;

namespace Orienteer.WinPhone.Commands
{
    public abstract class PinSecondaryTileCommand : Command<FrameworkElement>
    {
        public abstract string ActivationRoute { get; }

        public abstract string TileTitle { get; }

        protected string WideContent1 { get; set; }
        protected string WideContent2 { get; set; }
        protected string WideContent3 { get; set; }

        public bool IsAlreadyPinned { get { return ShellTile.ActiveTiles.Any(t => t.NavigationUri.ToString() == ActivationRoute); } }
        public bool IsNotAlreadyPinned { get { return !IsAlreadyPinned; } }

        public override bool CanExecute(FrameworkElement parameter)
        {
            return IsNotAlreadyPinned;
        }

        public override void Execute(FrameworkElement parameter)
        {
            var tileData = new IconicTileData
            {
                Title = TileTitle,
                IconImage = new Uri("/Assets/Tiles/IconicTileMediumLarge.png", UriKind.Relative),
                SmallIconImage = new Uri("/Assets/Tiles/IconicTileSmall.png", UriKind.Relative),
                WideContent1 = WideContent1,
                WideContent2 = WideContent2,
                WideContent3 = WideContent3
            };

            var tile = ShellTile.ActiveTiles.SingleOrDefault(t => t.NavigationUri.ToString() == ActivationRoute);
            if (tile == null)
            {

                ShellTile.Create(new Uri(ActivationRoute, UriKind.Relative), tileData, true);
            }
            else
            {
                tile.Update(tileData);
            }

            NotifyChanged(() => IsAlreadyPinned);
            NotifyChanged(() => IsNotAlreadyPinned);
            RaiseCanExecuteChanged();
        }
    }
}