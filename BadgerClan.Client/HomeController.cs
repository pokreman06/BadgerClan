using Microsoft.AspNetCore.Mvc;
namespace BadgerClan.Client;

[Controller]
[Route("Style")]
public class HomeController: Controller
{
    IFightStyle Style { get; set; }
    public HomeController(IFightStyle style)
    {
        this.Style = style;
    }

    [HttpGet]
    public string Get()
    {
        return Style.Name;
    }
}
