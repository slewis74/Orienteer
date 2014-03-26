using Windows.ApplicationModel.DataTransfer;

namespace Slab.WinStore.Pages
{
    public interface IShare
    {
        bool GetShareContent(DataRequest dataRequest);
    }
}