
namespace SDK.Models
{
    public class SDKResponse<T>
    {
        public List<T> Docs { get; set; }
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
    }
}
