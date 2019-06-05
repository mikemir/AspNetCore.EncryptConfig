using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EncryptConfig.WAppExample.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.EncryptConfig.WAppExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IOptions<Jwt> _jwtConfig;

        public ValuesController(IOptions<Jwt> jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var jwt = _jwtConfig.Value;
            return new string[] { jwt.ClientId, jwt.ClientSecret };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

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
