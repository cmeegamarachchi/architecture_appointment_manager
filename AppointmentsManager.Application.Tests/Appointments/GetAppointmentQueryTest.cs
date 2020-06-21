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
    public class GetAppointmentQueryTest
    {
        [Test]
        [Category("Integration")]
        public async Task Returns_an_appointment()
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
            
            // when GetAppointmentQuery is executed
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            var query = new AppointmentsManager.Application.Appointments.GetAppointmentQuery.Query { Id = appointment.Id};
            var queryHandler = new AppointmentsManager.Application.Appointments.GetAppointmentQuery.Handler(appointmentStorage);
            var result = await queryHandler.Handle(query, CancellationToken.None);            
            
            // then appointment is returned
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id == appointment.Id);
            Assert.IsTrue(result.Place == appointment.Place);
            Assert.IsTrue(result.Comments == appointment.Comments);
            Assert.IsTrue(result.Description == appointment.Description);
        }

        [Test]
        [Category("Integration")]
        public async Task Returns_null_for_no_matching_appointment_id()
        {
            var tempPath = Path.GetTempPath();
            var catalogueFileName = $"{Guid.NewGuid()}_catalogue.json"; 
  
            // given no appointment
            
            // when GetAppointmentQuery is executed with an arbitrary id
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            var query = new AppointmentsManager.Application.Appointments.GetAppointmentQuery.Query { Id = "123" };
            var queryHandler = new AppointmentsManager.Application.Appointments.GetAppointmentQuery.Handler(appointmentStorage);
            var result = await queryHandler.Handle(query, CancellationToken.None);            
            
            // then null is returned
            Assert.IsNull(result);
        }
    }
}