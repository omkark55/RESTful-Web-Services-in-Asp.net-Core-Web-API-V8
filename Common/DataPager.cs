namespace WebApi_Angular.Common
{
    public class DataPager
    {
        public long RecordCount { get; set; }
        public int PageNumber { get; set; }
        public bool IsFirstHit { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public bool IsAscending { get; set; }
        public int Skip => (PageNumber > 0 ? PageNumber - 1 : 0) * PageSize;    
        
        public string? SearchField { get; set; }
        public string? SearchValue { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FillterCondition { get; set; }


        public void Normalize(string defaultSortBy, int defaultPageSize)
        {
            PageNumber = PageNumber > 0 ? PageNumber : 1;
            PageSize = PageSize > 0 ? PageSize : defaultPageSize;
            SortBy = string.IsNullOrEmpty(SortBy) ? defaultSortBy : SortBy;
        }

    }
}
