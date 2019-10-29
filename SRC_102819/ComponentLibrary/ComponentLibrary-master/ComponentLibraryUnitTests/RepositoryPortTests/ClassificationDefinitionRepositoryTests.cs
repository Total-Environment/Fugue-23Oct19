using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
    public class ClassificationDefinitionRepositoryTests
    {
        [Fact]
        public async void
            Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithDifferentLevel1Value()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description", null);
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("level1.2", "level1.2description",
                null);
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var fixture =
                new Fixture().WithExistingAndFindReturnEmptyList(existingServiceClassificationDefinitionDao)
                    .Adding(serviceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithNull()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);

            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);

            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .NotReplacing(replacedServiceClassificationDefinitionDao);

            Func<Task> act = async () => await fixture.SystemUnderTest()
                .CreateClassificationDefinition(null);
            act.ShouldThrowExactly<ArgumentException>();
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithSameLevel1Value()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null)
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("level1.1", "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null),
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);
            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .Replacing(replacedServiceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void
            Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithSameLevel1ValueAndLevel2Value()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("level1.1", "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.2", "level3.2description",
                                new List<ClassificationDefinition>())
                        })
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                }),
                            new ClassificationDefinition("level3.2", "level3.2description",
                                new List<ClassificationDefinition>())
                        })
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);
            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .Replacing(replacedServiceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void
            Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithSameLevel1ValueAndLevel2ValueDifferentStringCase()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("LEVEL1.1", "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("LEVEL2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.2", "level3.2description",
                                new List<ClassificationDefinition>())
                        })
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "level3.1description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "level4.1description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "level5.1description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1",
                                                        "level6.1description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "level7.1description",
                                                                new List<ClassificationDefinition>())
                                                        })
                                                })
                                        })
                                }),
                            new ClassificationDefinition("level3.2", "level3.2description",
                                new List<ClassificationDefinition>())
                        })
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);
            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .Replacing(replacedServiceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Add_ShouldSaveServiceClassificationDefinitionDao_OnExistingTree_WhenPassedWithSameLevel1ValueDifferentStringCase()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null)
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("LEVEL1.1", "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null),
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);
            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .Replacing(replacedServiceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Add_ShouldSaveServiceClassificationDefinitionDao_WhenPassedWithEmptyChildArray()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("level1", "level1description",
                null);
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var fixture = new Fixture().WithEmptyData().Adding(serviceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Add_ShouldSaveServiceClassificationDefinitionDao_WhenPassedWithOneChild()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("level1", "level1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2", "level2description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var fixture = new Fixture().WithEmptyData().Adding(serviceClassificationDefinitionDao);
            await fixture.SystemUnderTest().CreateClassificationDefinition(serviceClassificationDefinitionDao);
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowInvalidArgumentException_OnExistingTree_WhenPassedWithSameLevel1ValueDifferentDefinition()
        {
            var existingServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null)
                });
            var existingServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var serviceClassificationDefinition = new ClassificationDefinition("level1.1", "level1.1anotherdescription",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var replacedServiceClassificationDefinition = new ClassificationDefinition("level1.1",
                "level1.1description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "level2.1description", null),
                    new ClassificationDefinition("level2.2", "level2.2description", null)
                });
            var replacedServiceClassificationDefinitionDao =
                new ClassificationDefinitionDao(replacedServiceClassificationDefinition);
            var fixture =
                new Fixture().WithExisting(existingServiceClassificationDefinitionDao)
                    .NotReplacing(replacedServiceClassificationDefinitionDao);
            Func<Task> act = async () => await fixture.SystemUnderTest()
                .CreateClassificationDefinition(serviceClassificationDefinitionDao);
            act.ShouldThrowExactly<ArgumentException>();
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowInvalidArgumentException_WhenPassedTreeStructure()
        {
            var serviceClassificationDefinition =
                new ClassificationDefinition("level1.1", "description1",
                    new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("level2.1", "description2",
                            new List<ClassificationDefinition>()),
                        new ClassificationDefinition("level2.2", "description3",
                            new List<ClassificationDefinition>())
                    });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var fixture = new Fixture().WithEmptyData().NotAdding(serviceClassificationDefinitionDao);
            Func<Task> act = async () => await fixture.SystemUnderTest()
                .CreateClassificationDefinition(
                    new ClassificationDefinitionDao(serviceClassificationDefinition));
            act.ShouldThrowExactly<ArgumentException>();
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowInvalidArgumentException_WhenPassedTreeWithMoreThanSevenLevels()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("level1.1", "description1",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("level2.1", "description2",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("level3.1", "description3",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("level4.1", "description4",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("level5.1", "description5",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("level6.1", "description6",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("level7.1",
                                                                "description7",
                                                                new List<ClassificationDefinition>
                                                                {
                                                                    new ClassificationDefinition("level8.1",
                                                                        "description8",
                                                                        new List<ClassificationDefinition> {})
                                                                })
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            var fixture = new Fixture().WithEmptyData().NotAdding(serviceClassificationDefinitionDao);
            Func<Task> act = async () => await fixture.SystemUnderTest()
                .CreateClassificationDefinition(
                    new ClassificationDefinitionDao(serviceClassificationDefinition));
            act.ShouldThrowExactly<ArgumentException>();
            fixture.VerifyExpectations();
        }

        [Fact]
        public void
            ServiceClassificationDefinitionRepository_ShouldBeAssignableToIServiceClassificationDefinitionRepository()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IClassificationDefinitionRepository>();
        }

        private class Fixture
        {
            private readonly Mock<IMongoCollection<ClassificationDefinitionDao>> _mockCollection =
                new Mock<IMongoCollection<ClassificationDefinitionDao>>();

            private readonly List<Action> _verifications = new List<Action>();

            public Fixture Adding(ClassificationDefinitionDao classificationDefinitionDao)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(It.Is<ClassificationDefinitionDao>(descDao =>
                                            descDao.Domain().Equals(classificationDefinitionDao.Domain())),
                                    It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public Mock<IMongoCollection<ClassificationDefinitionDao>> MongoCollection()
            {
                return _mockCollection;
            }

            public Fixture NotAdding(ClassificationDefinitionDao classificationDefinitionDao)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m => m.InsertOneAsync(It.Is<ClassificationDefinitionDao>(descDao =>
                                        descDao.Domain().Equals(classificationDefinitionDao.Domain())),
                                null, default(CancellationToken)),
                            Times.Never));
                return this;
            }

            public Fixture NotReplacing(ClassificationDefinitionDao classificationDefinitionDao)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.ReplaceOneAsync(It.IsAny<FilterDefinition<ClassificationDefinitionDao>>(),
                                    It.Is<ClassificationDefinitionDao>(descDao =>
                                            descDao.Domain().Equals(classificationDefinitionDao.Domain())),
                                    null, default(CancellationToken)),
                            Times.Never));
                return this;
            }

            public Fixture Replacing(ClassificationDefinitionDao classificationDefinitionDao)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.ReplaceOneAsync(It.IsAny<FilterDefinition<ClassificationDefinitionDao>>(),
                                    It.Is<ClassificationDefinitionDao>(descDao =>
                                            descDao.Domain().Equals(classificationDefinitionDao.Domain())),
                                    It.IsAny<UpdateOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));

                return this;
            }

            public ClassificationDefinitionRepository SystemUnderTest()
            {
                return new ClassificationDefinitionRepository(_mockCollection.Object);
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithEmptyData()
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            public Fixture WithExisting(ClassificationDefinitionDao classificationDefinitionDao)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, classificationDefinitionDao);
                return this;
            }

            public Fixture WithExistingAndFindReturnEmptyList(
                ClassificationDefinitionDao classificationDefinitionDao)
            {
                TestHelper.MockCollectionWithExistingAndFindReturnEmptyList(_mockCollection,
                    classificationDefinitionDao);
                return this;
            }
        }
    }
}