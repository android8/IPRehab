﻿using IPRehabModel;
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

    /// <summary>
    /// external patient data source from BI13.DMHealthFactors
    /// only needed for CRUD operation which would never happen other than select
    /// </summary>
    public interface IFSODPatientRepository : IRepositoryBase<FSODPatient>
    {
    }

    /// <summary>
    /// DB9 internal view with remote query of BI25.DMHealthFactors
    /// only needed for CRUD operation which would never happen other than select
    /// </summary>
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

