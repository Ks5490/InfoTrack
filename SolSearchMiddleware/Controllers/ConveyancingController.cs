using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SolSearch.Models;
using SolSearch.Services;

namespace SolSearch.Controllers
{

    [ApiController]
    [Route("api/v1/conveyancing")]
    public class ConveyancingController(IReportGeneratorService reportGenerator) : ControllerBase
    {
        [Route("getpotentialclients")]
        [HttpPost]
        public async Task<ActionResult<PotentialClient>> GetPotentialClients([FromBody]List<PossibleLocations> possibleLocations, bool enhanced = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var report = await reportGenerator.GenerateReport(possibleLocations, enhanced, cancellationToken);
                if (report == null)
                {
                    return NotFound("No Clients found with this search configuration");
                }
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
