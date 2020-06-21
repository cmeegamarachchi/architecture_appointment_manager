using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Storage.Appointments;
using FluentValidation;
using MediatR;

namespace AppointmentsManager.Application.Appointments
{
    public class DeleteAppointmentCommand
    {
        public class Command : IRequest
        {
            public string Id { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IAppointmentStorage _appointmentStorage1;

            public Handler(IAppointmentStorage _appointmentStorage)
            {
                _appointmentStorage1 = _appointmentStorage;
            }
            
            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                await _appointmentStorage1.Delete(command.Id);
                
                return Unit.Value;
            }
        }
    }
}