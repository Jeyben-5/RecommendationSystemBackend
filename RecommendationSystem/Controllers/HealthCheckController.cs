﻿using Microsoft.AspNetCore.Mvc;

namespace WaiterLogisticAdmin.Controllers
{
    [Route("api/health-check")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Service running.");
        }
    }
}