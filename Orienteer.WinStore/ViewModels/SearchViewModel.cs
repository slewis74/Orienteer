using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;

namespace Orienteer.WinStore.ViewModels
{
    public abstract class SearchViewModel<TResult> : CanRequestNavigation, ISearchViewModelBase
    {
        private readonly string _queryText;

        protected SearchViewModel(
            INavigator navigator,
            string queryText,
            TResult[] searchResults)
            : base(navigator)
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