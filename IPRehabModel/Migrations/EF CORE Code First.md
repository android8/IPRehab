# Entity Framework Code First with CORE Identity

Use the `PCC_FIT_Model_CORELibrary` and the `PPC_FIT_Repository_CORELibrary` to manage the data. To jump-start EF CORE, use the _`EF CORE Power Tools`_ to generate the codebase.  After that, do not use the Power Tools again.  Otherwise, it will erase all previously generated and modified code. 

### PCC_FIT_Model_CORELibrary

Use the `UserManager` of CORE Identity to manage the ASPNET tables in the PCC_FIT_Model_CORELibrary.

- remove or comment out the following from PCC_FITcontext.cs if found

      public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
      public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
      public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
      public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
      public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
      public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
      public virtual DbSet<AspNetUsers> AspNetUsers { get; set; } 

- modify the C# classes first in the PCC_FIT_Model_CORE_Library project until satisfied

  - modify POCO class in the target tblxxx.cs
  - modify the column attributes, key, foreign key, relationship, etc in the target xxxConfiguration.cs
 
- add the POCO and Configuration to the PCCFIT_Context.cs

      public virtual DbSet<tblStaffFacilityRelation> tblStaffFacilityRelation { get; set; }
      modelBuilder.ApplyConfiguration(new tblGrantFundingConfiguration());
- in the Solution Explorer, reduce startup project to only 1 project before migration.  Otherwise, the migration would fail

- in the Package Manager Console 

  > add-migration someName -v
  
- review and modify the up() and down() in the Migrations folder.  

  > remove-migration (to undo generated code)
  > 
  > update-database _-migration "name of the transcript shown in the Solution Explorer"_ (to commit the update with the generated code)


### PCC_FIT_Repository_CORELibrary

- create new interface IPCC_FIT_Repository.cs in the Contracts folder
- create concrete class to implement the interface
- add the conrete class to the RepositoryAggregate.cs
