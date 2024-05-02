using IPRehabWebAPI2.Models;
using PatientModel_TreatingSpecialty;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
    public interface IUserPatientCacheHelper_TreatingSpecialty
    {
        Task<List<PatientDTOTreatingSpecialty>> GetPatients(List<MastUserDTO> distinctUserFacilities, string criteria, string orderBy, int pageNumber, int PageSize, string patientID);
        Task<PatientDTOTreatingSpecialty> GetPatientByEpisode(int episodeID);
        //Task<List<MastUserDTO>> GetUserAccessLevels(string networkID);
        Task<List<RptRehabDetails>> GetAllFacilityPatients();
        Task<List<RptRehabDetails>> GetThisFacilityPatients(List<MastUserDTO> distinctUserFacilities);
    }
}