namespace Application.Services
{
    public class SearchService
    {
        private string _searchTerm = string.Empty;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnSearchChanged?.Invoke();
            }
        }

        public event Action? OnSearchChanged;
    }
}