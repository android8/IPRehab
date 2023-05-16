﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IPRehabModel
{
    /// <summary>
    /// output EF generated sql statement to file "generatedSQL.txt"
    /// </summary>
    public partial class IPRehabContext : DbContext
    {

        static string exeRuntimeDirectory =
            System.IO.Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().Location);

        //static string pathAndFile = Path.Combine(Environment.CurrentDirectory, @"\logs\generatedSQL.txt");
        static string pathAndFile = System.IO.Path.Combine(exeRuntimeDirectory, @"\logs\generatedSQL.txt");
        private readonly StreamWriter _logStream = new StreamWriter(pathAndFile, append: true);


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.LogTo(_logStream.WriteLine);

        public override void Dispose()
        {
            base.Dispose();
            _logStream.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await _logStream.DisposeAsync();
        }
    }
}
