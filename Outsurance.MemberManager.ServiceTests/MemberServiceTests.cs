using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Outsurance.MemberManager.Services;
using Outsurance.MemberManager.Core.Repositories;
using Outsurance.MemberManager.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Outsurance.MemberManager.ServiceTests
{
    [TestClass]
    public class MemberServiceTests
    {
        private MemberService _memberService;
        private Mock<IMemberRepository> _memberRepository;
        private IEnumerable<Member> _members;

        [TestInitialize]
        public void Setup()
        {
            _memberRepository = new Mock<IMemberRepository>();
            _memberService = new MemberService(_memberRepository.Object);
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
                }
            };
        }

        [TestMethod]
        public void Should_GetMembers_When_Analysing()
        {
            _memberRepository.Setup(x => x.Get())
                    .Returns(new List<Member>());

            _memberService.Analyse();

            _memberRepository.Verify(x => x.Get(), Times.Once());
        }

        [TestMethod]
        public void Should_AddAddresses_When_Analysing()
        {
            //Arrange
            _memberRepository.Setup(x => x.Get())
                             .Returns(_members);

            IEnumerable<Address> addresses = null;
            _memberRepository.Setup(x => x.Add(It.IsAny<IEnumerable<Address>>()))
                             .Callback<IEnumerable<Address>>(list =>
                             {
                                 addresses = list;
                             });

            _memberService.Analyse();

            _memberRepository.Verify(m => m.Add(It.Is<IEnumerable<Address>>(list => list.Equals(addresses))), Times.Once());
            Assert.IsNotNull(addresses);
            Assert.IsTrue(addresses.First().StreetName == "Ambling Way");
            Assert.IsTrue(addresses.Last().StreetName == "Sutherland St");
        }

        [TestMethod]
        public void Should_AddNameFrequencies_When_Analysing()
        {
            _memberRepository.Setup(x => x.Get())
                    .Returns(_members);          

            IEnumerable<NameFrequency> nameFrequencies = null;
            _memberRepository.Setup(m => m.Add(It.IsAny<IEnumerable<NameFrequency>>()))
                            .Callback<IEnumerable<NameFrequency>>(list =>
                            {
                                nameFrequencies = list;
                            });

            _memberService.Analyse();

            _memberRepository.Verify(m => m.Add(It.Is<IEnumerable<NameFrequency>>(list => list.Equals(nameFrequencies))), Times.Once());
            Assert.IsNotNull(nameFrequencies);
            Assert.AreEqual(1,nameFrequencies.Count(x => x.Name == "Clive" && x.Count == 2));
            Assert.AreEqual(1, nameFrequencies.Count(x => x.Name == "Smith" && x.Count == 2));

            Assert.IsTrue(nameFrequencies.First().Name == "Clive");
            Assert.IsTrue(nameFrequencies.Last().Name == "Owen");
        }


    }
}
