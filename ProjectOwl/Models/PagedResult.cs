using System;

namespace ProjectOwl.Models
{
    public class PagedResult<T>
    {
        public PagedResult(T data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }

        public T Data { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public int TotalRecords { get; set; }
        public string FirstPage => SetFirstPage;
        public int TotalPages => SetTotalPages;
        public string NextPage => SetNextPage;
        public string PreviousPage => SetPreviousPage;
        public string LastPage => SetLastPage;

        private int SetTotalPages => Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));
        private string SetNextPage => PageNumber >= 1 && PageNumber < TotalPages ? $"?pageNumber={PageNumber + 1}&pageSize={PageSize}" : null;
        private string SetPreviousPage => PageNumber - 1 >= 1 && PageNumber <= TotalPages ? $"?pageNumber={PageNumber - 1}&pageSize={PageSize}" : null;
        private string SetFirstPage => $"?pageNumber=1&pageSize={PageSize}";
        private string SetLastPage => $"?pageNumber={TotalPages}&pageSize={PageSize}";
    }
}
