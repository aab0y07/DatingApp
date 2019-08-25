using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        
        private readonly DataContext _context;
        public ValuesController (DataContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        /* public IActionResult GetValues() // IActionResult allows to return http responses to clients
        {
            var values = _context.Values.ToList();
            return Ok(values); // the result can be checked via postman 
        }*/
        
        // Async version of the method above
        public async Task<IActionResult> GetValues() // IActionResult allows to return http responses to clients
        {
            var values = await _context.Values.ToListAsync();
            return Ok(values); // the result can be checked via postman 
        }


        [AllowAnonymous ]
        [HttpGet("{id}")]
        /* public IActionResult GetValue(int id) // the difference betweeen this and previous one is that, this method returns specific value, not list of values
        {
            var value = _context.Values.FirstOrDefault(x => x.Id ==id);
            return Ok(value); // the result can be checked via postman 
        }*/

         public async Task<IActionResult> GetValue(int id) // the difference betweeen this and previous one is that, this method returns specific value, not list of values
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id ==id);
            return Ok(value); // the result can be checked via postman 
        }


        // Synchronous means that request comes to one of the methods and thread blocks it until to get a respond. 
        // But in most cases web server should handle with concurrent requests which means multithreading at the same time. In this scenario, good soöution is asynchronous code. 
        // IF we use asynchronous code, then thread is not blocked, kept open to handle requests and it passes off the action of going out to get data from the DB to a delegate 
        // and then when the result is returned it continues on with the request but it does not block any of our requests in that thread.  

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
