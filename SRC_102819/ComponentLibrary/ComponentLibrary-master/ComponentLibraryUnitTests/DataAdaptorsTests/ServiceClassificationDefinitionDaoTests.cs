using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DataAdaptorsTests
{
    public class ServiceClassificationDefinitionDaoTests
    {
        [Fact]
        public void Constructor_ShouldCreateForValidInput()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring", "Flooring - description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            serviceClassificationDefinitionDao.Should().NotBe(null);
            serviceClassificationDefinitionDao.Value.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR");
            serviceClassificationDefinitionDao.Description.ShouldBeEquivalentTo(
                "FLOORING | DADO | PAVIOUR - description");
            serviceClassificationDefinitionDao.ClassificationDefinitionDaos.First()
                .Value.ShouldBeEquivalentTo("Flooring");
            serviceClassificationDefinitionDao.ClassificationDefinitionDaos.First()
                .Description.ShouldBeEquivalentTo("Flooring - description");
        }

        [Fact]
        public void Constructor_ShouldCreateForValidInputWithNullChildren()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", null);
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            serviceClassificationDefinitionDao.Should().NotBe(null);
            serviceClassificationDefinitionDao.Value.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR");
            serviceClassificationDefinitionDao.Description.ShouldBeEquivalentTo(
                "FLOORING | DADO | PAVIOUR - description");
            serviceClassificationDefinitionDao.ClassificationDefinitionDaos.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentExceptionForNullInput()
        {
            ClassificationDefinition classificationDefinition = null;
            Action act = () =>
            {
                var serviceClassificationDefinitionDao =
                    new ClassificationDefinitionDao(classificationDefinition);
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario1()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", null);
            var existing =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            ClassificationDefinitionDao newTreePath = null;
            Action act = () =>
            {
                existing.MergeNewTreePath(newTreePath);
            };
            act.ShouldThrow<DuplicateResourceException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario2()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", null);
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Flooring",
                "Flooring - description", null);
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            existing.MergeNewTreePath(newTreePath);
            existing.Domain().ShouldBeEquivalentTo(new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring",
                        "Flooring - description", null)
                }));
        }

        [Fact]
        public void MergeNewTreePath_Scenario3()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description", null)
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Flooring",
                "Flooring - description", null);
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            Action act = () =>
            {
                existing.MergeNewTreePath(newTreePath);
            };
            act.ShouldThrow<DuplicateResourceException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario3aDifferentStringCase()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description", null)
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("FLOORING",
                "Flooring - description", null);
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            Action act = () =>
            {
                existing.MergeNewTreePath(newTreePath);
            };
            act.ShouldThrow<DuplicateResourceException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario4()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description", null)
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Test",
                "Test - description", null);
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            existing.MergeNewTreePath(newTreePath);
            existing.Domain().ShouldBeEquivalentTo(new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring",
                        "Flooring - description", null),
                    new ClassificationDefinition("Test",
                        "Test - description", null)
                }));
        }

        [Fact]
        public void MergeNewTreePath_Scenario5_WithDifferentDefinitionForExistingPath()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description",
                            new List<ClassificationDefinition>
                            {
                                new ClassificationDefinition("Natural Stone", "Natural Stone - description", null)
                            })
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Flooring",
                "Flooring - another description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("test", "test - description", null)
                });
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            Action act = () =>
            {
                existing.MergeNewTreePath(newTreePath);
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario6()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description",
                            new List<ClassificationDefinition>
                            {
                                new ClassificationDefinition("Natural Stone", "Natural Stone -description",
                                    new List<ClassificationDefinition>
                                    {
                                        new ClassificationDefinition("Jaisalmer", "Jaisalmer - description", null)
                                    })
                            })
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Flooring",
                "Flooring - description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Natural Stone", "Natural Stone -description", null)
                });
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            Action act = () =>
            {
                existing.MergeNewTreePath(newTreePath);
            };
            act.ShouldThrow<DuplicateResourceException>();
        }

        [Fact]
        public void MergeNewTreePath_Scenario7()
        {
            var existingServiceClassificationDefinition =
                new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                    "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                    {
                        new ClassificationDefinition("Flooring",
                            "Flooring - description",
                            new List<ClassificationDefinition>
                            {
                                new ClassificationDefinition("Natural Stone", "Natural Stone -description",
                                    new List<ClassificationDefinition>
                                    {
                                        new ClassificationDefinition("Jaisalmer", "Jaisalmer - description",
                                            new List<ClassificationDefinition>
                                            {
                                                new ClassificationDefinition("Udaipur Green Marble",
                                                    "Udaipur Green Marble - description", null)
                                            })
                                    })
                            })
                    });
            var existing =
                new ClassificationDefinitionDao(existingServiceClassificationDefinition);
            var newServiceClassificationDefinition = new ClassificationDefinition("Flooring",
                "Flooring - description",
                new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Natural Stone", "Natural Stone -description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("test", "test - description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("test1", "test1 - description", null)
                                })
                        })
                });
            var newTreePath = new ClassificationDefinitionDao(newServiceClassificationDefinition);
            existing.MergeNewTreePath(newTreePath);
            existing.Domain().ShouldBeEquivalentTo(new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring",
                        "Flooring - description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("Natural Stone", "Natural Stone -description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("Jaisalmer", "Jaisalmer - description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("Udaipur Green Marble",
                                                "Udaipur Green Marble - description", null)
                                        }),
                                    new ClassificationDefinition("test", "test - description",
                                        new List<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("test1", "test1 - description", null)
                                        })
                                })
                        })
                }));
        }

        [Fact]
        public void ValidateTreePath_Scenario1()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring",
                        "Flooring - description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            Action act = () =>
            {
                serviceClassificationDefinitionDao.ValidateTreePath();
            };
            act.ShouldNotThrow<ArgumentException>();
        }

        [Fact]
        public void ValidateTreePath_Scenario2()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring",
                        "Flooring - description", null),
                    new ClassificationDefinition("Test",
                        "Test - description", null)
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            Action act = () =>
            {
                serviceClassificationDefinitionDao.ValidateTreePath();
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ValidateTreePath_Scenario3()
        {
            var serviceClassificationDefinition = new ClassificationDefinition("FLOORING | DADO | PAVIOUR",
                "FLOORING | DADO | PAVIOUR - description", new List<ClassificationDefinition>
                {
                    new ClassificationDefinition("Flooring", "Flooring - description",
                        new List<ClassificationDefinition>
                        {
                            new ClassificationDefinition("Natural Stone", "Natural Stone - description",
                                new List<ClassificationDefinition>
                                {
                                    new ClassificationDefinition("Kota Blue", "Kota Blue - descriptioin",
                                        new EditableList<ClassificationDefinition>
                                        {
                                            new ClassificationDefinition("Test1", "Test1 - description",
                                                new List<ClassificationDefinition>
                                                {
                                                    new ClassificationDefinition("Test2", "Test2 - description",
                                                        new List<ClassificationDefinition>
                                                        {
                                                            new ClassificationDefinition("Test3",
                                                                "Test3 - description",
                                                                new List<ClassificationDefinition>
                                                                {
                                                                    new ClassificationDefinition("Test4",
                                                                        "Test4 - description", null)
                                                                })
                                                        })
                                                })
                                        })
                                })
                        })
                });
            var serviceClassificationDefinitionDao =
                new ClassificationDefinitionDao(serviceClassificationDefinition);
            Action act = () =>
            {
                serviceClassificationDefinitionDao.ValidateTreePath();
            };
            act.ShouldThrow<ArgumentException>();
        }
    }
}