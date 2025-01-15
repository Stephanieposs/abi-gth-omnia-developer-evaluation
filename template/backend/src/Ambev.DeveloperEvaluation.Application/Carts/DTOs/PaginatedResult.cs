using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.DTOs
{
    public class PaginatedResult<T> //: ApiResponseWithData<IEnumerable<T>>
    {
        
            public IEnumerable<T> Data { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }

            public PaginatedResult(IEnumerable<T> data, int totalCount, int currentPage, int pageSize)
            {
                Data = data;
                TotalCount = totalCount;
                CurrentPage = currentPage;
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            }
        
    }
}
