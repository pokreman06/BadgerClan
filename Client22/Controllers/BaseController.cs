using BadgerClan.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Client22.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        int requestNum=0;
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
            requestNum++;
            _logger.LogInformation($"Handling request #{requestNum}");
            //try
            //{
            return mode.GetMoves(request);
            //}
            //catch
            //{
            //    return new(new());
            //}
        }
    }
}
