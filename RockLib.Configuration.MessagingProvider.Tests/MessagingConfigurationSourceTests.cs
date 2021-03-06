using FluentAssertions;
using RockLib.Messaging;
using System;
using Xunit;

namespace RockLib.Configuration.MessagingProvider.Tests
{
    public class MessagingConfigurationSourceTests
    {
        [Fact]
        public void ConstructorThrowsIfReceiverIsNull()
        {
            Action action = () => new MessagingConfigurationSource(null);
            action.Should().ThrowExactly<ArgumentNullException>().WithMessage("*receiver*");
        }

        [Fact]
        public void ConstructorThrowsIfReceiverIsUsedByAnotherMessagingConfigurationSource()
        {
            var receiver = new FakeReceiver("fake");

            // Create a source with the receiver and throw it away.
            new MessagingConfigurationSource(receiver);

            // Passing the same receiver to another source causes it to throw.
            Action action = () => new MessagingConfigurationSource(receiver);
            action.Should().ThrowExactly<ArgumentException>().WithMessage("The same instance of IReceiver cannot be used to create multiple instances of MessagingConfigurationSource.*receiver");
        }

        [Fact]
        public void ConstructorThrowsIfReceiverIsAlreadyStarted()
        {
            var receiver = new FakeReceiver("fake");
            receiver.Start(m => m.AcknowledgeAsync());

            Action action = () => new MessagingConfigurationSource(receiver);
            action.Should().ThrowExactly<ArgumentException>().WithMessage("The receiver is already started.*receiver");
        }

        [Fact]
        public void ConstructorSetsReceiverProperty()
        {
            var receiver = new FakeReceiver("fake");

            var source = new MessagingConfigurationSource(receiver);

            source.Receiver.Should().BeSameAs(receiver);
        }

        [Fact]
        public void ConstructorSetsSettingFilterProperty()
        {
            var receiver = new FakeReceiver("fake");
            var filter = new FakeSettingFilter();

            var source = new MessagingConfigurationSource(receiver, filter);

            source.SettingFilter.Should().BeSameAs(filter);
        }

        [Fact]
        public void BuildMethodReturnsMessagingConfigurationProvider()
        {
            var receiver = new FakeReceiver("fake");
            var filter = new FakeSettingFilter();

            var source = new MessagingConfigurationSource(receiver, filter);

            var provider = source.Build(null);

            provider.Should().BeOfType<MessagingConfigurationProvider>();

            var messagingProvider = (MessagingConfigurationProvider)provider;

            messagingProvider.Receiver.Should().BeSameAs(receiver);
            messagingProvider.SettingFilter.Should().BeSameAs(filter);
        }

        [Fact]
        public void BuildMethodReturnsSameMessagingConfigurationProviderEachTime()
        {
            var receiver = new FakeReceiver("fake");

            var source = new MessagingConfigurationSource(receiver);

            var provider1 = source.Build(null);
            var provider2 = source.Build(null);

            provider1.Should().BeSameAs(provider2);
        }
    }
}
