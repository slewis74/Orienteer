using Windows.ApplicationModel.DataTransfer;

namespace SlabRt.Pages
{
    public interface IShare
    {
        bool GetShareContent(DataRequest dataRequest);
    }
}