using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using MediatR;

namespace AppointmentsManager.Application.Appointments
{
    public class GetAppointmentQuery
    {
        public class Query : IRequest<Appointment>
        {
            public Query(string id)
            {
                Id = id;
            }

            public string Id { get; }
        }

        public class Handler : IRequestHandler<Query, Appointment>
        {
            private readonly IAppointmentStorage _appointmentStorage;

            public Handler(IAppointmentStorage appointmentStorage)
            {
                _appointmentStorage = appointmentStorage;
            }
                
            public async Task<Appointment> Handle(Query request, CancellationToken cancellationToken)
            {
                Appointment result = null;
                    
                var appointmentSummary = (await _appointmentStorage.GetAppointmentSummaries())
                    .FirstOrDefault(i => i.Id == request.Id);

                if (appointmentSummary != null)
                {
                    result = await _appointmentStorage.GetAppointment(request.Id);
                }

                return result;
            }
        }
    }
}