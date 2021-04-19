using IPRehabModel;

namespace IPRehabRepository.Contracts
{
  public interface IAnswerRepository : IRepositoryBase<TblAnswer>
  {
  }

  public interface IEpisodeOfCareRepository : IRepositoryBase<TblEpisodeOfCare>
  {
  }

  public interface IQuestionRepository : IRepositoryBase<TblQuestion>
  {
  }

  public interface IUserRepository : IRepositoryBase<TblUser>
  {
  }

  public interface IPatientRepository : IRepositoryBase<TblPatient>
  {
  }

  public interface ICodeSetRepository : IRepositoryBase<TblCodeSet>
  {
  }

  public interface IQuestionInstructionRepository : IRepositoryBase<TblQuestionInstruction>
  {
  }

  public interface ISignatureRepository : IRepositoryBase<TblSignature>
  {
  }
}

