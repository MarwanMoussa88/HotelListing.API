namespace HotelListing.API.Models.PagedResult
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int RecordNumber { get; set; }

        public List<T> records { get; set; }
    }
}
