using IPRehabModel;
using PatientModel;

namespace IPRehabRepository.Contracts
{
    public interface IAnswerRepository : IRepositoryBase<tblAnswer>
    {
    }

    public interface IEpisodeOfCareRepository : IRepositoryBase<tblEpisodeOfCare>
    {
    }

    public interface IQuestionRepository : IRepositoryBase<tblQuestion>
    {
    }

    public interface IUserRepository : IRepositoryBase<tblUser>
    {
    }

    public interface IPatientRepository : IRepositoryBase<tblPatient>
    {
    }

    public interface ITreatingSpecialtyPatientRepository : IRepositoryBase<vTreatingSpecialtyRecent3Yrs>
    {
    }

    public interface ICodeSetRepository : IRepositoryBase<tblCodeSet>
    {
    }

    public interface IQuestionInstructionRepository : IRepositoryBase<tblQuestionInstruction>
    {
    }

    public interface ISignatureRepository : IRepositoryBase<tblSignature>
    {
    }

    public interface IQuestionMeasureRepository : IRepositoryBase<tblQuestionMeasure>
    {
    }
}

