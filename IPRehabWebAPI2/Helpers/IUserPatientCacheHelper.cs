using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
  public interface IUserPatientCacheHelper
  {
    Task<List<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string networkName, string criteria, string orderBy, int pageNumber, int PageSize, string patientID);
    Task<PatientDTO> GetPatientByEpisode(IEpisodeOfCareRepository _episodeRepository, IFSODPatientRepository _patientRepository, int EpisodeID);
    Task<List<MastUserDTO>> GetUserAccessLevels(string networkID);
  }
}