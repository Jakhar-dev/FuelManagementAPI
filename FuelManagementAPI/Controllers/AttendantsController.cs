using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuelManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendantsController : ControllerBase
    {
        private readonly IAttendantRepository _attendantRepository;

        public AttendantsController(IAttendantRepository attendantRepository)
        {
            _attendantRepository = attendantRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendant>>> GetAttendants()
        {
            return Ok(await _attendantRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Attendant>> GetAttendant(int id)
        {
            var attendant = await _attendantRepository.GetByIdAsync(id);
            if (attendant == null) return NotFound();
            return Ok(attendant);
        }

        [HttpPost("add-attendant")]
        public async Task<IActionResult> AddAttendant([FromBody] AttendantViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendant = new Attendant
            {
                AttendantName = model.AttendantName,
                Phone = model.Phone,
                Address = model.Address,
                JoiningDate = model.JoiningDate,
                TerminationDate = model.TerminationDate,
                IsActive = model.IsActive,
                Photo = model.Photo,
                Description = model.Description
            };

            var addedAttendant = await _attendantRepository.AddAsync(attendant);
            return Ok(addedAttendant);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAttendant(int id, Attendant attendant)
        {
            if (id != attendant.AttendantId) return BadRequest();
            await _attendantRepository.UpdateAsync(attendant);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAttendant(int id)
        {
            await _attendantRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
