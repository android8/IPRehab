﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using EF_StoreProc.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EF_StoreProc.Models
{
    public partial interface IRehabAssessmentCareToolContextProcedures
    {
        Task<List<sp_TreatingSpecialtiyPatientsResult>> sp_TreatingSpecialtiyPatientsAsync(string likeFacilityId, int? minusDays, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_UserAccessLevelAsync(string userName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<spQuestionAnswersResult>> spQuestionAnswersAsync(string facilityID6, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    }
}
