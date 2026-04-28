namespace rest.Helpers
{
    public class SingleResult<T>
    {
        public T Data { get; set; }
        public List<Link> _links { get; set; } = [];

        public SingleResult(T data)
        {
            Data = data;
        }
    }
}
