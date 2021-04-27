using IPRehabModel;
using IPRehabRepository.Contracts;

namespace IPRehabRepository
{
   class AnswerRepository : RepositoryBase<TblAnswer>, IAnswerRepository
   {
      public AnswerRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class CodeSetRepository : RepositoryBase<TblCodeSet>, ICodeSetRepository
   {
      public CodeSetRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class EpisodeOfCareRepository : RepositoryBase<TblEpisodeOfCare>, IEpisodeOfCareRepository
   {
      public EpisodeOfCareRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class PatientRepository : RepositoryBase<TblPatient>, IPatientRepository
   {
      public PatientRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class QuestionRepository : RepositoryBase<TblQuestion>, IQuestionRepository
   {
      public QuestionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class QuestionInstructionRepository : RepositoryBase<TblQuestionInstruction>, IQuestionInstructionRepository
   {
      public QuestionInstructionRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class SignatureRepository : RepositoryBase<TblSignature>, ISignatureRepository
   {
      public SignatureRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }

   class UserRepository : RepositoryBase<TblUser>, IUserRepository
   {
      public UserRepository(IPRehabContext repositoryContext)
          : base(repositoryContext)
      {
      }
   }
}
