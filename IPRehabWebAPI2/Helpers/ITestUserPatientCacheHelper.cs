using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
  //ToDo: to be deleted after test 
  public interface ITestUserPatientCacheHelper
  {
    Task<List<PatientDTO>> GetPatients(IFSODPatientRepository _patientRepository, string criteria, string orderBy, int pageNumber, int PageSize, string patientID);
    Task<PatientDTO> GetPatientByEpisode(IEpisodeOfCareRepository _episodeRepository, IFSODPatientRepository _patientRepository, int EpisodeID);
    Task<List<MastUserDTO>> GetUserAccessLevels(string networkID);
  }
}