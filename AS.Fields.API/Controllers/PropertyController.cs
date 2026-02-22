using AS.Fields.Application.Services;
using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.DTO.Property;
using AS.Fields.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.Fields.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyController(IPropertyService propertyService, IFieldService fieldService) : ApiBaseController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var properties = await propertyService.GetAllPropertiesAsync(GetUserId());
            return Success(properties, "Lista de propriedades do usuário, retornada com sucesso.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var property = await propertyService.GetPropertyByIdAsync(GetUserId(), id);
            return Success(property, "Propriedade encontrada.");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePropertyDTO dto)
        {
            var property = await propertyService.CreatePropertyAsync(GetUserId(), dto);
            return CreatedResponse(property, "Propriedade criada com sucesso.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, SavePropertyDTO dto)
        {
            await propertyService.UpdatePropertyAsync(GetUserId(), id, dto);
            return Success("Propriedade atualizada com sucesso!");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await propertyService.DeletePropertyAsync(GetUserId(), id);
            return NoContent();
        }

        #region Fields
        [HttpGet("{propertyId}/Field")]
        public async Task<IActionResult> GetFields([FromRoute] Guid propertyId)
        {
            var fields = await fieldService.GetAllFieldsAsync(propertyId);
            return Success(fields, "Lista de talhões da propriedade, retornada com sucesso.");
        }

        [HttpGet("Field/{id}")]
        public async Task<IActionResult> GetFieldById([FromRoute] Guid id)
        {
            var field = await fieldService.GetFieldByIdAsync(id);
            return Success(field, "Talhão encontrado.");
        }

        [HttpPost("{propertyId}/Field")]
        public async Task<IActionResult> CreateField([FromRoute] Guid propertyId, [FromBody] CreateFieldDTO dto)
        {
            var field = await fieldService.CreateFieldAsync(propertyId, dto);
            return CreatedResponse(field, "Talhão criado com sucesso.");
        }


        [HttpPatch("Field/{id}")]
        public async Task<IActionResult> PartialFieldUpdate([FromRoute] Guid id, PartialUpdateFieldDTO dto)
        {
            await fieldService.PartialUpdateFieldAsync(id, dto);
            return Success("Talhão atualizado com sucesso!");
        }


        [HttpDelete("Field/{id}")]
        public async Task<IActionResult> DeleteField([FromRoute] Guid id)
        {
            await fieldService.DeleteFieldAsync(id);
            return NoContent();
        }
        #endregion
    }
}
