using IPRehabModel;
using IPRehabRepository.Contracts;
using PatientModel;

namespace IPRehabRepository
{
   public class AnswerRepository : RepositoryBase<tblAnswer>, IAnswerRepository
   {
      public AnswerRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class CodeSetRepository : RepositoryBase<tblCodeSet>, ICodeSetRepository
   {
      public CodeSetRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class EpisodeOfCareRepository : RepositoryBase<tblEpisodeOfCare>, IEpisodeOfCareRepository
   {
      public EpisodeOfCareRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class PatientRepository : RepositoryBase<tblPatient>, IPatientRepository
   {
      public PatientRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   /// <summary>
   /// This repository is derived from the FSOD sechema in VHAAUSBI25.vha.med.va.gov.DMHealthFactors database
   /// </summary>
   public class FSODPatientRepository : FSODRepositoryBase<FSODPatientDetailFY21Q2>, IFSODPatientRepository
   {
      public FSODPatientRepository(DmhealthfactorsContext repositoryContext)
            : base(repositoryContext)
      {
      }
   }

   public class QuestionRepository : RepositoryBase<tblQuestion>, IQuestionRepository
   {
      public QuestionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class QuestionInstructionRepository : RepositoryBase<tblQuestionInstruction>, IQuestionInstructionRepository
   {
      public QuestionInstructionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class SignatureRepository : RepositoryBase<tblSignature>, ISignatureRepository
   {
      public SignatureRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class UserRepository : RepositoryBase<tblUser>, IUserRepository
   {
      public UserRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class QuestionStageRepository : RepositoryBase<tblQuestionStage>, IQuestionStageRepository
   {
      public QuestionStageRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }
}
