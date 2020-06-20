using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using NUnit.Framework;

namespace AppointmentsManager.Storage.Tests.Appointments
{
    [TestFixture]
    public class FileSystemAppointmentStorageTests
    {
        [Test]
        public async Task Can_save_and_read_appointment()
        {
            var appointment = new Appointment();
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Place = "Alaska";
            appointment.Comments = "this is a comment";
            appointment.Description = "this is a description";
            appointment.DateTime = new DateTime(2009, 11, 23);

            var tempPath = Path.GetTempPath();
          
            await new FileSystemAppointmentStorage(tempPath).Save(appointment);
        
            var newAppointment = await new FileSystemAppointmentStorage(tempPath).GetAppointment(appointment.Id);
        
            Assert.IsTrue(appointment.Id == newAppointment.Id);
            Assert.IsTrue(appointment.Place == newAppointment.Place);
            Assert.IsTrue(appointment.Comments == newAppointment.Comments);
            Assert.IsTrue(appointment.Description == newAppointment.Description);
            Assert.IsTrue(appointment.DateTime == newAppointment.DateTime);
        }
        
        [Test]
        public async Task Adding_appointment_creates_appointment_summary()
        {
            var appointment = new Appointment();
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Place = "Alaska";
            appointment.Comments = "this is a comment";
            appointment.Description = "this is a description";
            appointment.DateTime = new DateTime(2009, 11, 23);

            var tempPath = Path.GetTempPath();
  
            await new FileSystemAppointmentStorage(tempPath).Save(appointment);

            var appointmentSummary = await new FileSystemAppointmentStorage(tempPath).GetAppointmentSummaries();
  
            Assert.IsTrue(appointmentSummary.Any(i => i.Id == appointment.Id));
        }
        
        [Test]
        public async Task Deleting_appointment_deletes_appointment()
        {
            var tempPath = Path.GetTempPath();
  
            // given an appointment
            var appointment = new Appointment();
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Place = "Alaska";
            appointment.Comments = "this is a comment";
            appointment.Description = "this is a description";
            appointment.DateTime = new DateTime(2009, 11, 23);
  
            await new FileSystemAppointmentStorage(tempPath).Save(appointment);
  
            //when appointment is deleted
            await new FileSystemAppointmentStorage(tempPath).Delete(appointment.Id);
  
            //then appointment can no longer be loaded
            var deletedAppointment = await new FileSystemAppointmentStorage(tempPath).GetAppointment(appointment.Id);
            Assert.IsNull(deletedAppointment);
  
            //and then appointment summary will have be marked as deleted
            var appointmentSummary = await new FileSystemAppointmentStorage(tempPath).GetAppointmentSummaries();
  
            Assert.IsTrue(appointmentSummary.First(i => i.Id == appointment.Id).Deleted);
        }
        
        [Test]
        public async Task Updating_appointment_updates_appointment()
        {
            var tempPath = Path.GetTempPath();
  
            // given an appointment
            var appointment = new Appointment();
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Place = "Alaska";
            appointment.Comments = "this is a comment";
            appointment.Description = "this is a description";
            appointment.DateTime = new DateTime(2009, 11, 23);
  
            await new FileSystemAppointmentStorage(tempPath).Save(appointment);
  
            //when the appointment is updated
            appointment.Place = "Colombo";
            appointment.Description = "description is updated";
            await new FileSystemAppointmentStorage(tempPath).Save(appointment);
  
            //then updated appointment can be loaded
            var updatedAppointment = await new FileSystemAppointmentStorage(tempPath).GetAppointment(appointment.Id);
            Assert.IsTrue(updatedAppointment.Place == "Colombo");
            Assert.IsTrue(updatedAppointment.Description == "description is updated");

            //and then updated appointment details are correctly propagated to catalogue
            var catalogue = await new FileSystemAppointmentStorage(tempPath).GetAppointmentSummaries();
            var appointmentSummary = catalogue.First(i => i.Id == appointment.Id);
            Assert.IsTrue(appointmentSummary.Place == "Colombo");
        }
    }
}