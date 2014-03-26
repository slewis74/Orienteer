using System.Threading;
using Slab.Pages.Navigation;
using Slab.Xaml.ViewModels;

namespace SlabRt.ViewModels
{
    public abstract class SearchViewModelBase<TResult> : CanRequestNavigationBase, ISearchViewModelBase
    {
        private readonly string _queryText;

        protected SearchViewModelBase(
            INavigator navigator,
            string queryText,
            TResult[] searchResults)
            : base(navigator)
        {
            _queryText = queryText;
            SearchResults = searchResults;
        }

        protected SearchViewModelBase(
            INavigator navigator, 
            string queryText,
            TResult[] searchResults,
            SynchronizationContext synchronizationContext) : base(navigator, synchronizationContext)
        {
            _queryText = queryText;
            SearchResults = searchResults;
        }

        public override string PageTitle
        {
            get { return string.Format("Search Results - \"{0}\"", _queryText); }
        }

        public TResult[] SearchResults { get; set; }
    }
}