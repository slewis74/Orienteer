using Windows.ApplicationModel.DataTransfer;

namespace Orienteer.WinStore.Pages
{
    public interface IShare
    {
        bool GetShareContent(DataRequest dataRequest);
    }
}