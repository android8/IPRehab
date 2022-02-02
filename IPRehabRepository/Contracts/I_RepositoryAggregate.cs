namespace IPRehabRepository.Contracts
{
   public interface IRepositoryAggregate
   {
      IAnswerRepository AnswerRepository { get; }
      IEpisodeOfCareRepository EpisodeOfCareRepository { get; }
      IQuestionRepository QuestionRepository { get; }
      //IGrantFunding_Repository GrantFunding_Repository { get; }
      IUserRepository UserRepository { get; }
      IPatientRepository PatientRepository { get; }
      ICodeSetRepository CodeSetRepository { get; }
      IQuestionInstructionRepository QuestionInstructionRepository { get; }
      ISignatureRepository SignatureRepository { get; }
      IQuestionMeasureRepository QuestionMeasureRepository { get; }
      void Save();
   }
}
