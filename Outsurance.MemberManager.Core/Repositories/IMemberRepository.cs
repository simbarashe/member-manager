using Outsurance.MemberManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outsurance.MemberManager.Core.Repositories
{
    public interface IMemberRepository
    {
        IEnumerable<Member> Get();
        bool Add(IEnumerable<NameFrequency> nameFrequencies);
        bool Add(IEnumerable<Address> addresses);
    }
}
