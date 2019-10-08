using CoolPhotosChatService.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CoolPhotosChatService.Web.Hubs
{
    [Authorize(AuthenticationSchemes = Constants.AUTH_SCHEME)]
    public class ChatHub: Hub
    {
        public async Task SendMessage()
        {
        }
    }
}
