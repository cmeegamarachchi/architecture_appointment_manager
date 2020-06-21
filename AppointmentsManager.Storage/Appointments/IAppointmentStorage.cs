using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;

namespace AppointmentsManager.Storage.Appointments
{
   public interface IAppointmentStorage
   {
       Task<AppointmentSummary[]> GetAppointmentSummaries();
       Task<Appointment> GetAppointment(string id);
       Task Save(Appointment appointment);
       Task Delete(string id);
   }
}