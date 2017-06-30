using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Repositories;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        private ICarService _service;

        public CarController(ICarService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] QueryRequest query)
        {
            var results = _service.GetCars(query);

            if (results.Count > 0 && results != null)
                return Ok(results);
            else
                return NoContent();
        }
    }
}
