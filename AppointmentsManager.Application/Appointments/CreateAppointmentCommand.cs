using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using FluentValidation;
using MediatR;

namespace AppointmentsManager.Application.Appointments
{
    public class CreateAppointmentCommand
    {
        public class Command : IRequest<string>
        {
            public string Id { get; set; }
            public string Place { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
            public DateTime? DateTime { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Place).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IAppointmentStorage _appointmentStorage;

            public Handler(IAppointmentStorage appointmentStorage)
            {
                _appointmentStorage = appointmentStorage;
            }
            
            [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var appointment = new Appointment();
                appointment.Id = command.Id ?? new Guid().ToString();
                appointment.Place = command.Place;
                appointment.Comments = command.Comments;
                appointment.Description = command.Description;
                appointment.DateTime = command.DateTime ?? DateTime.Now;

                await _appointmentStorage.Save(appointment);

                return appointment.Id;
            }
        }
    }
}