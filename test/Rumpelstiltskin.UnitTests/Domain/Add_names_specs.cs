using NUnit.Framework;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.UnitTests.TestTools;

namespace Add_names_specs
{
    public class All
    {
        [Test]
        public void unqiue_names_can_be_added()
        {
           NameSelection selection = Selection.Setup()
                .WithEvents(Selection.New())
                .Handle(new AddNames
                {
                    Id = Selection.Id,
                    Names = new[] { "Beatrix", "Juliana" },
                })
                .AssertIsValid();

            Assert.AreEqual(12, selection.Names.Count);
        }

        [Test]
        public void double_names_are_not_added()
        {
            NameSelection selection = Selection.Setup()
                 .WithEvents(Selection.New())
                 .Handle(new AddNames
                 {
                     Id = Selection.Id,
                     Names = new[] { "Beatrix", "Indi" },
                 })
                 .AssertIsValid(
                    ValidationMessage.Warn("Some names occurred multiple times: Indi."));

            Assert.AreEqual(11, selection.Names.Count);
        }


        [Test]
        public void at_least_10_names_should_be_added_to_empty_selection()
        {
            Selection.Setup()
                 .WithEvents(Selection.Empty())
                 .Handle(new AddNames
                 {
                     Id = Selection.Id,
                     Names = new[] { "Indi", "Orianthi" },
                 })
                 .AssertInvalid(
                    ValidationMessage.Error("There should be at least 10 names to choose from.", "Names"));
        }

        [Test]
        public void existing_names_can_not_be_added()
        {
            Selection.Setup()
                 .WithEvents(Selection.New())
                 .Handle(new AddNames
                 {
                     Id = Selection.Id,
                     Names = new[] { "Indi", "Orianthi" },
                 })
                 .AssertInvalid(
                ValidationMessage.Error("No (new) names have been added."));
        }
    }
}
