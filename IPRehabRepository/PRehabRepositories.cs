using IPRehabModel;
using IPRehabRepository.Contracts;

namespace IPRehabRepository
{
   public class AnswerRepository : RepositoryBase<TblAnswer>, IAnswerRepository
   {
      public AnswerRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class CodeSetRepository : RepositoryBase<TblCodeSet>, ICodeSetRepository
   {
      public CodeSetRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class EpisodeOfCareRepository : RepositoryBase<TblEpisodeOfCare>, IEpisodeOfCareRepository
   {
      public EpisodeOfCareRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class PatientRepository : RepositoryBase<TblPatient>, IPatientRepository
   {
      public PatientRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class QuestionRepository : RepositoryBase<TblQuestion>, IQuestionRepository
   {
      public QuestionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class QuestionInstructionRepository : RepositoryBase<TblQuestionInstruction>, IQuestionInstructionRepository
   {
      public QuestionInstructionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class SignatureRepository : RepositoryBase<TblSignature>, ISignatureRepository
   {
      public SignatureRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class UserRepository : RepositoryBase<TblUser>, IUserRepository
   {
      public UserRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   public class QuestionStageRepository : RepositoryBase<TblQuestionStage>, IQuestionStageRepository
   {
      public QuestionStageRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }
}
