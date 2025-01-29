using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Client22.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StyleController(Mode mode) : ControllerBase
    {
        [HttpGet]
        public String Get()
        {
            return mode.style.Name;
        }
        [HttpPost("{style}")]
        public IResult Change(string style)
        {
            mode.style = style.ToLower() switch
            {
                "hold" => new Hold(),
                "attack" => new Attack(),
                _ => new Default()
            };
            return Results.Ok();
        }
    }
    
}
