using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AppointmentsManager.Domain.Appointments;

namespace AppointmentsManager.Storage.Appointments
{
   [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
   public class FileSystemAppointmentStorage: IAppointmentStorage
   {
       private readonly string _storagePath;

       public string AppointmentCatalogueFileName { get; set; } = "appointment_catalogue.json";
      
       public FileSystemAppointmentStorage(string storagePath)
       {
           _storagePath = storagePath;
       }

       public async Task<AppointmentSummary[]> GetAppointmentSummaries()
       {
           var appointmentSummaryList = new AppointmentSummary[0];

           var fileName = $"{_storagePath}/{AppointmentCatalogueFileName}";

           if (File.Exists(fileName))
           {
               using (var catalogueFile = File.OpenText(fileName))
               {
                   JsonSerializer serializer = new JsonSerializer();
                   await new TaskFactory().StartNew(() =>
                   {
                       appointmentSummaryList =
                           (AppointmentSummary[]) serializer.Deserialize(catalogueFile, typeof(AppointmentSummary[]));
                   });
               }
           }

           return appointmentSummaryList;
       }

       public async Task<Appointment> GetAppointment(string id)
       {
           var fileName = $"{_storagePath}/{id}.json";

           Appointment appointment = null;
          
           if (File.Exists(fileName))
           {
               using (var file = File.OpenText(fileName))
               {
                   JsonSerializer serializer = new JsonSerializer();
                   await new TaskFactory().StartNew(() =>
                   {
                       appointment = (Appointment) serializer.Deserialize(file, typeof(Appointment));
                   });
               }
           }

           return appointment;
       }


       [SuppressMessage("ReSharper", "VariableHidesOuterVariable")]
       public async Task Save(Appointment appointment)
       {
           AppointmentSummary[] UpdateAppointmentCatalogue(AppointmentSummary[] list, Appointment source)
           {
               AppointmentSummary appointmentSummary;
               
               // if found, update otherwise add-new
               if (list.Any(i => i.Id == source.Id))
               {
                   appointmentSummary = list.First(i => i.Id == source.Id);
               }
               else
               {
                   appointmentSummary = new AppointmentSummary(); 
                   list = list.Append(appointmentSummary).ToArray();
               }

               appointmentSummary.Id = source.Id;
               appointmentSummary.Place = source.Place;
               appointmentSummary.DateTime = source.DateTime;
               appointmentSummary.Deleted = false;
              
               return list;
           }

           var fileName = $"{appointment.Id}.json";
          
           await using (var file = File.CreateText($"{_storagePath}\\{fileName}"))
           {
               JsonSerializer serializer = new JsonSerializer();
               await new TaskFactory().StartNew(() =>
               {
                   serializer.Serialize(file, appointment);
               });
           }

           var appointmentCatalogue = await GetAppointmentSummaries();
           appointmentCatalogue = UpdateAppointmentCatalogue(appointmentCatalogue, appointment);

           await Save(appointmentCatalogue);
       }

       [SuppressMessage("ReSharper", "VariableHidesOuterVariable")]
       public async Task Delete(string id)
       {
           AppointmentSummary[] DisableAppointment(AppointmentSummary[] list, string id)
           {
               var appointmentSummary = list.FirstOrDefault(i => i.Id == id);
               if (appointmentSummary != null) appointmentSummary.Deleted = true;
               return list;
           }
          
           var appointmentSummaries = await GetAppointmentSummaries();
           appointmentSummaries = DisableAppointment(appointmentSummaries, id);

           await Save(appointmentSummaries);

           var oldPath = $"{_storagePath}/{id}.json";

           if (File.Exists(oldPath))
           {
               var newPath = $"{_storagePath}/{id}.deleted.json";
              
               File.Move(oldPath, newPath);
           }
       }

       private async Task Save(AppointmentSummary[] appointmentSummaryList)
       {
           await using (var file = File.CreateText($"{_storagePath}\\{AppointmentCatalogueFileName}"))
           {
               JsonSerializer serializer = new JsonSerializer();
               await new TaskFactory().StartNew(() =>
               {
                   serializer.Serialize(file, appointmentSummaryList);
               });
           }
       }
   }
}