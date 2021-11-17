using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalrServer.Hubs;

namespace SignalrServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        IHubContext<ChatHub> _hubContext;
        public MessagingController(IHubContext<ChatHub> hubcontext)
        {
            _hubContext = hubcontext;
        }

        [HttpPost]
        public void Post([FromBody] object value)
        {
            Debug.WriteLine("messaging: ");
            Debug.WriteLine(value);
            _hubContext.Clients.All.SendAsync("Posted", value.ToString());

        }
    }
}