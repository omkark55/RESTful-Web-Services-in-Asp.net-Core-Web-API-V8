namespace WebApi_Angular.Models
{
    public class ResponseResult
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public dynamic Result { get; set; }
        public string Message { get; set; }

        //public DataPager Pager { get; set; }

    }
}
