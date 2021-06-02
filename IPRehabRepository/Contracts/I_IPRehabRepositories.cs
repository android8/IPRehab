using IPRehabModel;
using PatientModel;

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

   /// <summary>
   /// external patient data source from BI25.DMHealthFactors
   /// only needed for CRUD operation which would never happen other than select
   /// </summary>
   public interface IFSODPatientRepository : IRepositoryBase<FSODPatientDetailFY21Q2>
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

   public interface IQuestionStageRepository : IRepositoryBase<TblQuestionStage>
   {
   }
}

