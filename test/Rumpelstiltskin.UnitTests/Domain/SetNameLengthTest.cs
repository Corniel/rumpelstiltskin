//using NUnit.Framework;
//using Qowaiv.DomainModel.TestTools.EventSourcing;
//using Qowaiv.Validation.Abstractions;
//using Qowaiv.Validation.TestTools;
//using Rumpelstiltskin.Domain.Handlers;
//using Rumpelstiltskin.Domain.Commands;
//using Rumpelstiltskin.Domain.Events;
//using System.Threading.Tasks;

//namespace Rumpelstiltskin.UnitTests.Domain
//{
//    public class SetNameLengthTest
//    {
//        [Test]
//        public async Task SetNameLength_NegativeMinLength_NotAllowed()
//        {
//            await Setup(GetId(), new Created());

//            var command = new SetNameLength
//            {
//                Id = GetId(),
//                MinLength = -1,
//            };

//            var handler = GetHandler();
//            var results = await handler.HandleAsync(command);

//            ValidationMessageAssert.WithErrors(results, ValidationMessage.Error("Minimum length can not be negative.", "NameLength.MinLength"));
//        }

//        [Test]
//        public async Task SetNameLength_SetMaxLengthSmallerMinLength_NotAllowed()
//        {
//            await Setup(GetId(), new Created());

//            var command = new SetNameLength
//            {
//                Id = GetId(),
//                MinLength = 10,
//                MaxLength = 2,
//            };

//            var handler = GetHandler();
//            var results = await handler.HandleAsync(command);

//            ValidationMessageAssert.WithErrors(results, ValidationMessage.Error("Maximum length should be bigger than minimum length.", "NameLength.MaxLength"));
//        }

//        [Test]
//        public async Task SetNameLength_Min2Max10_Published()
//        {
//            var version = await Setup(GetId(), new Created());

//            var command = new SetNameLength
//            {
//                Id = GetId(),
//                MinLength = 02,
//                MaxLength = 10,
//            };

//            var handler = GetHandler();
//            await handler.HandleAsync(command);

//            var stream = await GetEventsAsync(GetId(), version);
//            AggregateRootAssert.HasUncommittedEvents(stream, new NameLengthSet { MinLength = 2, MaxLength = 10 });

//        }
//    }
//}
