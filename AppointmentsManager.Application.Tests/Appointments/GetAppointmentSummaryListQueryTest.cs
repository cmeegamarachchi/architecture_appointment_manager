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
    public class GetAppointmentSummaryListQueryTest
    {
        [Test]
        [Category("Integration")]
        public async Task Returns_list_of_appointment_summaries()
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
            
            // when GetAppointmentSummaryListQuery is called
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            var query = new AppointmentsManager.Application.Appointments.GetAppointmentSummaryListQuery.Query();
            var queryHandler = new AppointmentsManager.Application.Appointments.GetAppointmentSummaryListQuery.Handler(appointmentStorage);
            var appointmentSummaryList = await queryHandler.Handle(query, CancellationToken.None);

            // then list of AppointmentSummary entries are returned
            Assert.IsTrue(appointmentSummaryList.Length == 1);
            Assert.IsTrue(appointmentSummaryList[0].Place == "Alaska");
        }

        [Test]
        [Category("Integration")]
        public async Task Returns_empty_list_when_no_appointments_are_available()
        {
            // given there are no appointments
            var tempPath = Path.GetTempPath();
            var catalogueFileName = $"{Guid.NewGuid()}_catalogue.json"; 
            
            var appointmentStorage = new FileSystemAppointmentStorage(tempPath){ AppointmentCatalogueFileName = catalogueFileName };
            
            // when GetAppointmentSummaryListQuery is called
            var query = new AppointmentsManager.Application.Appointments.GetAppointmentSummaryListQuery.Query();
            var queryHandler = new AppointmentsManager.Application.Appointments.GetAppointmentSummaryListQuery.Handler(appointmentStorage);
            var appointmentSummaryList = await queryHandler.Handle(query, CancellationToken.None);
            
            // then an empty list is returned
            Assert.IsTrue(appointmentSummaryList.Length == 0);
            
        }
    }
}