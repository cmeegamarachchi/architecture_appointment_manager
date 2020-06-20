using System;

namespace AppointmentsManager.Domain.Appointments
{
   public class Appointment
   {
       public string Id { get; set; }
       public string Place { get; set; }
       public string Description { get; set; }
       public string Comments { get; set; }
       public DateTime DateTime { get; set; }
   }
}