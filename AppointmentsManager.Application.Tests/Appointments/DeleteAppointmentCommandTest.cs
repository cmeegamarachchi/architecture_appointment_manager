using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using NUnit.Framework;

namespace AppointmentsManager.Application.Tests.Appointments
{
    [TestFixture]
    public class DeleteAppointmentCommandTest
    {
        [Test]
        public async Task Deletes_appointment()
        {
            var tempPath = Path.GetTempPath();
            var catalogueFileName = $"{Guid.NewGuid()}_catalogue.json"; 
  
            // given an appointment
            var appointment = new Appointment();
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Place = "Alaska";
            appointment.Comments = "this is a comment";
            appointment.Description = "this is a description";
            appointment.DateTime = new DateTime(2009, 11, 23);
  
            await new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName }.Save(appointment);
            
            // when DeleteAppointmentCommand is executed
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            var command = new AppointmentsManager.Application.Appointments.DeleteAppointmentCommand.Command();
            command.Id = appointment.Id;
            var commandHandler = new AppointmentsManager.Application.Appointments.DeleteAppointmentCommand.Handler(appointmentStorage);
            await commandHandler.Handle(command, CancellationToken.None);            
            
            // then appointment is deleted
            var deletedAppointment = await appointmentStorage.GetAppointment(appointment.Id);
            Assert.IsNull(deletedAppointment);
        }
    }
}