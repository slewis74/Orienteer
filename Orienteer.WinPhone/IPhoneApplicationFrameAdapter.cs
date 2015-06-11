using System.Threading.Tasks;
using Microsoft.Phone.Controls;

namespace Orienteer.WinPhone
{
    public interface IPhoneApplicationFrameAdapter
    {
        PhoneApplicationFrame PhoneApplicationFrame { get; set; }

        Task DoStartup();
    }
}