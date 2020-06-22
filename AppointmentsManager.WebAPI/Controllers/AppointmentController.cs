using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AppointmentsManager.Application.Appointments;
using AppointmentsManager.Domain.Appointments;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppointmentsManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController: ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<AppointmentSummary[]>> Get()
        {
            var result =
                await _mediator.Send(
                    new GetAppointmentSummaryListQuery.Query());

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(CreateAppointmentCommand.Command command)
        {
            var result =
                await _mediator.Send(command);
            
            return Ok(result);
        }
        
        [HttpPut]
        public async Task<ActionResult<string>> Put(UpdateAppointmentCommand.Command command)
        {
            var result =
                await _mediator.Send(command);
            
            return Ok(result);
        }

        [HttpPatch]
        [Route("{id}")]
        [SuppressMessage("ReSharper", "InvertIf")]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public async Task<ActionResult<string>> Patch(
            [FromBody] JsonPatchDocument<UpdateAppointmentCommand.Command> commandPatch, string id)
        {
            var appointment = await _mediator.Send(new GetAppointmentQuery.Query {Id = id});

            if (appointment != null)
            {
                var command = new UpdateAppointmentCommand.Command();
                command.Id = id;
                command.Comments = appointment.Comments;
                command.Place = appointment.Place;
                command.Description = appointment.Description;
                command.DateTime = appointment.DateTime;
                
                commandPatch.ApplyTo(command,ModelState);

                if (ModelState.IsValid)
                {
                    await _mediator.Send(command);
                }
                else
                {
                    var serializableModelState = new SerializableError(ModelState);
                    var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                    
                    return BadRequest(modelStateJson);
                }
                
                return Ok(id);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var command = new DeleteAppointmentCommand.Command { Id = id } ;
            await _mediator.Send(command);

            return NoContent();
        }
    }
}