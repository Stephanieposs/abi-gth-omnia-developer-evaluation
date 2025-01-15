namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class PaginatedResponse<T>  : ApiResponseWithData<IEnumerable<T>>
{
    
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    /*
     * public IEnumerable<T> Data { get; set; }
     * 
    public PaginatedResponse(IEnumerable<T> data, int totalCount, int currentPage, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    */
}