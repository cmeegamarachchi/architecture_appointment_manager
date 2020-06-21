using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Domain.Appointments;
using AppointmentsManager.Storage.Appointments;
using NUnit.Framework;

namespace AppointmentsManager.Application.Tests.Appointments
{
    [TestFixture]
    public class UpdateAppointmentCommandTest
    {
        [Test]
        [Category("Integration")]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public async Task Creates_appointment()
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
            
            // and given an updated appointment
            appointment.Place = "Colombo";
            
            // when UpdateAppointmentCommand is executed
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            var command = new AppointmentsManager.Application.Appointments.UpdateAppointmentCommand.Command();
            command.Id = appointment.Id;
            command.Place = "Colombo";
            command.Comments = appointment.Comments;
            command.Description = appointment.Description;
            command.DateTime = appointment.DateTime;
            var commandHandler = new AppointmentsManager.Application.Appointments.UpdateAppointmentCommand.Handler(appointmentStorage);
            var result = await commandHandler.Handle(command, CancellationToken.None);            
            
            // then appointment is updated
            var updatedAppointment = await appointmentStorage.GetAppointment(appointment.Id);
            Assert.IsTrue(updatedAppointment.Place == "Colombo");
            
            // then appointment-id is returned
            Assert.IsNotNull(result == appointment.Id);
        }
    }
}