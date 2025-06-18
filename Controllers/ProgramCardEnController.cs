using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models.Entities;
using MyUni.Models;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramCardEnController : ControllerBase

    {
        private readonly ApplicationDbContext dbContext;
        public ProgramCardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

        }
    }

}
