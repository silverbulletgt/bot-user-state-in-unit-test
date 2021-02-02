using BotUserStateTest.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BotUserStateTest.Unit.Tests
{
    [TestClass]
    public class SaveAndRetrieveUserStateDialogTest
    {
        [TestMethod]
        public void SaveAndRetrieveUserStateDialog_Test()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();

            serviceDescriptors.AddSingleton<IStorage>(new MemoryStorage());
            serviceDescriptors.AddSingleton<UserState>();

            serviceDescriptors.AddSingleton<StateService>();

            var services = serviceDescriptors.BuildServiceProvider();

            StateService stateService = services.GetService<StateService>();

            UserState userState = services.GetService<UserState>();
            AutoSaveStateMiddleware autoSaveStateMiddleware = new AutoSaveStateMiddleware(userState);

            var dialog = new SaveAndRetrieveUserStateDialog(stateService);
            var testClient = new DialogTestClient(Channels.Emulator, dialog, middlewares: new List<IMiddleware>() { autoSaveStateMiddleware });

            string userInitialText = "ValueToPutInUserState";

            var reply = testClient.SendActivityAsync<IMessageActivity>(userInitialText).Result;
            Assert.AreEqual("Say something else to retrieve the value.", reply.Text);

            reply = testClient.SendActivityAsync<IMessageActivity>("Get The Value").Result;
            Assert.AreEqual(userInitialText, reply.Text);
        }
    }
}
