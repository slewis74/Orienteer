namespace Orienteer.Pages.Navigation
{
    public class DataActionResult<TData> : ActionResult
    {
        public DataActionResult(TData data)
        {
            Data = data;
        }

        public TData Data { get; protected set; }
    }
}