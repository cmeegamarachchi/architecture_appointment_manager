using System;

namespace AppointmentsManager.Domain.Appointments
{
   public class AppointmentSummary
   {
       public string Id { get; set; }
       public string Place { get; set; }
       public DateTime DateTime { get; set; }
       public bool Deleted { get; set; }
   }
}