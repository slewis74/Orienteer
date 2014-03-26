using Microsoft.Phone.Controls;

namespace Slab.WinPhone
{
    public interface IPhoneApplicationFrameAdapter
    {
        PhoneApplicationFrame PhoneApplicationFrame { get; set; }
    }
}