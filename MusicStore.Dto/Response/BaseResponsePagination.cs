namespace MusicStore.Dto.Response;

public class BaseResponsePagination<T> : BaseResponse
{
    public ICollection<T>? Data { get; set; }
    public int TotalPages { get; set; }
}