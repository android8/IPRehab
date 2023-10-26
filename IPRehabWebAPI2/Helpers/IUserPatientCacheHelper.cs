using IPRehabModel;
using IPRehabWebAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
    public interface IUserPatientCacheHelper
    {
        Task<List<PatientDTOTreatingSpecialty>> GetPatients(List<MastUserDTO> distinctUserFacilities, string criteria, string orderBy, int pageNumber, int PageSize, string patientID);
        Task<PatientDTOTreatingSpecialty> GetPatientByEpisode(int episodeID);
        Task<List<MastUserDTO>> GetUserAccessLevels(string networkID);
        Task<List<vTreatingSpecialtyRecent3Yrs>> GetAllFacilityPatients(List<MastUserDTO> distinctUserFacilities);
        Task<List<vTreatingSpecialtyRecent3Yrs>> GetThisFacilityPatients(List<MastUserDTO> distinctUserFacilities);
    }
}