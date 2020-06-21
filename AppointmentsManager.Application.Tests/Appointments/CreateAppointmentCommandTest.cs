using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppointmentsManager.Storage.Appointments;
using NUnit.Framework;

namespace AppointmentsManager.Application.Tests.Appointments
{
    [TestFixture]
    public class CreateAppointmentCommandTest
    {
        [Test]
        [Category("Integration")]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public async Task Creates_appointment()
        {
            var tempPath = Path.GetTempPath();
            var catalogueFileName = $"{Guid.NewGuid()}_catalogue.json"; 
            
            // given an appointment
            var command = new AppointmentsManager.Application.Appointments.CreateAppointmentCommand.Command();
            command.Id = Guid.NewGuid().ToString();
            command.Place = "Alaska";
            command.Comments = "this is a comment";
            command.Description = "this is a description";
            command.DateTime = new DateTime(2009, 11, 23);

            // when CreateAppointmentCommand is executed
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath)
                {AppointmentCatalogueFileName = catalogueFileName};
            var commandHandler =
                new AppointmentsManager.Application.Appointments.CreateAppointmentCommand.Handler(appointmentStorage);
            var appointmentId = await commandHandler.Handle(command, CancellationToken.None);

            // then appointment is created
            var createdAppointment = await new FileSystemAppointmentStorage(tempPath)
                {AppointmentCatalogueFileName = catalogueFileName}.GetAppointment(command.Id);
            Assert.IsNotNull(createdAppointment);
            
            // and then id of the appointment is returned
            Assert.IsTrue(appointmentId == createdAppointment.Id);
            Assert.IsTrue(command.Id == createdAppointment.Id);
        }
    }
}