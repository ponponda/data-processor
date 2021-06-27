using NUnit.Framework;
using DataProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.Tests {
    [TestFixture()]
    public class DataProcessorTests {
        public class MockData {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? NullableInt { get; set; }
            public DateTime Date { get; set; }
            public MockChild Child { get; set; }

            public class MockChild {
                public int Id { get; set; }
                public string Name { get; set; }
                public MockChild Child { get; set; }
            }
        }

        List<MockData> source = new List<MockData> {
            new MockData { Id = 1, Name = "Mock A", NullableInt = 1, Date = new DateTime(2021 ,1 ,1) },
            new MockData { Id = 2, Name = "Mock A", NullableInt = 2, Date = new DateTime(2021 ,1 ,1) },
            new MockData { Id = 3, Name = "Test B",
                Child = new MockData.MockChild {
                Id = 2, Name = "Child 1-1",
                Child = new MockData.MockChild {
                    Name = "Child 2-1"
                }
            } }
        };

        [Test()]
        public void Filter() {
            Assert.AreEqual(1, DataProcessor.Load(source, new DataSourceLoadOption {
                Filter = new object[] { new object[] { "Id", "=", 2 } },
            }).TotalCount);
            Assert.AreEqual(1, DataProcessor.Load(source, new TestLoadOption {
                Filter = new object[] { "Child.Id", "=", 2 },
            }).TotalCount);
        }

        [Test()]
        public void Filter_And() {
            var result = DataProcessor.Load(source, new DataSourceLoadOption {
                Filter = new object[] { new object[] { "Id", "=", 2 }, new object[] { "Child.Id", "=", 2 } },
            });
            Assert.AreEqual(0, result.TotalCount);
        }

        [Test()]
        public void Summary() {
            var options = new TestLoadOption {
                Summary = new[] { new SummaryInfo { Field = "Id", Type = "sum" } },
            };
            var data = DataProcessor.Load(source, options);
            Assert.AreEqual(6, data.Summary[0]);
        }

        [Test()]
        public void Select() {
            var options = new TestLoadOption {
                Select = new[] { "Id", "Child.Child.Name", "Child.Id", "Child.Child.Id" },
            };

            var data = DataProcessor.Load(source, options).Data.Cast<MockData>();
            var first = data.First();
            var last = data.Last();
            Assert.Null(first.Child);
            Assert.AreEqual("Child 2-1", last.Child.Child.Name);
        }

        [Test()]
        public void Group() {
            var options = new TestLoadOption {
                Group = new[] { "Id" },
            };
            var groups = DataProcessor.Load(source, options).Data.Cast<GroupResult>();
            Assert.AreEqual(1, groups.First().Key);
        }

        [Test()]
        public void Group_Nested() {
            var options = new TestLoadOption {
                Group = new[] { "Id", "Name" },
            };
            var groups = DataProcessor.Load(source, options).Data.Cast<GroupResult>();
            Assert.AreEqual(1, groups.First().Key);
            Assert.AreEqual("Mock A", groups.First().Items.Cast<GroupResult>().First().Key);
        }

        [Test()]
        public void GroupSummary() {
            var options = new TestLoadOption {
                Group = new[] { "Name" },
                GroupSummary = new[] {
                new SummaryInfo { Field = "Id", Type = "avg" },
                new SummaryInfo { Field = "Id", Type = "count" },
                new SummaryInfo { Field = "Id", Type = "min" },
                new SummaryInfo { Field = "Id", Type = "max" },
                new SummaryInfo { Field = "Id", Type = "sum" },
            }
            };
            var groups = DataProcessor.Load(source, options).Data.Cast<GroupResult>();
            Assert.AreEqual(1.5, groups.First().Summary[0]);
            Assert.AreEqual(2, groups.First().Summary[1]);
            Assert.AreEqual(1, groups.First().Summary[2]);
            Assert.AreEqual(2, groups.First().Summary[3]);
            Assert.AreEqual(3, groups.First().Summary[4]);
        }

        [Test()]
        public void Sort() {
            var options = new TestLoadOption {
                Sort = new[] {
                    new SortingInfo { Field = "Id", Desc = true }
                },
            };

            var firstId = DataProcessor.Load(source, options).Data.Cast<MockData>().First().Id;
            Assert.AreEqual(3, firstId);
        }
    }
}