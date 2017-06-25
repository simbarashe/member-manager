using Outsurance.MemberManager.Core.Repositories;
using Outsurance.MemberManager.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outsurance.MemberManager.Core.Entities;

namespace Outsurance.MemberManager.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        public MemberService(IMemberRepository memberRepository )
        {
            _memberRepository = memberRepository;
        }

        public void Analyse()
        {
            var members = _memberRepository.Get();
            var addresses = GetAddresses(members);
            _memberRepository.Add(addresses);
            var nameFrequencies = GetNameFrequencies(members);
            _memberRepository.Add(nameFrequencies);        
        }

        private IEnumerable<NameFrequency> GetNameFrequencies(IEnumerable<Member> members)
        {
            var firstNameQuery = from m in members
                                 select new { Name = m.FirstName };

            var lastNameQuery = from m in members
                                select new { Name = m.LastName };

            var names = firstNameQuery.Concat(lastNameQuery);

            var nameFrequencies = from m in names
                                  group m by m.Name into grp
                                  let count = grp.Count()
                                  let name = grp.Key                                 
                                  select new NameFrequency { Name = name, Count = count };
            return nameFrequencies.OrderByDescending(a=>a.Count)
                                  .ThenBy(a=>a.Name);
        }

        private IEnumerable<Address> GetAddresses(IEnumerable<Member> members)
        {
            return members.Select(a => a.Address)
                          .OrderBy(a=>a.StreetName)
                          .ThenBy(a=>a.StreetNumber);
        }
    }
}
