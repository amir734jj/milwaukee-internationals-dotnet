using System.Collections;
using System.Threading.Tasks;
using API.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Abstracts
{
    public abstract class BasicCrudController<T> : Controller
    {
        [NonAction]
        public abstract IBasicCrudLogic<T> BasicCrudLogic();

        [HttpGet]
        [Route("")]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(IEnumerable), 200)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(BasicCrudLogic().GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Get")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(BasicCrudLogic().Get(id));
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation("Update")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] T instance)
        {
            return Ok(BasicCrudLogic().Update(id, instance));
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation("Delete")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return Ok(BasicCrudLogic().Delete(id));
        }
        
        [HttpPost]
        [Route("")]
        [SwaggerOperation("Save")]
        public async Task<IActionResult> Save([FromBody] T instance)
        {
            return Ok(BasicCrudLogic().Save(instance));
        }
    }
}