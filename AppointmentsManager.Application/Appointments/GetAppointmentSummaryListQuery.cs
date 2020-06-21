using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using MediatR;

namespace AppointmentsManager.Application.Appointments
{
    public class GetAppointmentSummaryListQuery
    {
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        public class Query: IRequest<AppointmentSummary[]> { }

        public class Handler : IRequestHandler<Query, AppointmentSummary[]>
        {
            private readonly IAppointmentStorage _appointmentStorage;

            public Handler(IAppointmentStorage appointmentStorage)
            {
                _appointmentStorage = appointmentStorage;
            }
            
            public async Task<AppointmentSummary[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _appointmentStorage.GetAppointmentSummaries();
            }
        }
    }
}