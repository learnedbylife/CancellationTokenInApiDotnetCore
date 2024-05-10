using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CancellationTokenInApiDotnetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CancellationTokenController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CancellationTokenController> _logger;

        public CancellationTokenController(ILogger<CancellationTokenController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "LongRunningProcess")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var randomId = Guid.NewGuid();
                var results = new List<string>();

                for (int i = 0; i < 100; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Misson aborted");
                        return StatusCode(499);
                    }
                    await Task.Delay(1000, cancellationToken);
                    var result = $"Missile Fired: {randomId} - Distance from launchpad:{i} KM";
                    Console.WriteLine(result);
                    results.Add(result);
                }

                return Ok(results);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Misson aborted by user, {ex}");
                throw;
            }
            
        }
        
    }
}
