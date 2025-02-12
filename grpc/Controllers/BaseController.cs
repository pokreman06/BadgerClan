using BadgerClan.Logic;
using grpc;
using Microsoft.AspNetCore.Mvc;

namespace grpc.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        public BaseController(ILogger<BaseController> logger, Mode mode)
        {
            _logger = logger;
            this.mode = mode;
        }
        private readonly ILogger<BaseController> _logger;
        private Mode mode;

        [HttpPost]
        public MoveResponse Basic(MoveRequest request)
        {
            return mode.GetMoves(request);
        }
        [HttpGet]
        public string ForJohnnyBoy()
        {
            return "For Johnathan";
        }
    }
}
