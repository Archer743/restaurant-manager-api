namespace Restaurant_API.Data.Models.Other
{
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];

        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}
