using Outsurance.MemberManager.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outsurance.MemberManager.Core.Entities;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Outsurance.MemberManager.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly string _addressesFilePath;
        private readonly string _nameFrequenciesFilePath;
        private readonly string _membersFilePath;
        private readonly string _exportsFolderPath;
        private readonly string _importsFolderPath;
        private readonly string _filesBaseFolderPath;
        public MemberRepository()
        {
            _filesBaseFolderPath = App.Default.FilesBaseFolderPath;
            _exportsFolderPath = Path.Combine(_filesBaseFolderPath, App.Default.ExportsFolderName);
            _importsFolderPath = Path.Combine(_filesBaseFolderPath, App.Default.ImportsFolderName);
            _addressesFilePath = Path.Combine(_exportsFolderPath, App.Default.AddressesFileName);
            _nameFrequenciesFilePath = Path.Combine(_exportsFolderPath, App.Default.NameFrequenciesFileName);
            _membersFilePath = Path.Combine(_importsFolderPath, App.Default.MembersFileName);
        }
        public IEnumerable<Member> Get()
        {
            if (!Directory.Exists(_importsFolderPath))
                Directory.CreateDirectory(_importsFolderPath);

            if (!File.Exists(_membersFilePath))
                yield break;

            var reader = new CsvReader(new StreamReader(_membersFilePath));
            while (reader.Read())
            {
                var firstName = reader.GetField("FirstName");
                var lastName = reader.GetField("LastName");
                var address = reader.GetField("Address");
                var phoneNumber = reader.GetField("PhoneNumber");
                var index = address.IndexOf(' ');
                yield return new Member
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    Address = new Address
                    {
                        StreetNumber = Convert.ToInt32(address.Substring(0, index)),
                        StreetName = address.Substring(index + 1, address.Length - (index + 1))
                    }
                };
            }
        }

        public bool Add(IEnumerable<Address> addresses)
        {            
            if (!addresses.Any())
                return false;

            if (!Directory.Exists(_exportsFolderPath))
                Directory.CreateDirectory(_exportsFolderPath);

            using (var writer = new CsvWriter(new StreamWriter(_addressesFilePath)))
            {
                foreach (var address in addresses)
                {
                    writer.WriteField($"{address.StreetNumber} {address.StreetName}");
                    writer.NextRecord();
                }
            }
            return true;
        }

        public bool Add(IEnumerable<NameFrequency> nameFrequencies)
        {
            if (!nameFrequencies.Any())
                return false;

            if (!Directory.Exists(_exportsFolderPath))
                Directory.CreateDirectory(_exportsFolderPath);

            using (var writer = new CsvWriter(new StreamWriter(_nameFrequenciesFilePath)))
            {
                foreach (var nameFrequency in nameFrequencies)
                {
                    writer.WriteField($"{nameFrequency.Name}");
                    writer.WriteField($"{nameFrequency.Count}");
                    writer.NextRecord();
                }
            }
            return true;
        }
    }
}
