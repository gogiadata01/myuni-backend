using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUni.Data;
using MyUni.Models.Entities;
using Newtonsoft.Json;
using MyUni.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MyUni.Models.Entities.EventCard;

namespace MyUni.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Changed to include 'api' in the route
    public class EventCardEnController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public EventCardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

    
    }
    
}
