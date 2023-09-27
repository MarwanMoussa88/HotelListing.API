namespace HotelListing.API.Models.QueryParameters
{
    public class QueryParameters
    {
        private int _pageSize = 16;
        public int StartIndex { get; set; }

        public int PageNumber { get; set; }

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

    }
}
