using IPRehabModel;
using IPRehabRepository.Contracts;
using PatientModel_TreatingSpecialty;
//using PatientModel_TreatingSpecialty;

namespace IPRehabRepository
{
    public class AnswerRepository : RepositoryBase_IPRehab<tblAnswer>, IAnswerRepository
    {
        public AnswerRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class CodeSetRepository : RepositoryBase_IPRehab<tblCodeSet>, ICodeSetRepository
    {
        public CodeSetRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class EpisodeOfCareRepository : RepositoryBase_IPRehab<tblEpisodeOfCare>, IEpisodeOfCareRepository
    {
        public EpisodeOfCareRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class PatientRepository : RepositoryBase_IPRehab<tblPatient>, IPatientRepository
    {
        public PatientRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class TreatingSpecialtyPatientRepository : RepositoryBase_IPRehab<vTreatingSpecialtyRecent3Yrs>, ITreatingSpecialtyPatientRepository
    {
        public TreatingSpecialtyPatientRepository(IPRehabContext repositoryContext)
              : base(repositoryContext)
        {
        }
    }

    public class TreatingSpecialtyDirectPatientRepository : RepositoryBase_TreatingSpecialty<RptRehabDetails>, ITreatingSpecialtyDirectPatientRepository
    {
        public TreatingSpecialtyDirectPatientRepository(DMTreatingSpecialtyContext repositoryContext)
              : base(repositoryContext)
        {
        }
    }

    public class TreatingSpecialtyPatientDemographicRepository : RepositoryBase_TreatingSpecialty<PatientDemographic>, ITreatingSpecialtyPatientDemographicRepository
    {
        public TreatingSpecialtyPatientDemographicRepository(DMTreatingSpecialtyContext repositoryContext)
              : base(repositoryContext)
        {
        }
    }

    public class QuestionRepository : RepositoryBase_IPRehab<tblQuestion>, IQuestionRepository
    {
        public QuestionRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class QuestionInstructionRepository : RepositoryBase_IPRehab<tblQuestionInstruction>, IQuestionInstructionRepository
    {
        public QuestionInstructionRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class SignatureRepository : RepositoryBase_IPRehab<tblSignature>, ISignatureRepository
    {
        public SignatureRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class UserRepository : RepositoryBase_IPRehab<tblUser>, IUserRepository
    {
        public UserRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }

    public class QuestionMeasureRepository : RepositoryBase_IPRehab<tblQuestionMeasure>, IQuestionMeasureRepository
    {
        public QuestionMeasureRepository(IPRehabContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
