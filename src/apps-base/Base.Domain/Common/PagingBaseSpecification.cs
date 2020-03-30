namespace Base.Domain.Common
{
    public abstract class PagingBaseSpecification<T> : BaseSpecification<T>, IPagingSpecification<T>
      where T : class
    {
        public int Page { get; protected set; }
        public int PageSize { get; protected set; }
        public string OrderBy { get; protected set; }
        public bool Desc { get; protected set; } = true;
    }
}
