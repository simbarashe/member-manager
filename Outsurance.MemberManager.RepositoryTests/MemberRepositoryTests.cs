using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Outsurance.MemberManager.Core.Entities;
using Outsurance.MemberManager.Repositories;
using System.Linq;

namespace Outsurance.MemberManager.RepositoryTests
{
    [TestClass]
    public class MemberRepositoryTests
    {
        private MemberRepository _memberRepository;
        private IEnumerable<Member> _members;

        [TestInitialize]
        public void Setup()
        {
            _memberRepository = new MemberRepository();

            _members = new List<Member>
            {
                new Member
                {
                    FirstName = "Jimmy",
                    LastName ="Smith",
                    PhoneNumber = "29384857",
                    Address = new Address
                    {
                        StreetNumber = 102,
                        StreetName = "Long Lane"
                    }
                },
                new Member
                {
                    FirstName = "Clive",
                    LastName ="Owen",
                    PhoneNumber = "31214788",
                    Address = new Address
                    {
                        StreetNumber = 65,
                        StreetName = "Ambling Way"
                    }
                },
                new Member
                {
                    FirstName = "James",
                    LastName ="Brown",
                    PhoneNumber = "32114566",
                    Address = new Address
                    {
                        StreetNumber = 82,
                        StreetName = "Stewart St"
                    }
                },
                new Member
                {
                    FirstName = "Clive",
                    LastName ="Smith",
                    PhoneNumber = "31214788",
                    Address = new Address
                    {
                        StreetNumber = 49,
                        StreetName = "Sutherland St"
                    }
                },
            };
        }

        [TestMethod]
        public void Should_GetAllMembers()
        {
            var memberRepository = new MemberRepository();

           var members = _memberRepository.Get();

            Assert.AreEqual(members.Count(), _members.Count());

            foreach (var member in members)
            {
                Assert.IsTrue(_members.Any(a=>a.FirstName == member.FirstName 
                                            && a.LastName == member.LastName 
                                            && a.PhoneNumber == member.PhoneNumber
                                            && a.Address.StreetName == member.Address.StreetName
                                             && a.Address.StreetNumber == member.Address.StreetNumber));
            }
        }

        [TestMethod]
        public void Should_AddAddresses()
        {
            var memberRepository = new MemberRepository();        
            var addresses = _members.Select(m =>m.Address);

            var success = memberRepository.Add(addresses);

            Assert.AreEqual(4, addresses.Count());
            Assert.IsTrue(success);            
        }

        [TestMethod]
        public void Should_AddNameFrequencies()
        {
            var memberRepository = new MemberRepository();
            var firstNameQuery = from m in _members
                                 select new { Name = m.FirstName };

            var lastNameQuery = from m in _members
                                select new { Name = m.LastName };

            var names = firstNameQuery.Concat(lastNameQuery);

            var nameFrequencies = (from m in names
                                  group m by m.Name into grp
                                  let count = grp.Count()
                                  let name = grp.Key
                                  select new NameFrequency {
                                      Name = name,
                                      Count = count })
                                  .OrderByDescending(a => a.Count)
                                  .ThenBy(a => a.Name);


            var success = memberRepository.Add(nameFrequencies);

            Assert.AreEqual(6, nameFrequencies.Count());
            Assert.IsTrue(success);
        }
    }
}
