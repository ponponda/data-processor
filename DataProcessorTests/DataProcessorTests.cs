using NUnit.Framework;
using DataProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new MockData { Id = 2, Name = "Test B",
                Child = new MockData.MockChild {
                Id = 2, Name = "Child 1-1",
                Child = new MockData.MockChild {
                    Name = "Child 2-1"
                }
            } }
        };

        [Test()]
        public void Load() {
            var u = source.Select(e => new { e.Id, c = e.Child.Id });
            Assert.AreEqual(1, DataProcessor.Load(source, new TestLoadOption {
                Filter = new object[] { new object[] { "Id", "=", 2 }, new object[] { "Child.Id", "=", 2 } },
            }).TotalCount);
            //Assert.AreEqual(1, DataProcessor.Load(source, new TestLoadOption {
            //    Filter = new object[] { "NullableInt", "=", 1 },
            //}).TotalCount);
            //Assert.AreEqual(0, DataProcessor.Load(source, new TestLoadOption {
            //    Filter = new object[] { "Date", ">", new DateTime(2021, 1, 1) },
            //}).TotalCount);
            //Assert.AreEqual(1, DataProcessor.Load(source, new TestLoadOption {
            //    Filter = new object[] { "Child.Id", "=", 2 },
            //}).TotalCount);
        }

        [Test()]
        public void Select() {
            var options = new TestLoadOption {
                Select = new[] { "Id", "Child.Child.Name", "Child.Id", "Child.Child.Id" },
            };

            var data = DataProcessor.Load(source, options).Data.Cast<MockData>();
            var first = data.ElementAt(0);
            var second = data.ElementAt(1);
            Assert.Null(first.Child);
            Assert.AreEqual("Child 2-1", second.Child.Child.Name);
        }

        [Test()]
        public void Sort() {
            var options = new TestLoadOption {
                Sort = "Id",
                SortDescending = true
            };

            var firstId = DataProcessor.Load(source, options).Data.Cast<MockData>().First().Id;
            Assert.AreEqual(2, firstId);
        }
    }
}